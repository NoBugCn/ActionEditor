using System;
using System.Collections.Generic;
using System.Linq;

namespace NBC.ActionEditor
{
    public static class EditorCustomFactory
    {
        #region Header

        private static bool _initHeadersDic = false;
        private static readonly Dictionary<Type, Type> _headerDic = new Dictionary<Type, Type>();

        public static HeaderBase GetHeader(Asset asset)
        {
            InitHeaderDic();

            var type = asset.GetType();
            if (_headerDic.TryGetValue(type, out var t))
            {
                return Activator.CreateInstance(t) as HeaderBase;
            }

            return null;
        }

        public static void InitHeaderDic()
        {
            if (_initHeadersDic) return;

            _initHeadersDic = true;

            //先获取有绑定关系的所有对象和面板对象映射
            Type type = typeof(HeaderBase);
            var childs = EditorTools.GetTypeMetaDerivedFrom(type);
            foreach (var t in childs)
            {
                var arrs = t.type.GetCustomAttributes(typeof(CustomHeader), true);
                foreach (var arr in arrs)
                {
                    if (arr is CustomHeader c)
                    {
                        var bindT = c.InspectedType;
                        var iT = t.type;
                        if (!_headerDic.ContainsKey(bindT))
                        {
                            if (!iT.IsAbstract) _headerDic[bindT] = iT;
                        }
                        else
                        {
                            var old = _headerDic[bindT];
                            if (!iT.IsAbstract && iT.IsSubclassOf(old))
                            {
                                _headerDic[bindT] = iT;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Inspectors

        private static bool _initInspectorsDic = false;

        private static readonly Dictionary<Type, Type> _inspectorsDic = new Dictionary<Type, Type>();

        public static InspectorsBase GetInspector(IData directable)
        {
            InitInspectorDic();

            var type = directable.GetType();
            InspectorsBase b = null;
            // Debug.Log($"type={type}");
            if (_inspectorsDic.ContainsKey(type))
            {
                var t = _inspectorsDic[type];
                b = Activator.CreateInstance(t) as InspectorsBase;
            }

            if (b == null)
            {
                b = new InspectorsBase();
            }

            b.SetTarget(directable);
            return b;
        }

        public static void InitInspectorDic()
        {
            if (_initInspectorsDic) return;

            _initInspectorsDic = true;

            //先获取有绑定关系的所有对象和面板对象映射
            Type type = typeof(InspectorsBase);
            var childs = EditorTools.GetTypeMetaDerivedFrom(type);
            foreach (var t in childs)
            {
                var arrs = t.type.GetCustomAttributes(typeof(CustomInspectors), true);
                foreach (var arr in arrs)
                {
                    if (arr is CustomInspectors c)
                    {
                        var bindT = c.InspectedType;
                        var iT = t.type;
                        if (!_inspectorsDic.ContainsKey(bindT))
                        {
                            if (!iT.IsAbstract) _inspectorsDic[bindT] = iT;
                        }
                        else
                        {
                            var old = _inspectorsDic[bindT];
                            //如果不是抽象类，且是子类就更新
                            if (!iT.IsAbstract && iT.IsSubclassOf(old))
                            {
                                _inspectorsDic[bindT] = iT;
                            }
                        }
                    }
                }
            }

            //找出没有映射关系的对象，并绑定其最近父类的面板对象
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDirectable))))
                .ToArray();
            foreach (var t in types)
            {
                if (!t.IsAbstract)
                {
                    var iT = TryAdd(_inspectorsDic, t);
                    if (iT != null)
                    {
                        _inspectorsDic[t] = iT;
                    }
                }
            }
        }

        #endregion

        private static Type TryAdd(Dictionary<Type, Type> dictionary, Type type)
        {
            if (type != null && !dictionary.ContainsKey(type))
            {
                return TryAdd(dictionary, type.BaseType);
            }

            if (type != null && dictionary.ContainsKey(type))
            {
                return dictionary[type];
            }

            return null;
        }
    }
}