using NBC.ActionEditor;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class ActionClipInspector<T> : ActionClipInspector where T : Clip
    {
        protected T action => (T)target;
    }

    [CustomInspectors(typeof(Clip), true)]
    public class ActionClipInspector : InspectorsBase
    {
        private Clip action => (Clip)target;

        public override void OnInspectorGUI()
        {
            ShowCommonInspector();
        }

        protected void ShowCommonInspector(bool showBaseInspector = true)
        {
            ShowErrors();
            ShowInOutControls();
            ShowBlendingControls();
            if (showBaseInspector)
            {
                base.OnInspectorGUI();
            }
        }
        
        void ShowErrors()
        {
            if (action.IsValid) return;
            EditorGUILayout.HelpBox("该剪辑无效。 请确保设置了所需的参数。",
                MessageType.Error);
            GUILayout.Space(5);
        }
        
        void ShowInOutControls()
        {
            var previousClip = action.GetPreviousSibling();
            var previousTime = previousClip != null ? previousClip.EndTime : action.Parent.StartTime;
            if (action.CanCrossBlend(previousClip))
            {
                previousTime -= Mathf.Min(action.Length / 2, (previousClip.EndTime - previousClip.StartTime) / 2);
            }

            var nextClip = action.GetNextSibling();
            var nextTime = nextClip != null ? nextClip.StartTime : action.Parent.EndTime;
            if (action.CanCrossBlend(nextClip))
            {
                nextTime += Mathf.Min(action.Length / 2, (nextClip.EndTime - nextClip.StartTime) / 2);
            }

            var canScale = action.CanScale();
            var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();

            var _in = action.StartTime;
            var _length = action.Length;
            var _out = action.EndTime;

            if (canScale)
            {
                GUILayout.Label("IN", GUILayout.Width(30));
                if (doFrames)
                {
                    _in *= Prefs.FrameRate;
                    _in = EditorGUILayout.DelayedIntField((int)_in, GUILayout.Width(80));
                    _in *= (1f / Prefs.FrameRate);
                }
                else
                {
                    _in = EditorGUILayout.DelayedFloatField(_in, GUILayout.Width(80));
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("◄");
                if (doFrames)
                {
                    _length *= Prefs.FrameRate;
                    _length = EditorGUILayout.DelayedIntField((int)_length, GUILayout.Width(80));
                    _length *= (1f / Prefs.FrameRate);
                }
                else
                {
                    _length = EditorGUILayout.DelayedFloatField(_length, GUILayout.Width(80));
                }

                GUILayout.Label("►");
                GUILayout.FlexibleSpace();

                GUILayout.Label("OUT", GUILayout.Width(30));
                if (doFrames)
                {
                    _out *= Prefs.FrameRate;
                    _out = EditorGUILayout.DelayedIntField((int)_out, GUILayout.Width(80));
                    _out *= (1f / Prefs.FrameRate);
                }
                else
                {
                    _out = EditorGUILayout.DelayedFloatField(_out, GUILayout.Width(80));
                }
            }

            GUILayout.EndHorizontal();

            if (canScale)
            {
                if (_in >= action.Parent.StartTime && _out <= action.Parent.EndTime)
                {
                    if (_out > _in)
                    {
                        EditorGUILayout.MinMaxSlider(ref _in, ref _out, previousTime, nextTime);
                    }
                    else
                    {
                        _in = EditorGUILayout.Slider(_in, previousTime, nextTime);
                        _out = _in;
                    }
                }
            }
            else
            {
                GUILayout.Label("IN", GUILayout.Width(30));
                _in = EditorGUILayout.Slider(_in, 0, action.Parent.EndTime);
                _out = _in;
            }


            if (GUI.changed)
            {
                if (_length != action.Length)
                {
                    _out = _in + _length;
                }

                _in = Mathf.Round(_in / Prefs.SnapInterval) * Prefs.SnapInterval;
                _out = Mathf.Round(_out / Prefs.SnapInterval) * Prefs.SnapInterval;

                _in = Mathf.Clamp(_in, previousTime, _out);
                _out = Mathf.Clamp(_out, _in, nextClip != null ? nextTime : float.PositiveInfinity);
                
                action.StartTime = _in;
                action.EndTime = _out;
                App.Repaint();
            }

            if (_in > action.Parent.EndTime)
            {
                EditorGUILayout.HelpBox(Lan.OverflowInvalid, MessageType.Warning);
            }
            else
            {
                if (_out > action.Parent.EndTime)
                {
                    EditorGUILayout.HelpBox(Lan.EndTimeOverflowInvalid, MessageType.Warning);
                }
            }

            if (_out < action.Parent.StartTime)
            {
                EditorGUILayout.HelpBox(Lan.OverflowInvalid, MessageType.Warning);
            }
            else
            {
                if (_in < action.Parent.StartTime)
                {
                    EditorGUILayout.HelpBox(Lan.StartTimeOverflowInvalid, MessageType.Warning);
                }
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// 显示混合输入/输出控件
        /// </summary>
        void ShowBlendingControls()
        {
            var canBlendIn = action.CanBlendIn();
            var canBlendOut = action.CanBlendOut();
            if ((canBlendIn || canBlendOut) && action.Length > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.BeginHorizontal();
                if (canBlendIn)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Blend In");
                    var max = action.Length - action.BlendOut;
                    action.BlendIn = EditorGUILayout.Slider(action.BlendIn, 0, max);
                    action.BlendIn = Mathf.Clamp(action.BlendIn, 0, max);
                    GUILayout.EndVertical();
                }

                if (canBlendOut)
                {
                    GUILayout.BeginVertical();
                    GUILayout.Label("Blend Out");
                    var max = action.Length - action.BlendIn;
                    action.BlendOut = EditorGUILayout.Slider(action.BlendOut, 0, max);
                    action.BlendOut = Mathf.Clamp(action.BlendOut, 0, max);
                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
    }
}