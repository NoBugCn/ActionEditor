using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class PreferencesGUI : ICustomized
    {
        public virtual void OnGUI()
        {
            GUILayout.BeginVertical("box");

            GUI.color = new Color(0, 0, 0, 0.3f);

            GUILayout.BeginHorizontal(Styles.headerBoxStyle);
            GUI.color = Color.white;
            GUILayout.Label($"<size=22><b>{Lan.PreferencesTitle}</b></size>");
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUILayout.BeginVertical("box");
            var lan = EditorTools.CleanPopup<string>("Language", Lan.Language,
                Lan.AllLanguages.Keys.ToList());
            if (lan != Lan.Language)
            {
                Lan.SetLanguage(lan);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            Prefs.timeStepMode =
                (Prefs.TimeStepMode)EditorGUILayout.EnumPopup(Lan.PreferencesTimeStepMode, Prefs.timeStepMode);
            if (Prefs.timeStepMode == Prefs.TimeStepMode.Seconds)
            {
                Prefs.snapInterval = EditorTools.CleanPopup<float>(Lan.PreferencesSnapInterval, Prefs.snapInterval,
                    Prefs.snapIntervals.ToList());
            }
            else
            {
                Prefs.frameRate = EditorTools.CleanPopup<int>(Lan.PreferencesFrameRate, Prefs.frameRate,
                    Prefs.frameRates.ToList());
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            Prefs.magnetSnapping =
                EditorGUILayout.Toggle(new GUIContent(Lan.PreferencesMagnetSnapping, Lan.PreferencesMagnetSnappingTips),
                    Prefs.magnetSnapping);
            Prefs.scrollWheelZooms =
                EditorGUILayout.Toggle(
                    new GUIContent(Lan.PreferencesScrollWheelZooms, Lan.PreferencesScrollWheelZoomsTips),
                    Prefs.scrollWheelZooms);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            Prefs.savePath = EditorTools.GUILayoutGetFolderPath(Lan.PreferencesSavePath, Lan.PreferencesSavePathTips,
                Prefs.savePath, true);

            Prefs.autoSaveSeconds = EditorGUILayout.IntSlider(
                new GUIContent(Lan.PreferencesAutoSaveTime, Lan.PreferencesAutoSaveTimeTips), Prefs.autoSaveSeconds, 5,
                120);
            GUILayout.EndVertical();


            GUILayout.EndVertical();

            DrawHelpButton();
        }

        protected virtual void DrawHelpButton()
        {
            if (GUILayout.Button(Lan.PreferencesHelpDoc))
            {
                Application.OpenURL("https://nobug.cn/book/414447506088261");
            }
        }
    }
}