using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace ValveKeyValue
{
    // TODO: Migrate to IVisitationListener
    internal static class ObjectCopier
    {
        public static TObject MakeObject<TObject>(KvObject keyValueObject)
            => MakeObject<TObject>(keyValueObject, new DefaultObjectReflector());

        public static object MakeObject(Type objectType, KvObject keyValueObject, IObjectReflector reflector)
            => InvokeGeneric(nameof(MakeObject), objectType, new object[] { keyValueObject, reflector });

        public static TObject MakeObject<TObject>(KvObject keyValueObject, IObjectReflector reflector)
        {
            Require.NotNull(keyValueObject, nameof(keyValueObject));
            Require.NotNull(reflector, nameof(reflector));

            if (keyValueObject.Value.ValueType == KvValueType.Collection)
            {
                if (IsLookupWithStringKey(typeof(TObject), out var lookupValueType))
                {
                    return (TObject)MakeLookup(lookupValueType, keyValueObject);
                }
                else if (IsDictionary(typeof(TObject)))
                {
                    return (TObject)MakeDictionary(typeof(TObject), keyValueObject);
                }
                else if (IsArray(keyValueObject, out var enumerableValues) && ConstructTypedEnumerable(typeof(TObject), enumerableValues, out var enumerable))
                {
                    return (TObject)enumerable;
                }
                else if (IsConstructibleEnumerableType(typeof(TObject)))
                {
                    throw new InvalidOperationException($"Cannot deserialize a non-array value to type \"{typeof(TObject).Namespace}.{typeof(TObject).Name}\".");
                }

                var typedObject = (TObject)FormatterServices.GetUninitializedObject(typeof(TObject));

                CopyObject(keyValueObject, typedObject, reflector);
                return typedObject;
            }
            else if (CanConvertValueTo(typeof(TObject)))
            {
                return (TObject)Convert.ChangeType(keyValueObject.Value, typeof(TObject));
            }
            else
            {
                throw new NotSupportedException(typeof(TObject).Name);
            }
        }

        public static object MakeObjectCollection(Type type, KvCollectionValue keyValueCollection)
            => MakeObjectCollection(type, keyValueCollection, new DefaultObjectReflector());

        public static object MakeObjectCollection(Type type, KvCollectionValue keyValueCollection, IObjectReflector reflector)
        {
            Require.NotNull(keyValueCollection, nameof(keyValueCollection));
            Require.NotNull(reflector, nameof(reflector));

            // Trying to cast to an IDictionary, so we add the values of the KvCollectionValue to the type
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                var dict = (IDictionary) FormatterServices.GetUninitializedObject(type);
                foreach (var obj in keyValueCollection)
                    dict.Add(obj.Name, obj.Value.ToType(dict.GetType().GetGenericArguments()[1], CultureInfo.CurrentCulture));
                return dict;
            }

            var typedObject = FormatterServices.GetUninitializedObject(type);
            CopyObjectCollection(keyValueCollection, typedObject, reflector);
            return typedObject;
        }

        public static KvObject FromObject<TObject>(TObject managedObject, string topLevelName)
            => FromObjectCore(managedObject, topLevelName, new DefaultObjectReflector(), new HashSet<object>());

        private static KvObject FromObjectCore<TObject>(TObject managedObject, string topLevelName, IObjectReflector reflector, HashSet<object> visitedObjects)
        {
            if (managedObject == null)
                throw new ArgumentNullException(nameof(managedObject));

            Require.NotNull(topLevelName, nameof(topLevelName));
            Require.NotNull(reflector, nameof(reflector));
            Require.NotNull(visitedObjects, nameof(visitedObjects));

            if (!typeof(TObject).IsValueType && typeof(TObject) != typeof(string) && !visitedObjects.Add(managedObject))
                throw new KeyValueException("Serialization failed - circular object reference detected.");

            if (typeof(IConvertible).IsAssignableFrom(typeof(TObject)))
                return new KvObject(topLevelName, (string)Convert.ChangeType(managedObject, typeof(string)));

            var childObjects = new List<KvObject>();

            if (typeof(IDictionary).IsAssignableFrom(typeof(TObject)))
            {
                var dictionary = (IDictionary)managedObject;
                var enumerator = dictionary.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var entry = enumerator.Entry;
                    childObjects.Add(new KvObject(entry.Key.ToString(), entry.Value.ToString()));
                }
            }
            else if (typeof(TObject).IsArray || typeof(IEnumerable).IsAssignableFrom(typeof(TObject)))
            {
                var counter = 0;
                foreach (var child in (IEnumerable)managedObject)
                {
                    var childKvObject = CopyObject(child, counter.ToString(), reflector, visitedObjects);
                    childObjects.Add(childKvObject);

                    counter++;
                }
            }
            else
            {
                foreach (var member in reflector.GetMembers(managedObject).OrderBy(p => p.Name))
                {
                    if (!member.MemberType.IsValueType && member.Value is null) continue;

                    var name = member.Name;
                    if (!member.IsExplicitName && name.Length > 0 && char.IsUpper(name[0]))
                    {
                        name = char.ToLower(name[0]) + name.Substring(1);
                    }

                    childObjects.Add(typeof(IConvertible).IsAssignableFrom(member.MemberType)
                        ? new KvObject(name, (string) Convert.ChangeType(member.Value, typeof(string)))
                        : CopyObject(member.Value, member.Name, reflector, visitedObjects));
                }
            }

            return new KvObject(topLevelName, childObjects);
        }

        private static KvObject CopyObject(object @object, string name, IObjectReflector reflector, HashSet<object> visitedObjects)
        {
            try
            {
                var keyValueRepresentation = (KvObject)typeof(ObjectCopier)
                    .GetMethod(nameof(FromObjectCore), BindingFlags.NonPublic | BindingFlags.Static)
                    ?.MakeGenericMethod(@object.GetType())
                    .Invoke(null, new[] { @object, name, reflector, visitedObjects });

                return keyValueRepresentation;
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return default(KvObject); // Unreachable.
            }
        }

        private static void CopyObject<TObject>(KvObject kv, TObject obj, IObjectReflector reflector)
        {
            Require.NotNull(kv, nameof(kv));

            // Cannot use Require.NotNull here because TObject might be a struct.
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Require.NotNull(reflector, nameof(reflector));

            var members = reflector.GetMembers(obj).ToDictionary(m => m.Name, m => m, StringComparer.OrdinalIgnoreCase);
            foreach (var item in kv.Children)
            {
                if (!members.TryGetValue(item.Name, out var member))
                    continue;
                member.Value = MakeObject(member.MemberType, item, reflector);
            }
        }

        public static void CopyObjectCollection(KvCollectionValue kv, object obj, IObjectReflector reflector)
        {
            Require.NotNull(kv, nameof(kv));

            // Cannot use Require.NotNull here because TObject might be a struct.
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Require.NotNull(reflector, nameof(reflector));
            var members = reflector.GetMembers(obj).ToDictionary(m => m.Name, m => m, StringComparer.OrdinalIgnoreCase);
            foreach (var item in kv)
            {
                if (!members.TryGetValue(item.Name, out var member))
                    continue;
                member.Value = MakeObject(member.MemberType, item, reflector);
            }
        }

        private static bool IsArray(KvObject obj, out KvValue[] values)
        {
            values = null;

            if (obj.Children.Any(i => !IsNumeric(i.Name)))
                return false;

            var items = obj.Children
                .Select(i => new { Index = int.Parse(i.Name), i.Value })
                .OrderBy(i => i.Index)
                .ToArray();

            for (var i = 0; i < items.Length; i++)
            {
                if (i != items[i].Index)
                    return false;
            }

            values = items.Select(i => i.Value).ToArray();
            return true;
        }

        private static bool IsLookupWithStringKey(Type type, out Type valueType)
        {
            valueType = null;

            if (!type.IsConstructedGenericType)
                return false;

            var genericType = type.GetGenericTypeDefinition();
            if (genericType != typeof(ILookup<,>)) return false;

            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length != 2) return false;
            if (genericArguments[0] != typeof(string)) return false;

            valueType = genericArguments[1];
            return true;
        }

        private static object MakeLookup(Type valueType, IEnumerable<KvObject> items)
            => InvokeGeneric(nameof(MakeLookupCore), valueType, items);

        private static ILookup<string, TValue> MakeLookupCore<TValue>(IEnumerable<KvObject> items)
            => items.ToLookup(kv => kv.Name, kv => (TValue)Convert.ChangeType(kv.Value, typeof(TValue)));

        private static readonly Dictionary<Type, Func<Type, object[], object>> EnumerableBuilders = new Dictionary<Type, Func<Type, object[], object>>
        {
            [typeof(List<>)] = (type, values) => InvokeGeneric(nameof(MakeList), type.GetGenericArguments()[0], new object[] { values }),
            [typeof(IList<>)] = (type, values) => InvokeGeneric(nameof(MakeList), type.GetGenericArguments()[0], new object[] { values }),
            [typeof(Collection<>)] = (type, values) => InvokeGeneric(nameof(MakeCollection), type.GetGenericArguments()[0], new object[] { values }),
            [typeof(ICollection<>)] = (type, values) => InvokeGeneric(nameof(MakeCollection), type.GetGenericArguments()[0], new object[] { values }),
            [typeof(ObservableCollection<>)] = (type, values) => InvokeGeneric(nameof(MakeObservableCollection), type.GetGenericArguments()[0], new object[] { values }),
        };

        private static bool ConstructTypedEnumerable(Type type, object[] values, out object typedEnumerable)
        {
            object listObject = null;

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var itemArray = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), values.Length);

                for (var i = 0; i < itemArray.Length; i++)
                {
                    itemArray.SetValue(Convert.ChangeType(values[i], elementType), i);
                }

                listObject = itemArray;
            }
            else if (type.IsConstructedGenericType)
            {
                if (EnumerableBuilders.TryGetValue(type.GetGenericTypeDefinition(), out var builder))
                    listObject = builder(type, values);
            }

            typedEnumerable = listObject;
            return listObject != null;
        }

        private static bool IsConstructibleEnumerableType(Type type)
        {
            if (type.IsArray) return true;
            if (!type.IsConstructedGenericType) return false;

            var gtd = type.GetGenericTypeDefinition();
            if (EnumerableBuilders.ContainsKey(gtd)) return true;

            return false;
        }

        private static object InvokeGeneric(string methodName, Type genericType, params object[] parameters)
        {
            var method = typeof(ObjectCopier)
                .GetTypeInfo()
                .GetDeclaredMethods(methodName)
                .Single(m => m.IsStatic && m.GetParameters().Length == parameters.Length);

            try
            {
                return method.MakeGenericMethod(genericType).Invoke(null, parameters);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // Unreachable
            }
        }

        private static List<TElement> MakeList<TElement>(object[] items)
        {
            return items.Select(i => Convert.ChangeType(i, typeof(TElement)))
                .Cast<TElement>()
                .ToList();
        }

        private static Collection<TElement> MakeCollection<TElement>(object[] items)
        {
            return new Collection<TElement>(MakeList<TElement>(items));
        }

        private static ObservableCollection<TElement> MakeObservableCollection<TElement>(object[] items)
        {
            return new ObservableCollection<TElement>(MakeList<TElement>(items));
        }

        private static bool IsNumeric(string str)
        {
            return str.Length != 0 && int.TryParse(str, out var unused);
        }

        private static bool IsDictionary(Type type)
        {
            if (!type.IsConstructedGenericType)
                return false;

            var genericType = type.GetGenericTypeDefinition();
            if (genericType != typeof(Dictionary<,>))
                return false;

            return true;
        }

        private static object MakeDictionary(Type type, KvObject kv)
        {
            var dictionary = Activator.CreateInstance(type);
            var genericArguments = type.GetGenericArguments();

            typeof(ObjectCopier)
                .GetMethod(nameof(FillDictionary), BindingFlags.Static | BindingFlags.NonPublic)
                ?.MakeGenericMethod(genericArguments)
                .Invoke(null, new[] { dictionary, kv });

            return dictionary;
        }

        private static void FillDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary, KvObject kv)
        {
            foreach (var item in kv.Children)
            {
                var key = (TKey)Convert.ChangeType(item.Name, typeof(TKey));
                var value = (TValue)Convert.ChangeType(item.Value, typeof(TValue));

                dictionary[key] = value;
            }
        }

        private static bool CanConvertValueTo(Type type)
        {
            return
                type == typeof(bool) ||
                type == typeof(byte) ||
                type == typeof(char) ||
                type == typeof(DateTime) ||
                type == typeof(decimal) ||
                type == typeof(double) ||
                type == typeof(float) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(uint) ||
                type == typeof(ulong) ||
                type == typeof(ushort) ||
                type == typeof(sbyte) ||
                type == typeof(short) ||
                type == typeof(string);
        }
    }
}
