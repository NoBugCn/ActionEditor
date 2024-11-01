using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    [Attachable(typeof(Group))]
    public abstract class Track : IDirectable
    {
        [SerializeField] private List<Clip> actionClips = new();

        [SerializeField] [HideInInspector] private string name;
        [SerializeField] [HideInInspector] private bool active = true;
        [SerializeField] [HideInInspector] private bool isLocked;
        [SerializeField] private Color color = Color.white;

        public Color Color => color.a > 0.1f ? color : Color.white;


        public virtual string info => string.Empty;

        public List<Clip> Clips
        {
            get => actionClips;
            set => actionClips = value;
        }


        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
        }

        [fsIgnore] public IDirector Root => Parent?.Root;
        [fsIgnore] public IDirectable Parent { get; private set; }

        [fsIgnore] public Group Group => Parent as Group;
        
        public string Name
        {
            get => name;
            set => name = value;
        }

        IEnumerable<IDirectable> IDirectable.Children => Clips;

        public GameObject Actor => Parent?.Actor;

        public virtual bool IsCollapsed
        {
            get => Parent != null && Parent.IsCollapsed;
            set { }
        }

        public virtual bool IsActive
        {
            get => Parent != null && Parent.IsActive && active;
            set
            {
                if (active != value)
                {
                    active = value;
                    if (Root != null) Root.Validate();
                }
            }
        }

        public virtual bool IsLocked
        {
            get => Parent != null && (Parent.IsLocked || isLocked);
            set => isLocked = value;
        }


        public int StartTimeInt => 0;
        public int EndTimeInt => 0;

        public virtual float StartTime
        {
            get => Parent?.StartTime ?? 0;
            set { }
        }


        public virtual float EndTime
        {
            get => Parent?.EndTime ?? 0;
            set { }
        }

        public virtual float BlendIn
        {
            get => 0f;
            set { }
        }


        public virtual float BlendOut
        {
            get => 0f;
            set { }
        }

        public bool CanCrossBlend => false;

        public bool Initialize()
        {
            return true;
        }

        public void Validate(IDirector root, IDirectable parent)
        {
            // Debug.Log($"设置轨道的父节点==={parent}");
            Parent = parent;
            OnAfterValidate();
        }

        protected virtual void OnCreate()
        {
        }

        protected virtual void OnAfterValidate()
        {
        }


        public void PostCreate(IDirectable parent)
        {
            Parent = parent;
            OnCreate();
        }


        public T AddClip<T>(float time) where T : Clip
        {
            return (T)AddClip(typeof(T), time);
        }

        public Clip AddClip(Type type, float time)
        {
            var catAtt =
                type.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
            if (catAtt != null && Clips.Count == 0) Name = catAtt.category + " Track";

            var newAction = Activator.CreateInstance(type) as Clip;

            Debug.Log($"type={type} newAction={newAction}");

            if (newAction != null)
            {
                // if (!newAction.CanAdd(this)) return null;

                newAction.StartTime = time;
                newAction.Name = type.Name;
                Clips.Add(newAction);
                newAction.PostCreate(this);

                var nextAction = Clips.FirstOrDefault(a => a.StartTime > newAction.StartTime);
                if (nextAction != null) newAction.EndTime = Mathf.Min(newAction.EndTime, nextAction.StartTime);

                Root.Validate();
                // DirectorUtility.selectedObject = newAction;
            }

            return newAction;
        }

        public Clip AddClip(Clip clip)
        {
            if (clip != null && clip.CanValidTime(this, clip.StartTime, clip.EndTime))
            {
                // if (!clip.CanAdd(this)) return null;
                if (clip.Parent != null && clip.Parent is Track track)
                {
                    track.DeleteAction(clip);
                }

                Clips.Add(clip);
                Root.Validate();
            }

            return clip;
        }

        public void DeleteAction(Clip action)
        {
            Clips.Remove(action);
            Root.Validate();
        }
    }
}