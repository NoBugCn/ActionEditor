namespace NBC.ActionEditor
{
    [Name("English")]
    public class LanEN : ILanguages
    {
        
        //**********  Welcome *********
        public static string Title = "Action Editor";
        public static string CreateAsset = "Create Asset";
        public static string SelectAsset = "Select Asset";
        public static string Seeting = "Preferences";

        //**********  Crate Window *********
        public static string CrateAssetType = "Create Type";
        public static string CrateAssetName = "Asset Name";
        public static string CreateAssetFileName = "Asset File Name";
        public static string CreateAssetConfirm = "Create";
        public static string CreateAssetReset = "Reset";
        public static string CreateAssetTipsNameNull = "Name cannot be empty!";
        public static string CreateAssetTipsRepetitive = "Duplicate name!";

        //**********  Preferences Window *********
        public static string PreferencesTitle = "Editor Preferences";
        public static string PreferencesTimeStepMode = "Time Step Mode";
        public static string PreferencesSnapInterval = "Snap Interval";
        public static string PreferencesFrameRate = "Frame Rate";
        public static string PreferencesMagnetSnapping = "Magnet Snapping";

        public static string PreferencesMagnetSnappingTips =
            "Turn on other clips before and after the clip is automatically attached";

        public static string PreferencesScrollWheelZooms = "Scroll Wheel Zooms";
        public static string PreferencesScrollWheelZoomsTips = "Turn on the scroll wheel to zoom the timeline area";
        public static string PreferencesSavePath = "Asset save path";
        public static string PreferencesSavePathTips = "Default path on creation and selection";
        public static string PreferencesAutoSaveTime = "auto save time";
        public static string PreferencesAutoSaveTimeTips = "Auto save interval";
        public static string PreferencesHelpDoc = "Help doc";


        //**********  Commom *********
        public static string Select = "Select";
        public static string SelectFile = "Select File";
        public static string SelectFolder = "Select Folder";
        public static string TipsTitle = "Tips";
        public static string TipsConfirm = "Confirm";
        public static string TipsCancel = "Cancel";
        public static string CompilingTips = "Compiling\n... please wait...";
        public static string Disable = "Disable";
        public static string Locked = "Locked";
        public static string Save = "Save";

        //**********  Header *********
        public static string HeaderLastSaveTime = "Last save time：{0}";
        public static string HeaderSelectAsset = "Select：[{0}]";
        public static string OpenPreferencesTips = "Open Preferences";
        public static string SelectAssetTips = "Select Asset";
        public static string OpenMagnetSnappingTips = "Open Magnet Snapping";
        public static string NewAssetTips = "New Asset";
        public static string BackMenuTips = "Back menu";
        public static string PlayLoopTips = "Loop Play";
        public static string PlayForwardTips = "Jump to the end";
        public static string StepForwardTips = "Jump to next frame";
        public static string PauseTips = "Pause";
        public static string PlayTips = "Play";
        public static string StopTips = "Stop";
        public static string StepBackwardTips = "Jump to previous frame";

        //**********  Group Menu *********
        public static string MenuAddTrack = "Add Track";
        public static string MenuPasteTrack = "Paste Track";
        public static string GroupAdd = "Add Group";
        public static string GroupDisable = "Disable Group";
        public static string GroupLocked = "Locked Group";
        public static string GroupReplica = "Replica Group";
        public static string GroupDelete = "Delete Group";
        public static string GroupDeleteTips = "confirm delete group?";

        //**********  Track Menu *********
        public static string TrackDisable = "Disable Track";
        public static string TrackLocked = "Locked Track";
        public static string TrackCopy = "Copy Track";
        public static string TrackReplica = "Replica Track";
        public static string TrackDelete = "Delete Track";
        public static string TrackDeleteTips = "confirm delete track?";

        //**********  Clip Menu *********
        public static string ClipCopy = "Copy";
        public static string ClipCut = "Cut";
        public static string ClipDelete = "Delete";
        public static string MatchClipLength = "Match Clip Length";
        public static string MatchPreviousLoop = "Match Previous Loop";
        public static string MatchNextLoop = "Match Next Loop";
        public static string ClipPaste = "Paste ({0})";

        //**********  Inspector *********
        public static string NotSelectAsset = "not selected asset。";
        public static string InsBaseInfo = "{0} base info";
        public static string OverflowInvalid = "Clip is outside of playable range";
        public static string EndTimeOverflowInvalid = "Clip end time is outside of playable range";
        public static string StartTimeOverflowInvalid = "Clip start time is outside of playable range";
    }
}