using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    [Serializable]
    public abstract partial class Asset : DirectableAsset, IDirector
    {
        public List<Group> groups = new List<Group>();
        [SerializeField] private float length = 5f;
        [SerializeField] private float viewTimeMin = 0f;
        [SerializeField] private float viewTimeMax = 5f;

        [HideInInspector, NonSerialized] Track[] m_CacheOutputTracks;

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


        public float MaxTime => Mathf.Max(ViewTimeMax, Length);
        public float ViewTime => ViewTimeMax - ViewTimeMin;

        public List<DirectableAsset> directables { get; private set; }

        public T AddGroup<T>() where T : Group, new()
        {
            var newGroup = CreateInstance<T>();
            newGroup.Name = "New Group";
            newGroup.Parent = this;
            groups.Add(newGroup);
            CreateUtilities.SaveAssetIntoObject(newGroup, this);
            DirectorUtility.selectedObject = newGroup;

            return newGroup;
        }

        public void DeleteGroup(Group group)
        {
            groups.Remove(group);
        }

        public Group PasteGroup(Group group)
        {
            var newGroup = Instantiate(group);
            if (newGroup != null)
            {
                newGroup.Parent = this;
                groups.Add(newGroup);
                CreateUtilities.SaveAssetIntoObject(newGroup, this);
                newGroup.Tracks.Clear();
                foreach (var track in group.Tracks)
                {
                    newGroup.PasteTrack(track);
                }
            }

            return newGroup;
        }

        public override void SaveToAssets()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}