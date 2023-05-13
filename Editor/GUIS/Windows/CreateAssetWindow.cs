using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class CreateAssetWindow : PopupWindowContent
    {
        private static Rect myRect;
        private static string _selectType;
        private static string _createName;

        public static void Show()
        {
            var rect = new Rect(Styles.RIGHT_MARGIN, Styles.TOOLBAR_HEIGHT + 5, 400, 150);
            myRect = rect;
            PopupWindow.Show(new Rect(rect.x, rect.y, 0, 0), new CreateAssetWindow());
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(myRect.width, myRect.height);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginVertical("box");

            GUI.color = new Color(0, 0, 0, 0.3f);

            GUILayout.BeginHorizontal(Styles.headerBoxStyle);
            GUI.color = Color.white;
            GUILayout.Label($"<size=22><b>{Lan.CreateAsset}</b></size>");
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUILayout.BeginVertical("box");

            _selectType = EditorTools.CleanPopup(Lan.CrateAssetType, _selectType, Prefs.AssetNames);
            _createName = EditorGUILayout.TextField(new GUIContent(Lan.CrateAssetName, Lan.CreateAssetFileName),
                _createName);
            GUI.backgroundColor = new Color(1, 0.5f, 0.5f);
            if (GUILayout.Button(new GUIContent(Lan.CreateAssetConfirm)))
            {
                CreateConfirm();
            }

            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(new GUIContent(Lan.CreateAssetReset)))
            {
                _selectType = Prefs.AssetNames[0];
                _createName = string.Empty;
            }

            GUILayout.EndVertical();
        }

        void CreateConfirm()
        {
            var path = $"{Prefs.savePath}/{_createName}.asset";
            if (string.IsNullOrEmpty(_createName))
            {
                EditorUtility.DisplayDialog(Lan.TipsTitle, Lan.CreateAssetTipsNameNull, Lan.TipsConfirm);
            }
            else if (AssetDatabase.LoadAssetAtPath<Asset>(path) != null)
            {
                EditorUtility.DisplayDialog(Lan.TipsTitle, Lan.CreateAssetTipsRepetitive, Lan.TipsConfirm);
            }
            else
            {
                var t = Prefs.AssetTypes[_selectType];

                var asset = ScriptableObject.CreateInstance(t);
                if (asset != null)
                {
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}