using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    public abstract class Asset : IDirector
    {
        [HideInInspector] public List<Group> groups = new();
        [SerializeField] private float length = 5f;
        [SerializeField] private float viewTimeMin;
        [SerializeField] private float viewTimeMax = 5f;

        [SerializeField] private float rangeMin;
        [SerializeField] private float rangeMax = 5f;

        public Asset()
        {
            Init();
        }


        [fsIgnore] public List<IDirectable> directables { get; private set; }

        public float Length
        {
            get => length;
            set => length = Mathf.Max(value, 0.1f);
        }

        public float ViewTimeMin
        {
            get => viewTimeMin;
            set
            {
                if (ViewTimeMax > 0) viewTimeMin = Mathf.Min(value, ViewTimeMax - 0.25f);
            }
        }

        public float ViewTimeMax
        {
            get => viewTimeMax;
            set => viewTimeMax = Mathf.Max(value, ViewTimeMin + 0.25f, 0);
        }


        public float ViewTime => ViewTimeMax - ViewTimeMin;

        public float RangeMin
        {
            get => rangeMin;
            set
            {
                rangeMin = value;
                if (rangeMin < 0) rangeMin = 0;
            }
        }

        public float RangeMax
        {
            get => rangeMax;
            set
            {
                rangeMax = value;
                if (rangeMax < length) rangeMax = length;
            }
        }


        public void UpdateMaxTime()
        {
            var t = 0f;
            foreach (var group in groups)
            {
                if (!group.IsActive) continue;
                foreach (var track in group.Tracks)
                {
                    if (!track.IsActive) continue;
                    foreach (var clip in track.Clips)
                        if (clip.EndTime > t)
                            t = clip.EndTime;
                }
            }

            Length = t;
        }

        public void DeleteGroup(Group group)
        {
            groups.Remove(group);
            Validate();
        }

        public void Validate()
        {
            directables = new List<IDirectable>();
            foreach (IDirectable group in groups.AsEnumerable().Reverse())
            {
                directables.Add(group);
                try
                {
                    group.Validate(this, null);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                foreach (var track in group.Children.Reverse())
                {
                    directables.Add(track);
                    try
                    {
                        track.Validate(this, group);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    foreach (var clip in track.Children)
                    {
                        directables.Add(clip);
                        try
                        {
                            clip.Validate(this, track);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }

            if (directables != null)
                foreach (var d in directables)
                    d.OnAfterDeserialize();

            UpdateMaxTime();
        }

        public Group AddGroup(Type type)
        {
            if (!typeof(Group).IsAssignableFrom(type)) return null;
            var newGroup = Activator.CreateInstance(type) as Group;
            if (newGroup != null)
            {
                newGroup.Name = "New Group";
                groups.Add(newGroup);
                Validate();
            }

            return newGroup;
        }

        public T AddGroup<T>(string name = "") where T : Group, new()
        {
            var newGroup = new T();
            if (string.IsNullOrEmpty(name))
            {
                name = newGroup.GetType().Name;
            }

            newGroup.Name = name;
            groups.Add(newGroup);
            Validate();
            return newGroup;
        }


        public void Init()
        {
            Validate();
        }

        public void OnBeforeSerialize()
        {
            if (directables != null)
                foreach (var d in directables)
                    d.OnBeforeSerialize();

            // groupStr = FullSerializerExtensions.Serialize(typeof(List<Group>), groups);
        }

        public void OnAfterDeserialize()
        {
            // if (!string.IsNullOrEmpty(groupStr))
            // {
            //     var obj = FullSerializerExtensions.Deserialize(typeof(List<Group>), groupStr);
            //     if (obj is List<Group> list)
            //     {
            //         groups = list;
            //     }
            // }
        }
    }
}