using Bridge.Abstraction;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
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
                            if (method.GetParameters().Any())
                            {
                                CreateModel(modelCompileUnit, method.GetParameters().First().ParameterType);
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
            if (modelType.Namespace == null 
                || modelType.Namespace.Equals("System")
                || modelType.Namespace.StartsWith("System."))
            {
                return null;
            }

            CodeNamespace? codeNamespace = null;
            for (int i = 0; i < compileUnit.Namespaces.Count; i++)
            {
                if(compileUnit.Namespaces[i].Name == modelType.Namespace)
                {
                    codeNamespace = compileUnit.Namespaces[i];
                }
            }
            if (codeNamespace == null)
            {
                codeNamespace = new CodeNamespace(modelType.Namespace);
                compileUnit.Namespaces.Add(codeNamespace);
            }

            CodeTypeDeclaration? codeType = null;
            for (int i = 0; i < codeNamespace.Types.Count; i++)
            {
                if (codeNamespace.Types[i].Name == modelType.Name)
                {
                    codeType = codeNamespace.Types[i];
                }
            }
            if(codeType == null)
            {
                codeType = new CodeTypeDeclaration(modelType.Name);

                AddMembers(compileUnit, codeType, modelType);
                codeNamespace.Types.Add(codeType);
            }

            return compileUnit;
        }

        private CodeTypeDeclaration AddMembers(CodeCompileUnit compileUnit, CodeTypeDeclaration codeType, Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {

                var codeProperty = new CodeMemberProperty();
                codeProperty.Name = property.Name;
                if (!property.PropertyType.Namespace.Equals("System")
                    && !property.PropertyType.Namespace.StartsWith("System."))
                {
                    CreateModel(compileUnit, property.PropertyType);
                }

                codeProperty.Type = new CodeTypeReference(property.PropertyType);
                codeProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                var fieldName = "_" + property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);

                CodeMemberField field1 = new CodeMemberField(codeProperty.Type, fieldName);
                field1.Attributes = MemberAttributes.Private;

                codeType.Members.Add(field1);

                codeProperty.GetStatements.Add(new CodeMethodReturnStatement(
                    new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(), fieldName)));

                codeProperty.SetStatements.Add(new CodeAssignStatement(
                    new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName),
                    new CodePropertySetValueReferenceExpression()));

                codeType.Members.Add(codeProperty);
            }
            return codeType;
        }

        private CodeCompileUnit AddUsing(CodeCompileUnit compileUnit, string[] usingStatements)
        {
            var codeNamespace = compileUnit.Namespaces[0];
            foreach (var statement in usingStatements)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(statement));
            }
            return compileUnit;
        }

        private CodeCompileUnit AddField(CodeCompileUnit compileUnit, Type type, string name)
        {

            CodeMemberField field1 = new CodeMemberField(type, name);
            field1.Attributes = MemberAttributes.Private;
            compileUnit.Namespaces[0].Types[0].Members.Add(field1);
            return compileUnit;
        }

        private CodeCompileUnit AddProperty(CodeCompileUnit compileUnit, Type type, string name)
        {

            CodeMemberProperty property1 = new CodeMemberProperty();
            property1.Name = name;
            property1.Type = new CodeTypeReference(type);
            property1.Attributes = MemberAttributes.Public | MemberAttributes.Final;


            compileUnit.Namespaces[0].Types[0].Members.Add(property1);
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
            stringConstructor.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"_mqType = Bridge.MQType.{mqType.ToString()}")));

            compileUnit.Namespaces[0].Types[0].Members.Add(stringConstructor);
            return compileUnit;
        }

        private CodeCompileUnit AddMethod(CodeCompileUnit compileUnit, Type returnType, bool isMulticast, string name, string queueName, string actionName, (Type, string)[] parameters)
        {

            CodeMemberMethod method1 = new CodeMemberMethod();

            method1.Name = name;
            if (isMulticast)
            {
                CodeTypeReferenceExpression publisher = new CodeTypeReferenceExpression("await _publisher");
                CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                    publisher, "PublishMulticastAsync",
                    new CodeSnippetExpression("_mqType"),
                    new CodeSnippetExpression($"\"{queueName}\""),
                    new CodeSnippetExpression($"\"{actionName}\""));

                foreach (var parameter in parameters)
                {
                    cs1.Parameters.Add(new CodeSnippetExpression(parameter.Item2));
                }

                method1.ReturnType = new CodeTypeReference($"async Task");

                method1.Statements.Add(cs1);
            }
            else
            {
                if (!returnType.Equals(typeof(void)))
                {
                    CodeTypeReferenceExpression publisher = new CodeTypeReferenceExpression("await _publisher");
                    var methodName = "PublishAndWaitReplyAsync";
                    var typeParameters = new List<CodeTypeReference>();
                    if (parameters.Any())
                    {
                        typeParameters.Add(new CodeTypeReference(parameters.First().Item1));
                        typeParameters.Add(new CodeTypeReference(returnType));
                    }
                    else
                    {
                        typeParameters.Add(new CodeTypeReference(returnType));
                    }
                    CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(publisher, methodName, typeParameters.ToArray()),
                        new CodeSnippetExpression("_mqType"),
                        new CodeSnippetExpression($"\"{queueName}\""),
                        new CodeSnippetExpression($"\"{actionName}\""));

                    foreach (var parameter in parameters)
                    {
                        cs1.Parameters.Add(new CodeSnippetExpression(parameter.Item2));
                    }

                    method1.ReturnType = new CodeTypeReference("async Task", new CodeTypeReference(returnType));


                    method1.Statements.Add(new CodeMethodReturnStatement(cs1));

                }
                else
                {
                    CodeTypeReferenceExpression publisher = new CodeTypeReferenceExpression("await _publisher");
                    CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
                        publisher, "PublishAsync",
                        new CodeSnippetExpression("_mqType"),
                        new CodeSnippetExpression($"\"{queueName}\""),
                        new CodeSnippetExpression($"\"{actionName}\""));

                    foreach (var parameter in parameters)
                    {
                        cs1.Parameters.Add(new CodeSnippetExpression(parameter.Item2));
                    }

                    method1.ReturnType = new CodeTypeReference($"async Task");

                    method1.Statements.Add(cs1);
                }
            }

            method1.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            foreach (var parameter in parameters)
            {
                method1.Parameters.Add(new CodeParameterDeclarationExpression(parameter.Item1, parameter.Item2));
            }

            compileUnit.Namespaces[0].Types[0].Members.Add(method1);

            return compileUnit;
        }
    }
}
