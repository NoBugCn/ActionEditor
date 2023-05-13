using UnityEditor;

namespace NBC.ActionEditor
{
    public static class Menus
    {
        [MenuItem("NBC/Action Editor/Open Action Editor", false, 0)]
        public static void OpenDirectorWindow()
        {
            ActionEditorWindow.ShowWindow();
        }
    }
}