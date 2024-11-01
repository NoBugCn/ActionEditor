using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class HeaderGUI : ICustomized
    {
        public void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            DrawNowAssetName();
            DrawAssetsHeader();
            GUILayout.FlexibleSpace();

            DrawToolbarRight();

            GUILayout.EndHorizontal();
        }

        protected virtual void DrawAssetsHeader()
        {
            if (App.AssetData == null) return;
            var customAssetHeader = EditorCustomFactory.GetHeader(App.AssetData);
            customAssetHeader?.OnGUI();
        }

        protected virtual void DrawNowAssetName()
        {
            var gName = App.TextAsset != null ? App.TextAsset.name : "";
            var size = GUI.skin.label.CalcSize(new GUIContent(gName));
            var width = size.x + 8;
            if (width < 80) width = 80;
            if (GUILayout.Button($"[{gName}]", EditorStyles.toolbarDropDown, GUILayout.Width(width)))
            {
                App.AutoSave();
                ObjectSelectorWindow.ShowObjectPicker<TextAsset>(null, App.OnObjectPickerConfig, "Assets/");
            }
        }


        protected virtual void DrawToolbarRight()
        {
            //显示保持状态
            GUI.color = Color.white.WithAlpha(0.3f);
            GUI.skin.label.richText = true;
            GUILayout.Label(
                $"<size=11>{string.Format(Lan.HeaderLastSaveTime, App.LastSaveTime.ToString("HH:mm:ss"))}</size>");
            GUI.color = Color.white;

            if (GUILayout.Button(new GUIContent(Styles.SaveIcon, Lan.Save), EditorStyles.toolbarButton,
                    GUILayout.Width(26)))
            {
                App.AutoSave(); //先保存当前的
            }

            Prefs.MagnetSnapping = GUILayout.Toggle(Prefs.MagnetSnapping,
                new GUIContent(Styles.MagnetIcon, Lan.OpenMagnetSnappingTips),
                EditorStyles.toolbarButton);

            if (GUILayout.Button(new GUIContent(Styles.SettingsIcon, Lan.OpenPreferencesTips),
                    EditorStyles.toolbarButton, GUILayout.Width(26)))
            {
                PreferencesWindow.Show(new Rect(Styles.ScreenWidth - 5 - 400 - Styles.TimelineLeftTotalWidth, 25, 400,
                    Styles.ScreenHeight - 70));
                
                // PreferencesWindow.Show(new Rect(G.ScreenWidth - 5 - 400, Styles.TOOLBAR_HEIGHT + 5, 400,
                //     G.ScreenHeight - Styles.TOOLBAR_HEIGHT - 50));
            }
        }
    }
}