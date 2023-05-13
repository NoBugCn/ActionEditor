using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class DirectorUtility
    {
        private static ActionClip _copyClip;
        private static System.Type _copyClipType;
        
        [System.NonSerialized] private static InspectorPreviewAsset _currentInspectorPreviewAsset;

        [System.NonSerialized] private static ScriptableObject _selectedObject;
        public static event System.Action<ScriptableObject> onSelectionChange;
        
        public static ActionClip CopyClip
        {
            get => _copyClip;
            set
            {
                _copyClip = value;
                if (value != null)
                {
                    _copyClipType = value.GetType();
                }
                else
                {
                    _copyClipType = default;
                }
            }
        }

        public static System.Type GetCopyType()
        {
            return _copyClipType;
        }


        public static void FlushCopyClip()
        {
            _copyClipType = null;
            _copyClip = null;
        }


        public static void CutClip(ActionClip clip)
        {
            _copyClip = clip;
            _copyClipType = clip.GetType();
            clip.Parent.DeleteAction(clip);
        }
        

        public static InspectorPreviewAsset CurrentInspectorPreviewAsset
        {
            get
            {
                if (_currentInspectorPreviewAsset == null)
                {
                    _currentInspectorPreviewAsset = ScriptableObject.CreateInstance<InspectorPreviewAsset>();
                }

                return _currentInspectorPreviewAsset;
            }
        }


        public static ScriptableObject selectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                if (value != null)
                {
#if UNITY_EDITOR
                    Selection.activeObject = CurrentInspectorPreviewAsset;
                    EditorUtility.SetDirty(CurrentInspectorPreviewAsset);
#endif
                }

                if (onSelectionChange != null)
                {
                    onSelectionChange(value);
                }
            }
        }
    }
}