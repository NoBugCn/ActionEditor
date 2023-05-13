namespace NBC.ActionEditor
{
    [Name("简体中文")]
    public class LanCHS : ILanguages
    {
        
        //**********  Welcome *********
        public static string Title = "行为时间轴编辑器";
        public static string CreateAsset = "创建时间轴";
        public static string SelectAsset = "选择时间轴";
        public static string Seeting = "编辑器配置";


        //**********  Crate Window *********
        public static string CrateAssetType = "创建类型";
        public static string CrateAssetName = "时间轴名称";
        public static string CreateAssetFileName = "时间轴的文件名称";
        public static string CreateAssetConfirm = "创建";
        public static string CreateAssetReset = "重置";
        public static string CreateAssetTipsNameNull = "名称不能为空";
        public static string CreateAssetTipsRepetitive = "已存在同名时间轴";


        //**********  Preferences Window *********
        public static string PreferencesTitle = "编辑器首选项";
        public static string PreferencesTimeStepMode = "时间步长模式";
        public static string PreferencesSnapInterval = "时间步长";
        public static string PreferencesFrameRate = "帧率";
        public static string PreferencesMagnetSnapping = "剪辑吸附";
        public static string PreferencesMagnetSnappingTips = "是否开启剪辑自动吸附前后其他剪辑";
        public static string PreferencesScrollWheelZooms = "滚轮缩放";
        public static string PreferencesScrollWheelZoomsTips = "是否开启滚轮缩放时间轴区域";
        public static string PreferencesSavePath = "配置保存地址";
        public static string PreferencesSavePathTips = "创建和选择时的默认地址";
        public static string PreferencesAutoSaveTime = "自动保存时间";
        public static string PreferencesAutoSaveTimeTips = "定时自动保存操作的间隔时间";
        public static string PreferencesHelpDoc = "帮助文档";


        //**********  Commom *********
        public static string Select = "选择";
        public static string SelectFile = "选择文件";
        public static string SelectFolder = "选择文件夹";
        public static string TipsTitle = "提示";
        public static string TipsConfirm = "确定";
        public static string TipsCancel = "取消";
        public static string CompilingTips = "编译中\n...请稍后...";
        public static string Disable = "禁用";
        public static string Locked = "锁定";
        public static string Save = "保存";

        //**********  Header *********
        public static string HeaderLastSaveTime = "最后保存时间：{0}";
        public static string HeaderSelectAsset = "选中：[{0}]";
        public static string OpenPreferencesTips = "打开首选项界面";
        public static string SelectAssetTips = "点击切换时间轴";
        public static string OpenMagnetSnappingTips = "开启剪辑磁性吸附";
        public static string NewAssetTips = "新建时间轴";
        public static string BackMenuTips = "返回主菜单";
        public static string PlayLoopTips = "循环播放";
        public static string PlayForwardTips = "跳转结尾处";
        public static string StepForwardTips = "跳转下一帧";
        public static string PauseTips = "点击暂停";
        public static string PlayTips = "点击播放";
        public static string StopTips = "点击停止播放";
        public static string StepBackwardTips = "跳转上一帧";

        //**********  Group Menu *********
        public static string MenuAddTrack = "添加轨道";
        public static string MenuPasteTrack = "粘贴轨道";
        public static string GroupAdd = "添加组";
        public static string GroupDisable = "禁用组";
        public static string GroupLocked = "锁定组";
        public static string GroupReplica = "复制组";
        public static string GroupDelete = "删除组";
        public static string GroupDeleteTips = "确定删除组吗?";

        //**********  Track Menu *********
        public static string TrackDisable = "禁用轨道";
        public static string TrackLocked = "锁定轨道";
        public static string TrackCopy = "拷贝轨道";
        public static string TrackReplica = "复制轨道";
        public static string TrackDelete = "删除轨道";
        public static string TrackDeleteTips = "确定删除改轨道吗?";

        //**********  Clip Menu *********
        public static string ClipCopy = "拷贝";
        public static string ClipCut = "剪切";
        public static string ClipDelete = "删除";
        public static string MatchClipLength = "匹配长度";
        public static string MatchPreviousLoop = "匹配上个循环";
        public static string MatchNextLoop = "匹配下个循环";
        public static string ClipPaste = "粘贴 ({0})";

        //**********  Inspector *********
        public static string NotSelectAsset = "当前没有选中的时间轴对象。";
        public static string InsBaseInfo = "{0} 基础信息";
        public static string OverflowInvalid = "剪辑超出有效范围";
        public static string EndTimeOverflowInvalid = "剪辑结束时间超出有效范围";
        public static string StartTimeOverflowInvalid = "剪辑开始时间超出可播放范围";
    }
}