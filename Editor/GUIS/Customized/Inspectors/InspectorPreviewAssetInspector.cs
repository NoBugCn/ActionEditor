using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    [CustomEditor(typeof(InspectorPreviewAsset))]
    public class InspectorPreviewAssetInspector : Editor
    {
        private bool _optionsAssetFold = true;

        private static Asset _lastAsset;
        private static bool _willResample;

        private static Dictionary<IData, InspectorsBase> directableEditors =
            new Dictionary<IData, InspectorsBase>();

        private static InspectorsBase _currentDirectableEditor;
        private static InspectorsBase _currentAssetEditor;


        void OnEnable()
        {
            _currentDirectableEditor = null;
            _willResample = false;
        }

        void OnDisable()
        {
            _currentDirectableEditor = null;
            directableEditors.Clear();
            _willResample = false;
        }

        protected override void OnHeaderGUI()
        {
            GUILayout.Space(18f);
        }

        public override void OnInspectorGUI()
        {
            var ow = target as InspectorPreviewAsset;
            if (ow == null || App.SelectCount < 1)
            {
                EditorGUILayout.HelpBox(Lan.NotSelectAsset, MessageType.Info);
                return;
            }

            GUI.skin.GetStyle("label").richText = true;

            GUILayout.Space(5);

            DoAssetInspector();
            DoSelectionInspector();


            if (_willResample)
            {
                _willResample = false;
                EditorApplication.delayCall += () => { Debug.Log("cutscene.ReSample();"); };
            }

            Repaint();
        }


        void DoAssetInspector()
        {
            if (App.AssetData == null) return;
            var assetData = App.AssetData;
            GUI.color = new Color(0, 0, 0, 0.2f);
            GUILayout.BeginHorizontal(Styles.HeaderBoxStyle);
            GUI.color = Color.white;
            var title = string.Format(Lan.InsBaseInfo, Prefs.GetAssetTypeName(assetData.GetType()));
            GUILayout.Label(
                $"<b><size=18>{(_optionsAssetFold ? "▼" : "▶")} {title}</size></b>");
            GUILayout.EndHorizontal();

            var lastRect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.Link);

            if (Event.current.type == EventType.MouseDown && lastRect.Contains(Event.current.mousePosition))
            {
                _optionsAssetFold = !_optionsAssetFold;
                Event.current.Use();
            }

            GUILayout.Space(2);
            if (_optionsAssetFold)
            {
                if (!directableEditors.TryGetValue(assetData, out var newEditor))
                {
                    directableEditors[assetData] = newEditor = EditorCustomFactory.GetInspector(assetData);
                }

                if (_currentAssetEditor != newEditor)
                {
                    _currentAssetEditor = newEditor;
                }

                if (_currentAssetEditor != null)
                {
                    _currentAssetEditor.OnInspectorGUI();
                }
            }
        }

        void DoSelectionInspector()
        {
            var data = App.FistSelect;

            if (data == null)
            {
                _currentDirectableEditor = null;
                return;
            }


            if (!directableEditors.TryGetValue(data, out var newEditor))
            {
                directableEditors[data] =
                    newEditor = EditorCustomFactory.GetInspector(data);
            }

            if (_currentDirectableEditor != newEditor)
            {
                var enableMethod = newEditor.GetType().GetMethod("OnEnable",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.FlattenHierarchy);
                if (enableMethod != null)
                {
                    enableMethod.Invoke(newEditor, null);
                }

                _currentDirectableEditor = newEditor;
            }

            EditorTools.BoldSeparator();
            GUILayout.Space(4);
            ShowPreliminaryInspector();

            if (_currentDirectableEditor != null) _currentDirectableEditor.OnInspectorGUI();
        }

        /// <summary>
        /// 选中对象基本信息
        /// </summary>
        void ShowPreliminaryInspector()
        {
            if (App.AssetData == null) return;
            var type = App.FistSelect.GetType();
            var nameAtt = type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
            var name = nameAtt != null ? nameAtt.name : type.Name.SplitCamelCase();

            GUI.color = new Color(0, 0, 0, 0.2f);
            GUILayout.BeginHorizontal(Styles.HeaderBoxStyle);
            GUI.color = Color.white;

            GUILayout.Label($"<b><size=18>{name}</size></b>");


            GUILayout.EndHorizontal();

            var desAtt =
                type.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            var description = desAtt != null ? desAtt.description : string.Empty;
            if (!string.IsNullOrEmpty(description))
            {
                EditorGUILayout.HelpBox(description, MessageType.None);
            }

            GUILayout.Space(2);
        }
    }
}