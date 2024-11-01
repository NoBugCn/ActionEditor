namespace NBC.ActionEditor
{
    public interface ISubClipContainable : IDirectable
    {
        float SubClipOffset { get; set; }
        
        float SubClipSpeed { get; }
        
        float SubClipLength { get; }
    }
}