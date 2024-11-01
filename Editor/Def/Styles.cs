using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    [InitializeOnLoad]
    public static class Styles
    {
        private static GUISkin _styleSheet;
        private static GUIStyle _headerBoxStyle;
        private static float _timelineLeftWidth;
        private static float _timelineRightWidth;

        public static Texture2D Logo;
        public static Texture2D WhiteTexture => EditorGUIUtility.whiteTexture;
        public static Texture2D BackgroundTexture;

        public static Texture2D FirstFrameIcon;
        public static Texture2D LastFrameIcon;
        public static Texture2D NextFrameIcon;
        public static Texture2D PlayIcon;
        public static Texture2D PrevFrameIcon;
        public static Texture2D RangeIcon;
        public static Texture2D PauseIcon;
        public static Texture2D StopIcon;

        public static Texture2D EyeIcon;
        public static Texture2D LockIcon;
        public static Texture2D CreateIcon;
        public static Texture2D MenuIcon;
        public static Texture2D CollapsedIcon;
        public static Texture2D ExpandedIcon;

        public static Texture2D BackIcon;
        public static Texture2D SaveIcon;
        public static Texture2D SettingsIcon;
        public static Texture2D MagnetIcon;

        public static Texture2D TimelineTimeCursorIcon;
        public static Texture2D TimelineStartPlaybackIcon;
        public static Texture2D TimelineEndPlaybackIcon;
        public static Texture2D SignalIcon;
        public static Texture2D Stripes;

        public static Color EndPointerColor = new Color(57 / 255f, 122 / 255f, 234 / 255f, 1);
        public static Color TimeStepRectColor = new Color(57 / 255f, 122 / 255f, 234 / 255f, 50 / 255f);

        public static Color ClipBackColor = new Color(0.3f, 0.3f, 0.3f);

        public static Color ClipBlendColor = new Color(144 / 255f, 144 / 255f, 144 / 255f, 0.5f);

        public static Color ClipSelectColor = Color.white;

        public static GUIStyle HeaderBoxStyle => _headerBoxStyle != null
            ? _headerBoxStyle
            : _headerBoxStyle = _styleSheet.GetStyle("HeaderBox");


        public static GUIStyle WhiteBox => _styleSheet.GetStyle("WhiteBox");
        public static GUIStyle NotScrollbar => _styleSheet.GetStyle("NotScrollbar");


        // public static GUIStyle NotVerticalScrollbar { get; private set; }
        // public static GUIStyle NotHorizontalScrollbar { get; private set; }
        public const int SplitterWidth = 5;
        public const int RightGapWidth = 4;
        public const int HeaderHeight = 20;
        public const int PlayControlHeight = 40;
        public const int Space = 2;
        public const int LineHeight = 26;
        public const int ClipBottomRectHeight = 4;
        public const int ClipScaleRectWidth = 5;

        public const int BottomHeight = 16;


        public static float TimelineLeftWidth
        {
            get => _timelineLeftWidth;
            set
            {
                _timelineLeftWidth = value;
                EditorPrefs.SetFloat(PrefsConst.Width, value);
            }
        }

        public static float TimelineLeftTotalWidth => TimelineLeftWidth + SplitterWidth + RightGapWidth;

        public static float TimelineRightWidth
        {
            get => _timelineRightWidth;
            set => _timelineRightWidth = value;
        }

        public static Vector2 TimelineScrollPos;


        public static float ScreenWidth => Screen.width / EditorGUIUtility.pixelsPerPoint;

        public static float ScreenHeight => Screen.height / EditorGUIUtility.pixelsPerPoint;


        static Styles()
        {
            Load();
        }

        [InitializeOnLoadMethod]
        public static void Load()
        {
            BackgroundTexture = (Texture2D)Resources.Load("nbc/Background");
            Logo = (Texture2D)Resources.Load("nbc/Logo");
            FirstFrameIcon = (Texture2D)Resources.Load("nbc/play/FirstFrame");
            LastFrameIcon = (Texture2D)Resources.Load("nbc/play/LastFrame");
            NextFrameIcon = (Texture2D)Resources.Load("nbc/play/NextFrame");
            PlayIcon = (Texture2D)Resources.Load("nbc/play/Play");
            PrevFrameIcon = (Texture2D)Resources.Load("nbc/play/PrevFrame");
            RangeIcon = (Texture2D)Resources.Load("nbc/play/Range");
            PauseIcon = (Texture2D)Resources.Load("nbc/play/pause");
            StopIcon = (Texture2D)Resources.Load("nbc/play/stop");

            EyeIcon = (Texture2D)Resources.Load("nbc/icon/Eye");
            LockIcon = (Texture2D)Resources.Load("nbc/icon/Lock");
            CreateIcon = (Texture2D)Resources.Load("nbc/icon/Create");
            MenuIcon = (Texture2D)Resources.Load("nbc/icon/Menu");

            CollapsedIcon = (Texture2D)Resources.Load("nbc/icon/Collapsed");
            ExpandedIcon = (Texture2D)Resources.Load("nbc/icon/Expanded");

            SettingsIcon = (Texture2D)Resources.Load("nbc/icon/settings");
            BackIcon = (Texture2D)Resources.Load("nbc/icon/rollback");
            SaveIcon = (Texture2D)Resources.Load("nbc/icon/save");
            MagnetIcon = (Texture2D)Resources.Load("nbc/icon/magnet");

            TimelineTimeCursorIcon = (Texture2D)Resources.Load("nbc/TimelineTimeCursor");


            TimelineEndPlaybackIcon = (Texture2D)Resources.Load("nbc/TimelineEndPlayback");
            TimelineStartPlaybackIcon = (Texture2D)Resources.Load("nbc/TimelineStartPlayback");

            Stripes = (Texture2D)Resources.Load("nbc/Stripes");

            SignalIcon = (Texture2D)Resources.Load("nbc/Signal");

            _styleSheet = (GUISkin)Resources.Load("nbc/StyleSheet");


            _timelineLeftWidth = EditorPrefs.GetFloat(PrefsConst.Width, 240);
        }
    }
}