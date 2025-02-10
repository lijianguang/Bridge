using Bridge.Abstraction;
using Microsoft.CSharp;
using Microsoft.VisualBasic.FileIO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bridge.Core
{
    public class ProxyGenerator : IProxyGenerator
    {
        private string _namespacePrefix = string.Empty;
        public IProxyGenerator SetNamespacePrefix(string namespacePrefix)
        {
            _namespacePrefix = namespacePrefix;
            return this;
        }
        public void Generate(Assembly assembly, params string[] outputPaths)
        {
            List<Type> allMQHandlers = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MQHandlerBase))).ToList();

            var modelCompileUnit = CreateCodeCompileUnit();

            foreach (Type mqHandler in allMQHandlers)
            {
                var mqHandlerAttribute = mqHandler.GetCustomAttribute<MQHandlerAttribute>();
                if(mqHandlerAttribute != null)
                {
                    var compileUnit = CreateCodeCompileUnit();
                    
                    var codeNamespace = GetCodeNamespace(compileUnit, "");

                    var codeType = GetCodeType(codeNamespace, $"{mqHandler.Name}Proxy", default);

                    AddPrivateField(codeType, typeof(IPublisher), "_publisher");
                    AddPrivateField(codeType, typeof(MQType), "_mqType");

                    var mqType = mqHandlerAttribute.MQType;
                    var queueName = mqHandlerAttribute.QueueName;
                    var isMulticast = mqHandlerAttribute.IsMulticast;

                    AddContructor(codeType, new[] { (typeof(IPublisher), "publisher") },
                        statements =>
                        {
                            statements.Add(new CodeExpressionStatement(new CodeSnippetExpression("_publisher = publisher")));
                            statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"_mqType = global::{mqType.GetType().FullName}.{mqType.ToString()}")));
                        });

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
                            AddMethod(codeType, returnType, isMulticast, method.Name, queueName, actionName, parameters: method.GetParameters().Select(m => (m.ParameterType, m.Name!)).ToArray());
                            
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
                    foreach(var path in outputPaths)
                    {
                        GenerateCSFile(path, $"{mqHandler.Name}Proxy", compileUnit);
                    }
                }
            }
            foreach (var path in outputPaths)
            {
                GenerateCSFile(path, $"ProxyModel", modelCompileUnit);
            }
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
            codeNamespaceName = string.IsNullOrEmpty(codeNamespaceName)  ? "" : codeNamespaceName;
            codeNamespaceName = string.IsNullOrEmpty(_namespacePrefix) ? codeNamespaceName : $"{_namespacePrefix}.{codeNamespaceName}";
            codeNamespaceName = codeNamespaceName.TrimStart('.').TrimEnd('.');

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

        private CodeTypeDeclaration GetCodeType(CodeNamespace codeNamespace, string codeTypeName, Action<CodeTypeDeclaration>? decorateCodeTypeIfNew)
        {
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
                if(decorateCodeTypeIfNew != null)
                {
                    decorateCodeTypeIfNew(codeType);
                }
            }
            return codeType;
        }

        private CodeCompileUnit CreateCodeCompileUnit()
        {
            return new CodeCompileUnit();
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
            var codeNamespace = GetCodeNamespace(compileUnit, modelType.Namespace);
            var codeType = GetCodeType(codeNamespace, modelName,
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

        private CodeMemberField AddPrivateField(CodeTypeDeclaration codeType, Type type, string name)
        {
            CodeMemberField field = new CodeMemberField();
            field.Type = new CodeTypeReference($"global::{type.FullName}");
            field.Name = name;
            field.Attributes = MemberAttributes.Private;
            codeType.Members.Add(field);
            return field;
        }

        private CodeConstructor AddContructor(CodeTypeDeclaration codeType, (Type, string)[] parameters, Action<CodeStatementCollection>? buildStatements)
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            foreach (var parameter in parameters)
            {
                constructor.Parameters.Add(new CodeParameterDeclarationExpression($"global::{parameter.Item1.FullName}", parameter.Item2));
            }
            if(buildStatements != null)
            {
                buildStatements(constructor.Statements);
            }

            codeType.Members.Add(constructor);
            return constructor;
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

        private CodeMemberMethod AddMethod(CodeTypeDeclaration codeType, Type returnType, bool isMulticast, string name, string queueName, string actionName, (Type, string)[] parameters)
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

            codeType.Members.Add(method);

            return method;
        }
    }
}
