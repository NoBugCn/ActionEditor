using UnityEngine;

namespace NBC.ActionEditor
{
    public class WelcomeView : ViewBase
    {
        public override void OnDraw()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            if (Styles.Logo != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(Styles.Logo, GUILayout.Width(128), GUILayout.Height(128)); // 设置图片尺寸
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 30;
            titleStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("行为时间轴编辑器", titleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUI.backgroundColor = new Color(0.8f, 0.8f, 1, 1f);

            if (DrawCenteredButton(Lan.CreateAsset))
            {
                CreateAssetWindow.Show();
            }

            GUI.backgroundColor = Color.white;
            if (DrawCenteredButton(Lan.SelectAsset))
            {
                // ObjectSelectorWindow.ShowObjectPicker<Asset>(null, App.OnObjectPickerConfig, "Assets/");
                ObjectSelectorWindow.ShowObjectPicker<TextAsset>(null, App.OnObjectPickerConfig, "Assets/");
            }

            if (DrawCenteredButton(Lan.Seeting))
            {
                PreferencesWindow.Show(new Rect(Styles.ScreenWidth - 5 - 400, 25, 400,
                    Styles.ScreenHeight - 70));
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private bool DrawCenteredButton(string buttonText)
        {
            var ret = false;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonText, GUILayout.Width(220), GUILayout.Height(40)))
            {
                ret = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return ret;
        }
    }
}