namespace NBC.ActionEditor
{
    /// <summary>
    /// 保护子片段的内容，如动画/音频。
    /// </summary>
    public interface ISubClipContainable : IData
    {
        /// <summary>
        /// 偏移量
        /// </summary>
        float SubClipOffset { get; set; }

        /// <summary>
        /// 子片段速度
        /// </summary>
        float SubClipSpeed { get; }

        /// <summary>
        /// 子片段长度
        /// </summary>
        float SubClipLength { get; }
    }
}