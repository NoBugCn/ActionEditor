using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

namespace NBC.ActionEditor
{
    public delegate void CallbackFunction();

    public delegate void OpenAssetFunction(DirectorAsset asset);

    public static class App
    {
        private static TextAsset _textAsset;

        public static CallbackFunction OnPlay;
        public static CallbackFunction OnStop;

        public static DirectorAsset AssetData { get; private set; } = null;

        public static TextAsset TextAsset
        {
            get => _textAsset;
            set
            {
                _textAsset = value;

                if (_textAsset == null)
                {
                    AssetData = null;
                }
                else
                {
                    var obj = Json.Deserialize(typeof(DirectorAsset), _textAsset.text);
                    if (obj is DirectorAsset asset)
                    {
                        AssetData = asset;
                        asset.Init();
                        App.Refresh();
                    }
                }
            }
        }

        public static EditorWindow Window;

        public static long Frame;

        public static float Width;

        public static void OnObjectPickerConfig(Object obj)
        {
            if (obj is TextAsset textAsset)
            {
                TextAsset = textAsset;
            }
        }

        public static void SaveAsset()
        {
            if (AssetData == null) return;
            AssetData.OnBeforeSerialize();
            var path = AssetDatabase.GetAssetPath(TextAsset);
            var json = Json.Serialize(AssetData);
            System.IO.File.WriteAllText(path, json);
        }

        public static void OnGUIEnd()
        {
            if (Frame > NeedForceRefreshFrame)
            {
                NeedForceRefresh = false;
            }

            Frame++;
            if (Frame >= long.MaxValue)
            {
                Frame = 0;
            }
        }


        public static void OnUpdate()
        {
            TryAutoSave();
        }

        #region AutoSave

        public static DateTime LastSaveTime => _lastSaveTime;

        private static DateTime _lastSaveTime = DateTime.Now;

        /// <summary>
        /// 尝试自动保存
        /// </summary>
        public static void TryAutoSave()
        {
            var timespan = DateTime.Now - _lastSaveTime;
            if (timespan.Seconds > Prefs.autoSaveSeconds)
            {
                AutoSave();
            }
        }

        public static void AutoSave()
        {
            _lastSaveTime = DateTime.Now;
            SaveAsset();
        }

        #endregion

        #region Copy&Cut

        public static IDirectable CopyAsset { get; set; }
        public static bool IsCut { get; set; }

        #endregion

        #region Select

        public static IDirectable[] SelectItems => _selectList.ToArray();
        public static int SelectCount => _selectList.Count;
        private static readonly List<IDirectable> _selectList = new List<IDirectable>();

        public static IDirectable FistSelect => _selectList.Count > 0 ? _selectList.First() : null;

        public static bool CanMultipleSelect { get; set; }

        [System.NonSerialized] private static InspectorPreviewAsset _currentInspectorPreviewAsset;

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

        public static void Select(params IDirectable[] objs)
        {
            var change = false;
            if (objs == null)
            {
                if (_selectList.Count > 0) change = true;
            }
            else
            {
                if (objs.Length != _selectList.Count) change = true;
                else
                {
                    var pickCount = 0;
                    foreach (var obj in objs)
                    {
                        if (_selectList.Contains(obj)) pickCount++;
                    }

                    if (pickCount != objs.Length)
                    {
                        change = true;
                    }
                }
            }

            if (!change) return;
            _selectList.Clear();
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    _selectList.Add(obj);
                }

                Selection.activeObject = CurrentInspectorPreviewAsset;
                EditorUtility.SetDirty(CurrentInspectorPreviewAsset);
                
                // DirectorUtility.selectedObject = FistSelect;
            }

            if (_selectList.Count == 1 && _selectList[0] is not Clip)
            {
                CanMultipleSelect = true;
            }
            else
            {
                CanMultipleSelect = false;
            }
        }

        public static bool IsSelect(IDirectable directable)
        {
            return _selectList.Contains(directable);
        }

        #endregion

        #region Refresh

        public static bool NeedForceRefresh { get; private set; }
        public static long NeedForceRefreshFrame { get; private set; }

        public static void Refresh()
        {
            NeedForceRefresh = true;
            NeedForceRefreshFrame = Frame;
        }


        public static void Repaint()
        {
            if (Window != null)
            {
                Window.Repaint();
            }
        }

        #endregion

        #region 播放相关

        private static float _currentTime = 2;

        public static bool IsPlay;

        public static bool IsStop;

        public static bool IsRange;

        public static float Length
        {
            get
            {
                if (AssetData != null) return AssetData.Length;

                return 0;
            }
        }

        /// <summary>
        /// 当前时间
        /// </summary>
        public static float CurrentTime
        {
            get => _currentTime;
            set => _currentTime = Mathf.Clamp(value, 0, Length);
        }


        // private static AssetPlayer _player => AssetPlayer.Inst;
        //
        // public static bool IsStop =>
        //     Application.isPlaying
        //         ? _player.IsPaused || !_player.IsActive
        //         : EditorPlaybackState == EditorPlaybackState.Stoped;
        //
        // internal static EditorPlaybackState EditorPlaybackState = EditorPlaybackState.Stoped;
        //
        // public static WrapMode EditorPlaybackWrapMode = WrapMode.Loop;
        //
        // public static bool IsPlay => _player.CurrentTime > 0;
        //
        // public static void Play(Action callback = null)
        // {
        //     Play(EditorPlaybackState.PlayingForwards, callback);
        // }
        //
        // private static void Play(EditorPlaybackState playbackState, Action callback = null)
        // {
        //     if (Application.isPlaying)
        //     {
        //         return;
        //     }
        //
        //     EditorPlaybackState = playbackState;
        //     OnPlay?.Invoke();
        // }
        //
        // public static void Pause()
        // {
        //     EditorPlaybackState = EditorPlaybackState.Stoped;
        //     OnStop?.Invoke();
        // }
        //
        // public static void Stop(bool forceRewind)
        // {
        //     if (AssetData != null)
        //         _player.CurrentTime = 0;
        //     EditorPlaybackState = EditorPlaybackState.Stoped;
        //     // WillRepaint = true;
        //
        //     OnStop?.Invoke();
        // }
        //
        // public static void StepForward()
        // {
        //     if (Math.Abs(_player.CurrentTime - _player.Length) < 0.00001f)
        //     {
        //         _player.CurrentTime = 0;
        //         return;
        //     }
        //
        //     _player.CurrentTime += Prefs.snapInterval;
        // }
        //
        // public static void StepBackward()
        // {
        //     if (_player.CurrentTime == 0)
        //     {
        //         _player.CurrentTime = _player.Length;
        //         return;
        //     }
        //
        //     _player.CurrentTime -= Prefs.snapInterval;
        // }

        #endregion
    }
}