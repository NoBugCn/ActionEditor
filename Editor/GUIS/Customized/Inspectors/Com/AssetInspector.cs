using UnityEditor;

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
            EditorGUILayout.TextField("Name", action.Length.ToString());
        }
    }
}