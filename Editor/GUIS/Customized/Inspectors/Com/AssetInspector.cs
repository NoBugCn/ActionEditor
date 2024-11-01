using NBC.ActionEditor;
using UnityEditor;
using UnityEditor.VersionControl;

namespace NBC.ActionEditor
{
    [CustomInspectors(typeof(Asset), true)]
    public class AssetInspector : InspectorsBase
    {
        private Asset action => (Asset)target;

        public override void OnInspectorGUI()
        {
            ShowCommonInspector();
            base.OnInspectorGUI();
        }

        protected void ShowCommonInspector(bool showBaseInspector = true)
        {
            // action.version = EditorGUILayout.TextField("Name", action.version);
        }
    }
}