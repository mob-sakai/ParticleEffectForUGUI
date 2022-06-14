using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Coffee.NanoMonitor
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NumericProperty))]
    internal sealed class NumericPropertyDrawer : PropertyDrawer
    {
        private static Action<PropertyInfo> s_OnMenuSelected;
        private static GenericMenu s_PropertyMenu;

        private static readonly Dictionary<Type, string> s_SupportedTypes = new Dictionary<Type, string>
        {
            {typeof(bool), "bool"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(int), "int"},
            {typeof(long), "long"},
            {typeof(byte), "byte"},
            {typeof(ushort), "ushort"},
            {typeof(uint), "uint"},
            {typeof(ulong), "ulong"},
            {typeof(float), "float"},
            {typeof(double), "double"},
            {typeof(decimal), "decimal"},
        };

        private static void Init()
        {
            if (s_PropertyMenu != null) return;

            var properties = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .SelectMany(type => type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty))
                .Where(pi => pi.GetMethod != null && s_SupportedTypes.ContainsKey(pi.PropertyType))
                .OrderBy(pi => ConvertToMenuItem(pi, false))
                .ToArray();

            s_PropertyMenu = new GenericMenu();
            s_PropertyMenu.AddItem(new GUIContent("No Property"), false, arg => s_OnMenuSelected?.Invoke(arg as PropertyInfo), null);
            s_PropertyMenu.AddItem(new GUIContent("(Non Public Properties)/"), false, ()=>{});
            s_PropertyMenu.AddSeparator("");

            foreach (var pi in properties)
            {
                s_PropertyMenu.AddItem(new GUIContent(ConvertToMenuItem(pi, true)), false, arg => s_OnMenuSelected?.Invoke(arg as PropertyInfo), pi);
            }
        }

        private static string ConvertToMenuItem(PropertyInfo p, bool propertyType)
        {
            var type = p.DeclaringType;
            if (type == null) return "";
            var category = p.GetMethod.IsPublic && type.IsPublic ? "" : "(Non Public Properties)/";
            var typeName = type.FullName;
            var asmName = type.Assembly.GetName().Name;
            if (asmName == "UnityEngine.CoreModule")
                asmName = "UnityEngine";
            return propertyType
                ? $"{category}{asmName}/{typeName}/{s_SupportedTypes[p.PropertyType]} {p.Name}"
                : $"{category}{asmName}/{typeName}/{p.Name}";
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init();
            label = EditorGUI.BeginProperty(position, label, property);

            var path = property.FindPropertyRelative("m_Path");
            var split = path.stringValue.Split(';', ' ', ',');
            var name = split.Length == 4 ? $"{split[0]}.{split[3]}" : "No Property";

            position = EditorGUI.PrefixLabel(position, label);
            if (GUI.Button(position, name, EditorStyles.popup))
            {
                s_OnMenuSelected = p =>
                {
                    path.stringValue = p == null ? "" : $"{p.DeclaringType?.FullName}, {p.DeclaringType?.Assembly.GetName().Name};{p.Name}";
                    property.serializedObject.ApplyModifiedProperties();
                };
                s_PropertyMenu.DropDown(position);
            }

            EditorGUI.EndProperty();
        }
    }
#endif

    [Serializable]
    public class NumericProperty : ISerializationCallbackReceiver
    {
        //################################
        // Serialized Members.
        //################################
        [SerializeField] private string m_Path = "";


        //################################
        // Public Members.
        //################################
        public double Get()
        {
            return _get?.Invoke() ?? -1;
        }


        //################################
        // Private Members.
        //################################
        private Func<double> _get = null;

        private static PropertyInfo GetPropertyInfo(string path)
        {
            var p = path.Split(';');
            if (p.Length != 2) return null;

            var type = Type.GetType(p[0]);
            if (type == null)
            {
                UnityEngine.Debug.LogException(new Exception($"Type '{p[0]}' is not found"));
                return null;
            }

            var pInfo = type.GetProperty(p[1], BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static);
            if (pInfo == null)
            {
                UnityEngine.Debug.LogException(new Exception($"Member '{p[1]}' is not found in type '{type}'"));
            }

            return pInfo;
        }

        private static Func<double> CreateFunc(MethodInfo mInfo)
        {
            if (mInfo == null) return null;
            switch (Type.GetTypeCode(mInfo.ReturnType))
            {
                case TypeCode.Boolean:
                    var f_bool = (Func<bool>) mInfo.CreateDelegate(typeof(Func<bool>));
                    return () => f_bool() ? 1 : 0;
                case TypeCode.Byte:
                    var f_byte = (Func<byte>) mInfo.CreateDelegate(typeof(Func<byte>));
                    return () => f_byte();
                case TypeCode.SByte:
                    var f_sbyte = (Func<sbyte>) mInfo.CreateDelegate(typeof(Func<sbyte>));
                    return () => f_sbyte();
                case TypeCode.UInt16:
                    var f_ushort = (Func<ushort>) mInfo.CreateDelegate(typeof(Func<ushort>));
                    return () => f_ushort();
                case TypeCode.UInt32:
                    var f_uint = (Func<uint>) mInfo.CreateDelegate(typeof(Func<uint>));
                    return () => f_uint();
                case TypeCode.UInt64:
                    var f_ulong = (Func<ulong>) mInfo.CreateDelegate(typeof(Func<ulong>));
                    return () => f_ulong();
                case TypeCode.Int16:
                    var f_short = (Func<short>) mInfo.CreateDelegate(typeof(Func<short>));
                    return () => f_short();
                case TypeCode.Int32:
                    var f_int = (Func<int>) mInfo.CreateDelegate(typeof(Func<int>));
                    return () => f_int();
                case TypeCode.Int64:
                    var f_long = (Func<long>) mInfo.CreateDelegate(typeof(Func<long>));
                    return () => f_long();
                case TypeCode.Decimal:
                    var f_decimal = (Func<decimal>) mInfo.CreateDelegate(typeof(Func<decimal>));
                    return () => (double) f_decimal();
                case TypeCode.Double:
                    var f_double = (Func<double>) mInfo.CreateDelegate(typeof(Func<double>));
                    return f_double;
                case TypeCode.Single:
                    var f_float = (Func<float>) mInfo.CreateDelegate(typeof(Func<float>));
                    return () => f_float();
                default:
                    UnityEngine.Debug.LogException(new Exception(string.Format("Method '{1}.{0}' is not supported.", mInfo.Name, mInfo.DeclaringType)));
                    return null;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            var pInfo = GetPropertyInfo(m_Path);
            _get = CreateFunc(pInfo?.GetMethod);
        }
    }
}
