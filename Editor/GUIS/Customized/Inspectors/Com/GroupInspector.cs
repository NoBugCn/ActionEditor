using NBC.ActionEditor;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor.Design.Editor.Inspectors
{
    public abstract class GroupInspector<T> : ActionClipInspector where T : Group
    {
        protected T action => (T)target;
    }

    [CustomInspectors(typeof(Group), true)]
    public class GroupInspector : InspectorsBase
    {
        private Group action => (Group)target;

        public override void OnInspectorGUI()
        {
            ShowCommonInspector();
        }

        protected void ShowCommonInspector(bool showBaseInspector = true)
        {
            action.Name = EditorGUILayout.TextField("Name", action.Name);
            if (showBaseInspector)
            {
                base.OnInspectorGUI();
            }
        }
    }
}