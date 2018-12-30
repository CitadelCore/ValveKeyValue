﻿using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using ValveKeyValue.Test.Test_Data;

namespace ValveKeyValue.Test
{
    internal class ApiSurfaceTestCase
    {
        [Test]
        public void ApiSurfaceIsWellKnown()
        {
            var expected = TestDataHelper.ReadTextResource("apisurface.txt");
            var actual = GenerateApiSurface(typeof(KvObject).GetTypeInfo().Assembly);

            Assert.That(actual, Is.EqualTo(expected), "This may indicate a breaking change.");
        }

        private static string GenerateApiSurface(Assembly assembly)
        {
            var sb = new StringBuilder();

            var publicTypes = assembly.GetTypes()
                .Where(t => t.GetTypeInfo().IsPublic)
                .OrderBy(t => t.Namespace)
                .ThenBy(t => t.Name);

            foreach (var type in publicTypes)
            {
                GenerateTypeApiSurface(sb, type);
            }

            return sb.ToString();
        }

        private static void GenerateTypeApiSurface(StringBuilder sb, Type type)
        {
            var typeInfo = type.GetTypeInfo();

            sb.Append("public ");

            if (typeInfo.IsSealed)
            {
                sb.Append("sealed ");
            }

            if (typeInfo.IsClass)
            {
                sb.Append("class");
            }
            else if (typeInfo.IsInterface)
            {
                sb.Append("interface");
            }
            else if (typeInfo.IsEnum)
            {
                sb.Append("enum");
            }
            else
            {
                sb.Append("struct");
            }

            sb.Append(' ');
            sb.Append(GetTypeAsString(type));
            sb.Append("\n{\n");

            if (typeInfo.IsEnum)
            {
                var members = Enum.GetNames(type);
                foreach (var member in members)
                {
                    var rawValue = type.GetField(member, BindingFlags.Public | BindingFlags.Static).GetValue(null);
                    var convertedValue = Convert.ChangeType(rawValue, Enum.GetUnderlyingType(type));

                    sb.Append("    ");
                    sb.Append(member);
                    sb.Append(" = ");
                    sb.Append(convertedValue);
                    sb.Append(";\n");
                }

                sb.Append("\n");
            }

            var methods = type
                .GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderBy(t => t.Name);

            foreach (var method in methods)
            {
                if (method.IsPrivate || method.IsAssembly || method.IsFamilyAndAssembly)
                {
                    continue;
                }

                sb.Append("    ");

                sb.Append(method.IsPublic ? "public" : "protected");

                if (IsHidingMember(method)) sb.Append(" new");
                if (method.IsStatic) sb.Append(" static");

                sb.Append(' ');
                sb.Append(GetTypeAsString(method.ReturnType));
                sb.Append(' ');
                sb.Append(method.Name);

                if (method.IsGenericMethodDefinition)
                {
                    sb.Append('<');
                    sb.Append(string.Join(", ", method.GetGenericArguments().Select(GetTypeAsString)));
                    sb.Append('>');
                }

                sb.Append('(');
                sb.Append(string.Join(", ", method.GetParameters().Select(GetParameterAsString)));
                sb.Append(");\n");
            }

            sb.Append("}\n\n");
        }

        private static bool IsHidingMember(MethodInfo method)
        {
            var baseType = method.DeclaringType.GetTypeInfo().BaseType;
            if (baseType == null) return false;

            var baseMethod = baseType.GetMethod(method.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (baseMethod == null) return false;
            if (baseMethod.DeclaringType == method.DeclaringType) return false;

            var methodDefinition = method.GetBaseDefinition();
            var baseMethodDefinition = baseMethod.GetBaseDefinition();
            if (methodDefinition.DeclaringType == baseMethodDefinition.DeclaringType) return false;

            var methodParameters = method.GetParameters();
            var baseMethodParameters = baseMethod.GetParameters();
            if (methodParameters.Length != baseMethodParameters.Length) return false;

            return !methodParameters.Where((t, i) => t.ParameterType != baseMethodParameters[i].ParameterType).Any();
        }

        private static string GetTypeAsString(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var elementTypeAsString = GetTypeAsString(elementType);
                return string.Format(CultureInfo.InvariantCulture, "{0}[]", elementTypeAsString);
            }

            if (type == typeof(bool))
            {
                return "bool";
            }
            else if (type == typeof(byte))
            {
                return "byte";
            }
            else if (type == typeof(char))
            {
                return "char";
            }
            else if (type == typeof(decimal))
            {
                return "decimal";
            }
            else if (type == typeof(double))
            {
                return "double";
            }
            else if (type == typeof(float))
            {
                return "float";
            }
            else if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(long))
            {
                return "long";
            }
            else if (type == typeof(object))
            {
                return "object";
            }
            else if (type == typeof(sbyte))
            {
                return "sbyte";
            }
            else if (type == typeof(short))
            {
                return "short";
            }
            else if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(uint))
            {
                return "uint";
            }
            else if (type == typeof(ulong))
            {
                return "ulong";
            }
            else if (type == typeof(ushort))
            {
                return "ushort";
            }
            else if (type == typeof(void))
            {
                return "void";
            }

            var sb = new StringBuilder();

            if (type.Namespace != "System")
            {
                sb.Append(type.Namespace);
                sb.Append('.');
            }

            sb.Append(type.Name);

            if (type.IsConstructedGenericType)
            {
                sb.Append("[[");
                sb.Append(string.Join(", ", type.GetGenericArguments().Select(GetTypeAsString)));
                sb.Append("]]");
            }

            return sb.ToString();
        }

        private static string GetParameterAsString(ParameterInfo parameter)
        {
            var sb = new StringBuilder();

            if (parameter.IsOut)
            {
                sb.Append("out ");
            }
            else if (parameter.ParameterType.IsByRef)
            {
                sb.Append("ref ");
            }

            sb.Append(GetTypeAsString(parameter.ParameterType));
            sb.Append(' ');
            sb.Append(parameter.Name);

            return sb.ToString();
        }
    }
}
