
namespace NBC.ActionEditor
{
    public interface IDirectableTimePointer
    {
        PreviewBase target { get; }
        float time { get; }
        void TriggerForward(float currentTime, float previousTime);
        void TriggerBackward(float currentTime, float previousTime);
        void Update(float currentTime, float previousTime);
    }

    public struct StartTimePointer : IDirectableTimePointer
    {
        private bool triggered;
        private float lastTargetStartTime;
        public PreviewBase target { get; private set; }
        float IDirectableTimePointer.time => target.directable.StartTime;

        public StartTimePointer(PreviewBase target)
        {
            this.target = target;
            triggered = false;
            lastTargetStartTime = target.directable.StartTime;
        }

        void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime)
        {
            if (!target.directable.IsActive) return;
            if (currentTime >= target.directable.StartTime)
            {
                if (!triggered)
                {
                    triggered = true;
                    target.Enter();
                    target.Update(target.directable.ToLocalTime(currentTime), 0);
                }
            }
        }

        void IDirectableTimePointer.Update(float currentTime, float previousTime)
        {
            if (!target.directable.IsActive) return;
            if (currentTime >= target.directable.StartTime && currentTime < target.directable.EndTime &&
                currentTime > 0)
            {
                var deltaMoveClip = target.directable.StartTime - lastTargetStartTime;
                var localCurrentTime = target.directable.ToLocalTime(currentTime);
                var localPreviousTime = target.directable.ToLocalTime(previousTime + deltaMoveClip);

                target.Update(localCurrentTime, localPreviousTime);
                lastTargetStartTime = target.directable.StartTime;
            }
        }

        void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime)
        {
            if (!target.directable.IsActive) return;
            if (currentTime < target.directable.StartTime || currentTime <= 0)
            {
                if (triggered)
                {
                    triggered = false;
                    target.Update(0, target.directable.ToLocalTime(previousTime));
                    target.Reverse();
                }
            }
        }
    }

    public struct EndTimePointer : IDirectableTimePointer
    {
        private bool triggered;
        public PreviewBase target { get; private set; }
        float IDirectableTimePointer.time => target.directable.EndTime;

        public EndTimePointer(PreviewBase target)
        {
            this.target = target;
            triggered = false;
        }

        void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime)
        {
            if (!target.directable.IsActive) return;
            if (currentTime >= target.directable.EndTime)
            {
                if (!triggered)
                {
                    triggered = true;
                    target.Update(target.directable.GetLength(), target.directable.ToLocalTime(previousTime));
                    target.Exit();
                }
            }
        }


        void IDirectableTimePointer.Update(float currentTime, float previousTime)
        {
            
        }


        void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime)
        {
            if (!target.directable.IsActive) return;
            if (currentTime < target.directable.EndTime || currentTime <= 0)
            {
                if (triggered)
                {
                    triggered = false;
                    target.ReverseEnter();
                    target.Update(target.directable.ToLocalTime(currentTime), target.directable.GetLength());
                }
            }
        }
    }
}