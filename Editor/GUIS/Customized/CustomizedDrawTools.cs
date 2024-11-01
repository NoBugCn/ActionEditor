using System;
using System.Collections.Generic;

namespace NBC.ActionEditor
{
    public class CustomizedDrawTools
    {
        #region 可定制GUI

        /// <summary>
        /// 类型映射
        /// </summary>
        private static readonly Dictionary<Type, Type> _customizedTypeDic = new Dictionary<Type, Type>();

        /// <summary>
        /// 类型实例映射
        /// </summary>
        private static readonly Dictionary<Type, ICustomized> _customizedInstDic = new Dictionary<Type, ICustomized>();

        public static void Draw<T>() where T : ICustomized
        {
            var type = typeof(T);
            if (_customizedInstDic.TryGetValue(type, out var customized) && customized != null)
            {
                customized.OnGUI();
                return;
            }

            var subType = GetSubclassType(type);
            if (subType == null) return;
            _customizedTypeDic[type] = subType;
            if (Activator.CreateInstance(subType) is ICustomized custom)
            {
                _customizedInstDic[type] = custom;
                custom.OnGUI();
            }
        }

        private static Type GetSubclassType(Type type)
        {
            if (_customizedTypeDic.TryGetValue(type, out var t))
            {
                return t;
            }

            EditorTools.GetTypeLastSubclass(type, _customizedTypeDic);
            if (_customizedTypeDic.TryGetValue(type, out t))
            {
                return t;
            }

            return null;
        }

        #endregion
    }
}