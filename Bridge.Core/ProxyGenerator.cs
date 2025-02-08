using Bridge.Abstraction;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;

namespace Bridge.Core
{
    public class ProxyGenerator : IProxyGenerator
    {
        public void Generate(Assembly assembly, string outputPath)
        {
            List<Type> allMQHandlers = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MQHandlerBase))).ToList();

            var modelCompileUnit = CreateClass("Proxy.Model", $"ProxyModel");

            foreach (Type mqHandler in allMQHandlers)
            {
                var mqHandlerAttribute = mqHandler.GetCustomAttribute<MQHandlerAttribute>();
                if(mqHandlerAttribute != null)
                {
                    var compileUnit = CreateClass("Proxy", $"{mqHandler.Name}Proxy");

                    AddField(compileUnit, typeof(IPublisher), "_publisher");
                    AddField(compileUnit, typeof(MQType), "_mqType");

                    var mqType = mqHandlerAttribute.MQType;
                    var queueName = mqHandlerAttribute.QueueName;
                    var isMulticast = mqHandlerAttribute.IsMulticast;

                    AddContructor(compileUnit, mqType, new[] { (typeof(IPublisher), "publisher") });

                    MethodInfo[] methods = mqHandler.GetMethods();

                    foreach (MethodInfo method in methods)
                    {
                        var mqActionAttribute = method.GetCustomAttribute<MQActionAttribute>();
                        if(mqActionAttribute != null)
                        {
                            var actionName = mqActionAttribute.Action;
                            var returnType = method.ReturnType;
                            if(method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                            {
                                returnType = method.ReturnType.GenericTypeArguments.First();
                            }
                            else if (method.ReturnType.Equals(typeof(Task)))
                            {
                                returnType = typeof(void);
                            }
                            AddMethod(compileUnit, returnType, isMulticast, method.Name, queueName, actionName, parameters: method.GetParameters().Select(m => (m.ParameterType, m.Name!)).ToArray());
                            
                            if (!returnType.Equals(typeof(void)))
                            {
                                CreateModel(modelCompileUnit, returnType);
                            }
                            foreach (var parameter in method.GetParameters())
                            {
                                CreateModel(modelCompileUnit, parameter.ParameterType);
                            }
                        }
                    }
                    GenerateCSFile(outputPath, $"{mqHandler.Name}Proxy", compileUnit);
                }
            }
            GenerateCSFile(outputPath, $"ProxyModel", modelCompileUnit);
        }

        private void GenerateCSFile(string outputPath, string fileName, CodeCompileUnit compileUnit)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            string sourceFile = fileName + "." + provider.FileExtension;
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, sourceFile), false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw, "    ");
                provider.GenerateCodeFromCompileUnit(compileUnit, tw, new CodeGeneratorOptions());
                tw.Close();
            }
        }
        private CodeNamespace GetCodeNamespace(CodeCompileUnit compileUnit, string? codeNamespaceName)
        {
            codeNamespaceName = codeNamespaceName ?? string.Empty;
            CodeNamespace? codeNamespace = null;
            for (int i = 0; i < compileUnit.Namespaces.Count; i++)
            {
                if (compileUnit.Namespaces[i].Name == codeNamespaceName)
                {
                    codeNamespace = compileUnit.Namespaces[i];
                }
            }
            if (codeNamespace == null)
            {
                codeNamespace = new CodeNamespace(codeNamespaceName);
                compileUnit.Namespaces.Add(codeNamespace);
            }
            return codeNamespace;
        }
        private CodeTypeDeclaration GetCodeType(CodeCompileUnit compileUnit, string? codeNamespaceName, string codeTypeName, Action<CodeTypeDeclaration> decorateCodeTypeIfNew)
        {
            var codeNamespace = GetCodeNamespace(compileUnit, codeNamespaceName);

            CodeTypeDeclaration? codeType = null;
            for (int i = 0; i < codeNamespace.Types.Count; i++)
            {
                if (codeNamespace.Types[i].Name == codeTypeName)
                {
                    codeType = codeNamespace.Types[i];
                }
            }
            if (codeType == null)
            {
                codeType = new CodeTypeDeclaration(codeTypeName);
                codeNamespace.Types.Add(codeType);
                decorateCodeTypeIfNew(codeType);
            }
            return codeType;
        }
        private CodeCompileUnit CreateClass(string classNamespace, string className)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            CodeNamespace codeNamespace = new CodeNamespace(classNamespace);

            compileUnit.Namespaces.Add(codeNamespace);
            CodeTypeDeclaration class1 = new CodeTypeDeclaration(className);
            codeNamespace.Types.Add(class1);

            return compileUnit;
        }

        private CodeCompileUnit CreateModel(CodeCompileUnit compileUnit, Type modelType)
        {
            if (modelType.IsGenericType && !modelType.IsGenericTypeDefinition)
            {
                foreach (var genericTypeArgument in modelType.GenericTypeArguments)
                {
                    CreateModel(compileUnit, genericTypeArgument);
                }

                return CreateModel(compileUnit, modelType.GetGenericTypeDefinition());
            }
            
            if (modelType.Namespace != null
                && (modelType.Namespace.Equals("System") || modelType.Namespace.StartsWith("System.")))
            {
                return compileUnit;
            }
            var modelName = modelType.Name;

            if (modelType.IsGenericTypeDefinition)
            {
                string[]? genericTypeParameterNames = (modelType as TypeInfo)?.GenericTypeParameters.Select(x => x.Name).ToArray();
                modelName = genericTypeParameterNames is null ? 
                    modelName : $"{modelType.Name.Split('`').First()}<{string.Join(", ", genericTypeParameterNames)}>";
            }

            var codeType = GetCodeType(compileUnit, modelType.Namespace, modelName,
               ct =>
               {
                   if (modelType.IsEnum)
                   {
                       ct.IsEnum = true;
                   }
                   else if (modelType.IsValueType)
                   {
                       ct.IsStruct = true;
                   }
                   AddMembers(compileUnit, ct, modelType);
               }
               );

            return compileUnit;
        }

        private CodeTypeDeclaration AddLiteralMember(CodeTypeDeclaration codeType, FieldInfo literal)
        {
            var codeField = new CodeMemberField
            {
                Name = literal.Name,
                InitExpression = new CodePrimitiveExpression(literal.GetRawConstantValue())
            };

            codeType.Members.Add(codeField);

            return codeType;
        }

        private CodeTypeDeclaration AddPropertyMember(CodeTypeDeclaration codeType, PropertyInfo property)
        {
            var codeProperty = new CodeMemberProperty();
            codeProperty.Name = property.Name;

            codeProperty.Type = new CodeTypeReference(property.PropertyType);

            codeProperty.Attributes = MemberAttributes.Final;

            if (property.Attributes.HasFlag(PropertyAttributes.None))
            {
                codeProperty.Attributes |= MemberAttributes.Public;
            }

            var fieldName = "_" + property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);

            CodeMemberField field = new CodeMemberField(codeProperty.Type, fieldName);
            field.Attributes = MemberAttributes.Private;

            codeType.Members.Add(field);

            codeProperty.GetStatements.Add(new CodeMethodReturnStatement(
                new CodeFieldReferenceExpression(
                new CodeThisReferenceExpression(), fieldName)));

            codeProperty.SetStatements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
                new CodePropertySetValueReferenceExpression()));

            codeType.Members.Add(codeProperty);
            
            return codeType;
        }

        private CodeTypeDeclaration AddFieldMember(CodeTypeDeclaration codeType, FieldInfo field)
        {
            var codeField = new CodeMemberField();

            codeField.Name = field.Name;
            codeField.Type = new CodeTypeReference(field.FieldType);
            codeField.Attributes = MemberAttributes.Final;
            if (field.IsPublic)
            {
                codeField.Attributes |= MemberAttributes.Public;
            }
            if (field.IsStatic)
            {
                codeField.Attributes |= MemberAttributes.Static;
            }
            if (field.IsPrivate)
            {
                codeField.Attributes |= MemberAttributes.Private;
            }
            
            codeType.Members.Add(codeField);

            return codeType;
        }

        private CodeTypeDeclaration AddMembers(CodeCompileUnit compileUnit, CodeTypeDeclaration codeType, Type type)
        {            
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.IsGenericParameter)
                {
                    CreateModel(compileUnit, property.PropertyType);
                }

                AddPropertyMember(codeType, property);
            }

            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (!field.FieldType.IsGenericParameter)
                {
                    CreateModel(compileUnit, field.FieldType);
                }

                if (field.IsLiteral)
                {
                    AddLiteralMember(codeType, field);
                }
                else if(!field.Attributes.HasFlag(FieldAttributes.RTSpecialName))
                {
                    AddFieldMember(codeType, field);
                }
            }
            
            return codeType;
        }

        private CodeCompileUnit AddField(CodeCompileUnit compileUnit, Type type, string name)
        {
            CodeMemberField field1 = new CodeMemberField(type, name);
            field1.Attributes = MemberAttributes.Private;
            compileUnit.Namespaces[0].Types[0].Members.Add(field1);
            return compileUnit;
        }

        private CodeCompileUnit AddContructor(CodeCompileUnit compileUnit, MQType mqType, (Type, string)[] parameters)
        {
            CodeConstructor stringConstructor = new CodeConstructor();
            stringConstructor.Attributes = MemberAttributes.Public;
            foreach (var parameter in parameters)
            {
                stringConstructor.Parameters.Add(new CodeParameterDeclarationExpression(parameter.Item1, parameter.Item2));
            }
            stringConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("_publisher = publisher")));
            stringConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"_mqType = {mqType.GetType().Namespace}.{mqType.GetType().Name}.{mqType.ToString()}")));

            compileUnit.Namespaces[0].Types[0].Members.Add(stringConstructor);
            return compileUnit;
        }

        private CodeMemberMethod BuildMethod(CodeTypeReference returnType, string name, (Type, string)[] parameters, Action<CodeStatementCollection> buildStatements)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = name.EndsWith("Async") ? name : $"{name}Async";
            method.ReturnType = returnType;
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            buildStatements(method.Statements);

            foreach (var parameter in parameters)
            {
                method.Parameters.Add(new CodeParameterDeclarationExpression(parameter.Item1, parameter.Item2));
            }
            return method;
        }

        private CodeCompileUnit AddMethod(CodeCompileUnit compileUnit, Type returnType, bool isMulticast, string name, string queueName, string actionName, (Type, string)[] parameters)
        {
            var codeReturn = returnType.Equals(typeof(void)) || isMulticast ?
                new CodeTypeReference("async Task") : new CodeTypeReference("async Task", new CodeTypeReference(returnType));

            var method = BuildMethod(codeReturn, name, parameters, (statements) =>
            {
                var invokeMethodName = "PublishAsync";
                var typeParameters = new List<CodeTypeReference>();

                if (isMulticast)
                {
                    invokeMethodName = "PublishMulticastAsync";
                }
                else if (!returnType.Equals(typeof(void)))
                {
                    invokeMethodName = "PublishAndWaitReplyAsync"; 
                    if (parameters.Any())
                    {
                        typeParameters.Add(new CodeTypeReference(parameters.First().Item1));
                        typeParameters.Add(new CodeTypeReference(returnType));
                    }
                    else
                    {
                        typeParameters.Add(new CodeTypeReference(returnType));
                    }
                }
                
                CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(new CodeTypeReferenceExpression("await _publisher"), 
                        invokeMethodName, typeParameters.ToArray()),
                    new CodeArgumentReferenceExpression("_mqType"),
                    new CodeArgumentReferenceExpression($"\"{queueName}\""),
                    new CodeArgumentReferenceExpression($"\"{actionName}\""));

                foreach (var parameter in parameters)
                {
                    cs1.Parameters.Add(new CodeSnippetExpression(parameter.Item2));
                }
                if(returnType.Equals(typeof(void)) || isMulticast)
                {
                    statements.Add(cs1);
                }
                else
                {
                    statements.Add(new CodeMethodReturnStatement(cs1));
                }
            });

            compileUnit.Namespaces[0].Types[0].Members.Add(method);

            return compileUnit;
        }
    }
}
