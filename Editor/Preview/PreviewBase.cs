namespace NBC.ActionEditor
{
    public abstract class PreviewBase<T> : PreviewBase where T : ActionClip
    {
        public T clip => (T)directable;
    }

    public abstract class PreviewBase
    {
        public DirectableAsset directable;

        public void SetTarget(DirectableAsset t)
        {
            directable = t;
        }
        
        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void ReverseEnter()
        {
        }

        public virtual void Reverse()
        {
        }

        
        public abstract void Update(float time, float previousTime);
    }
}