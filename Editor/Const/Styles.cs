using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    [InitializeOnLoad]
    public static class Styles
    {
        public static Texture2D Logo;
        public static Texture2D stripes;
        public static Texture2D magnetIcon;
        public static Texture2D lockIcon;
        public static Texture2D hiddenIcon;

        public static Texture2D playIcon;
        public static Texture2D playForwardIcon;
        public static Texture2D playBackwardIcon;
        public static Texture2D stepForwardIcon;
        public static Texture2D stepBackwardIcon;
        public static Texture2D pauseIcon;
        public static Texture2D playLoopIcon;

        public static Texture2D carretIcon;
        public static Texture2D cutsceneIconOpen;
        public static Texture2D backIcon;
        public static Texture2D saveIcon;
        public static Texture2D settingsIcon;
        public static Texture2D plusIcon;
        public static Texture2D menuIcon;

        private static GUISkin styleSheet;


        public static Color HIGHLIGHT_COLOR => EditorGUIUtility.isProSkin ? new Color(0.65f, 0.65f, 1) : new Color(0.1f, 0.1f, 0.1f);
        
        public static readonly Color LIST_SELECTION_COLOR = new Color(0.5f, 0.5f, 1, 0.3f);
        public static readonly Color GROUP_COLOR = new Color(0f, 0f, 0f, 0.25f);
        
        /// <summary>
        /// 右边边距
        /// </summary>
        public const float RIGHT_MARGIN = 16;

        /// <summary>
        /// 工具栏的高度
        /// </summary>
        public const float TOOLBAR_HEIGHT = 20;

        /// <summary>
        /// 上边距
        /// </summary>
        public const float TOP_MARGIN = 20;

        /// <summary>
        /// 组高
        /// </summary>
        public const float GROUP_HEIGHT = 31;

        /// <summary>
        /// 组右边距
        /// </summary>
        public const float GROUP_RIGHT_MARGIN = 4;

        /// <summary>
        /// 第一个组上边距
        /// </summary>
        public const float FIRST_GROUP_TOP_MARGIN = 22;

        /// <summary>
        /// 轨道上下边距
        /// </summary>
        public const float TRACK_MARGINS = 4;

        /// <summary>
        /// 轨道右边距
        /// </summary>
        public const float TRACK_RIGHT_MARGIN = 4;

        /// <summary>
        /// 底部滚动条高度
        /// </summary>
        public const float BOTTOM_HEIGHT = 20;
        
        public static float LEFT_MARGIN
        {
            get => Prefs.trackListLeftMargin;
            set => Prefs.trackListLeftMargin = Mathf.Clamp(value, 230, 400);
        }
        
        static Styles()
        {
            Load();
        }

        [InitializeOnLoadMethod]
        public static void Load()
        {
            stripes = (Texture2D)Resources.Load("nbc/Stripes");
            magnetIcon = (Texture2D)Resources.Load("nbc/magnet");
            lockIcon = (Texture2D)Resources.Load("nbc/LockIcon");
            hiddenIcon = (Texture2D)Resources.Load("nbc/HiddenIcon");
            playIcon = (Texture2D)Resources.Load("nbc/play");
            playForwardIcon = (Texture2D)Resources.Load("nbc/playForward");
            playBackwardIcon = (Texture2D)Resources.Load("nbc/playBackward");
            playLoopIcon = (Texture2D)Resources.Load("nbc/loopPlay");
            stepForwardIcon = (Texture2D)Resources.Load("nbc/stepForward");
            stepBackwardIcon = (Texture2D)Resources.Load("nbc/stepBackward");
            pauseIcon = (Texture2D)Resources.Load("nbc/pause");
            carretIcon = (Texture2D)Resources.Load("nbc/CarretIcon");
            cutsceneIconOpen = (Texture2D)Resources.Load("nbc/CutsceneIconOpen");
            settingsIcon = (Texture2D)Resources.Load("nbc/settings");
            backIcon = (Texture2D)Resources.Load("nbc/back");
            saveIcon = (Texture2D)Resources.Load("nbc/save");
            plusIcon = (Texture2D)Resources.Load("nbc/plus");
            menuIcon = (Texture2D)Resources.Load("nbc/menu");

            Logo = (Texture2D)Resources.Load("nbc/Logo");

            styleSheet = (GUISkin)Resources.Load("nbc/StyleSheet");
        }

        public static Texture2D whiteTexture => EditorGUIUtility.whiteTexture;

        private static GUIStyle _shadowBorderStyle;

        public static GUIStyle shadowBorderStyle =>
            _shadowBorderStyle != null
                ? _shadowBorderStyle
                : _shadowBorderStyle = styleSheet.GetStyle("ShadowBorder");

        private static GUIStyle _clipBoxStyle;

        public static GUIStyle clipBoxStyle => _clipBoxStyle != null ? _clipBoxStyle : _clipBoxStyle = styleSheet.GetStyle("ClipBox");

        private static GUIStyle _clipBoxFooterStyle;


        private static GUIStyle _clipBoxHorizontalStyle;

        public static GUIStyle clipBoxHorizontalStyle =>
            _clipBoxHorizontalStyle != null
                ? _clipBoxHorizontalStyle
                : _clipBoxHorizontalStyle = styleSheet.GetStyle("ClipBoxHorizontal");

        private static GUIStyle _timeBoxStyle;

        public static GUIStyle timeBoxStyle => _timeBoxStyle != null ? _timeBoxStyle : _timeBoxStyle = styleSheet.GetStyle("TimeBox");

        private static GUIStyle _headerBoxStyle;

        public static GUIStyle headerBoxStyle => _headerBoxStyle != null ? _headerBoxStyle : _headerBoxStyle = styleSheet.GetStyle("HeaderBox");

        private static GUIStyle _hollowFrameStyle;

        public static GUIStyle hollowFrameStyle =>
            _hollowFrameStyle != null
                ? _hollowFrameStyle
                : _hollowFrameStyle = styleSheet.GetStyle("HollowFrame");

        private static GUIStyle _hollowFrameHorizontalStyle;

        public static GUIStyle hollowFrameHorizontalStyle =>
            _hollowFrameHorizontalStyle != null
                ? _hollowFrameHorizontalStyle
                : _hollowFrameHorizontalStyle = styleSheet.GetStyle("HollowFrameHorizontal");


        private static GUIStyle _centerLabel;

        public static GUIStyle centerLabel
        {
            get
            {
                if (_centerLabel != null)
                {
                    return _centerLabel;
                }

                _centerLabel = new GUIStyle("label");
                _centerLabel.alignment = TextAnchor.MiddleCenter;
                return _centerLabel;
            }
        }
        
        
        
    }
}