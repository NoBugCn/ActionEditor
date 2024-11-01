using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class TimelineHeaderView : ViewBase
    {
        private GUIStyle _customToolbarButtonStyle;

        public override void OnDraw()
        {
            DrawPlayControl();
            DrawPlayHeader();
        }

        #region Play control

        private float _buttonWidth = 30;

        private void DrawPlayControl()
        {
            if (_customToolbarButtonStyle == null)
            {
                _customToolbarButtonStyle = new GUIStyle(EditorStyles.toolbarButton)
                {
                    fixedHeight = Styles.PlayControlHeight
                };
            }

            var rect = new Rect(0, 0, Styles.TimelineLeftWidth, Styles.PlayControlHeight);
            GUILayout.BeginArea(rect);

            _buttonWidth = rect.width / 7;

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (DrawButton(Styles.BackIcon, Lan.BackMenuTips))
            {
                App.TextAsset = null;
                // GUILayout.EndHorizontal();
                // return;
            }

            DrawButton(Styles.FirstFrameIcon, Lan.FirstFrame);
            DrawButton(Styles.PrevFrameIcon, Lan.StepBackwardTips);

            EditorGUI.BeginChangeCheck();
            var isPlaying = DrawToggle(App.IsPlay, Styles.PlayIcon, App.IsPlay ? Lan.StopTips : Lan.PlayTips);
            if (EditorGUI.EndChangeCheck())
            {
                App.IsPlay = isPlaying;
                if (isPlaying)
                {
                    // App.Play();
                }
                else
                {
                    // App.Stop(true);
                }
            }

            DrawButton(Styles.NextFrameIcon, Lan.StepBackwardTips);
            DrawButton(Styles.LastFrameIcon, Lan.StepBackwardTips);
            App.IsRange = DrawToggle(App.IsRange, Styles.RangeIcon, Lan.StepBackwardTips);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private bool DrawButton(Texture image, string tooltip)
        {
            return GUILayout.Button(new GUIContent(image, tooltip),
                _customToolbarButtonStyle, GUILayout.Width(_buttonWidth));
        }

        private bool DrawToggle(bool value, Texture image, string tooltip)
        {
            return GUILayout.Toggle(value, new GUIContent(image, tooltip), _customToolbarButtonStyle,
                GUILayout.Width(_buttonWidth));
        }

        #endregion

        #region Header

        private Rect _headerRect;

        private void DrawPlayHeader()
        {
            var gap = Styles.TimelineLeftWidth + Styles.SplitterWidth;
            _headerRect = new Rect(Position.x + gap, Position.y,
                Position.width - gap,
                Position.height - Styles.HeaderHeight);

            GUILayout.BeginArea(_headerRect);
            CustomizedDrawTools.Draw<HeaderGUI>();
            GUILayout.EndArea();


            GUI.color = Color.black.WithAlpha(0.2f);
            GUI.DrawTexture(new Rect(_headerRect.x, _headerRect.y + _headerRect.height, _headerRect.width, 1),
                Styles.WhiteTexture);
        }

        #endregion
    }
}