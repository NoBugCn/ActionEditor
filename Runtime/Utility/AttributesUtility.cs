using System;
using System.Collections.Generic;
using System.Reflection;

namespace NBC.ActionEditor
{
    public static class AttributesUtility
    {
        private static readonly Dictionary<Type, Dictionary<int, string>> CacheMenuName =
            new Dictionary<Type, Dictionary<int, string>>();

        public static string GetMenuName(int index, Type type)
        {
            // type.GetCustomAttributes();
            if (type != null)
            {
                if (CacheMenuName.TryGetValue(type, out var dictionary))
                {
                    if (dictionary.TryGetValue(index, out string name))
                    {
                        return name;
                    }
                }
                else
                {
                    CacheMenuName[type] = new Dictionary<int, string>();
                }

                FieldInfo[] fieldInfos = type.GetFields();
                foreach (var field in fieldInfos)
                {
                    var fieldType = field.FieldType;
                    var value = field.GetValue(type);
                    if (value is int i && i == index)
                    {
                        var attributes = field.GetCustomAttributes();
                        foreach (var attribute in attributes)
                        {
                            var t = attribute.GetType();
                            if (attribute is MenuNameAttribute menuNameAttribute)
                            {
                                CacheMenuName[type][index] = menuNameAttribute.showName;
                                return menuNameAttribute.showName;
                            }
                        }
                    }
                }
            }

            return index.ToString();
        }
    }
}