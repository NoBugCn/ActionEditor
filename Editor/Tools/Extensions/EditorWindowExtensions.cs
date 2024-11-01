using System.Collections.Generic;
using UnityEditor;

namespace NBC.ActionEditor
{
    public static class EditorWindowExtensions
    {
        private static Dictionary<EditorWindow, List<ViewBase>> _views =
            new Dictionary<EditorWindow, List<ViewBase>>();

        private static void AddView(EditorWindow window, ViewBase view)
        {
            if (!_views.TryGetValue(window, out var list))
            {
                list = new List<ViewBase>();
                _views[window] = list;
            }

            if (!list.Contains(view))
                list.Add(view);
        }

        public static T CreateView<T>(this EditorWindow window) where T : ViewBase, new()
        {
            var cls = new T();
            cls.Init(window);
            AddView(window, cls);
            return cls;
        }

        public static void UpdateViews(this EditorWindow window)
        {
            if (_views.TryGetValue(window, out var list))
            {
                foreach (var view in list)
                {
                    if (!view.Visible) continue;
                    view.Update();
                }
            }
        }
    }
}