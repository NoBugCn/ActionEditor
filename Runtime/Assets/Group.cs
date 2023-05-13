using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    public class Group : DirectableAsset
    {
        [SerializeField, HideInInspector] private List<Track> tracks = new List<Track>();
        [SerializeField, HideInInspector] private bool isCollapsed = false;
        [SerializeField, HideInInspector] private bool active = true;
        [SerializeField, HideInInspector] private bool isLocked = false;


        public virtual Asset Parent
        {
            get => (Asset)_parent;
            set => _parent = value;
        }

        public override float StartTime => 0;
        public override float EndTime => Parent.Length;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<Track> Tracks
        {
            get => tracks;
            set => tracks = value;
        }

        public override bool IsActive
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;
                }
            }
        }

        public override bool IsCollapsed
        {
            get => isCollapsed;
            set => isCollapsed = value;
        }

        public override bool IsLocked
        {
            get => isLocked;
            set => isLocked = value;
        }


        #region 增删

        public bool CanAddTrack(Track track)
        {
            return track != null && CanAddTrackOfType(track.GetType());
        }

        public bool CanAddTrackOfType(Type type)
        {
            if (type == null || !type.IsSubclassOf(typeof(Track)) || type.IsAbstract)
            {
                return false;
            }

            if (type.IsDefined(typeof(UniqueAttribute), true) &&
                Tracks.FirstOrDefault(t => t.GetType() == type) != null)
            {
                return false;
            }

            var attachAtt = type.RTGetAttribute<AttachableAttribute>(true);
            if (attachAtt == null || attachAtt.Types == null || attachAtt.Types.All(t => t != this.GetType()))
            {
                return false;
            }

            return true;
        }

        public T AddTrack<T>(string _name = null) where T : Track
        {
            return (T)AddTrack(typeof(T), _name);
        }

        public Track AddTrack(Type type, string _name = null)
        {
            var newTrack = CreateInstance(type);
            if (newTrack is Track track)
            {
                track.Name = type.Name;
                track.Parent = this;
                Tracks.Add(track);

                CreateUtilities.SaveAssetIntoObject(track, this);
                DirectorUtility.selectedObject = track;

                return track;
            }

            return null;
        }

        public void DeleteTrack(Track track)
        {
            // Undo.RegisterCompleteObjectUndo(this, "Delete Track");
            Tracks.Remove(track);
            if (ReferenceEquals(DirectorUtility.selectedObject, track))
            {
                DirectorUtility.selectedObject = null;
            }

            // Undo.DestroyObjectImmediate(track);
            // EditorUtility.SetDirty(this);
            // root?.Validate();
            // root?.SaveToAssets();
        }


        public Track PasteTrack(Track track)
        {
            if (!CanAddTrack(track))
            {
                return null;
            }

            var newTrack = Instantiate(track);
            if (newTrack != null)
            {
                newTrack.Parent = this;
                Tracks.Add(newTrack);
                CreateUtilities.SaveAssetIntoObject(newTrack, this);
                newTrack.Clips.Clear();
                foreach (var clip in track.Clips)
                {
                    newTrack.PasteClip(clip);
                }
            }

            return newTrack;
        }

        #endregion
    }
}