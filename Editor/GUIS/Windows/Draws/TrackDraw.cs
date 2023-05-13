using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor.Draws
{
    public static class TrackDraw
    {
        private const float BOX_WIDTH = 30f;

        #region Icons

        private static readonly Dictionary<Type, Texture> _iconDictionary = new Dictionary<Type, Texture>();
        private static readonly Dictionary<Type, Color> _colorDictionary = new Dictionary<Type, Color>();

        private static Texture GetIcon(this Track track)
        {
            var type = track.GetType();
            if (_iconDictionary.TryGetValue(type, out var icon))
            {
                return icon;
            }

            var att = track.GetType().RTGetAttribute<ShowIconAttribute>(true);

            if (att != null)
            {
                if (att.texture != null)
                {
                    icon = att.texture;
                }
                else if (!string.IsNullOrEmpty(att.iconPath))
                {
                    if (att.iconPath.StartsWith("Assets/"))
                    {
                        icon = AssetDatabase.LoadAssetAtPath<Texture>(att.iconPath);
                    }
                    else
                    {
                        icon = Resources.Load(att.iconPath) as Texture;
                    }
                }

                if (icon == null && !string.IsNullOrEmpty(att.iconPath))
                {
                    icon = EditorGUIUtility.FindTexture(att.iconPath);
                }

                if (icon == null && att.fromType != null)
                {
                    icon = AssetPreview.GetMiniTypeThumbnail(att.fromType);
                }
            }

            if (icon != null)
            {
                _iconDictionary[type] = icon;
            }

            return icon;
        }

        private static Color GetColor(this Track track)
        {
            var type = track.GetType();
            if (_colorDictionary.TryGetValue(type, out var icon))
            {
                return icon;
            }

            var colorAttribute = track.GetType().GetCustomAttribute<ColorAttribute>();
            if (colorAttribute != null)
            {
                _colorDictionary[type] = colorAttribute.Color;
            }
            else
            {
                _colorDictionary[type] = Color.gray;
            }

            return _colorDictionary[type];
        }

        #endregion

        public static void Draw(Track track, Rect trackRect)
        {
            var e = Event.current;

            DoDefaultInfoGUI(track, e, trackRect);

            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
        }

        public static void DoDefaultInfoGUI(Track track, Event e, Rect trackRect)
        {
            var dopeRect = new Rect(0, 0, 4f, track.ShowHeight);
            GUI.color = track.GetColor();
            GUI.Box(dopeRect, string.Empty, Styles.headerBoxStyle);
            GUI.color = Color.white;

            var iconBGRect = new Rect(4f, 0, BOX_WIDTH, track.ShowHeight);
            iconBGRect = iconBGRect.ExpandBy(-1);
            var textInfoRect =
                Rect.MinMaxRect(iconBGRect.xMax + 2, 0, trackRect.width - BOX_WIDTH - 2, track.ShowHeight);
            var curveButtonRect = new Rect(trackRect.width - BOX_WIDTH, 0, BOX_WIDTH, track.ShowHeight);

            GUI.color = Color.black.WithAlpha(0.1f);
            GUI.DrawTexture(iconBGRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            var icon = track.GetIcon();
            if (icon != null)
            {
                var iconRect = new Rect(0, 0, 16, 16);
                iconRect.center = iconBGRect.center;
                GUI.color = ReferenceEquals(DirectorUtility.selectedObject, track)
                    ? Color.white
                    : new Color(1, 1, 1, 0.8f);
                GUI.DrawTexture(iconRect, icon);
                GUI.color = Color.white;
            }

            var nameString = $"<size=11>{track.Name}</size>";
            var infoString = $"<size=9><color=#707070>{track.info}</color></size>";
            GUI.color = track.IsActive ? Color.white : Color.grey;
            GUI.Label(textInfoRect, $"{nameString}\n{infoString}");
            GUI.color = Color.white;


            var wasEnable = GUI.enabled;
            GUI.enabled = true;

            GUI.color = Color.grey;
            if (!track.IsActive)
            {
                var hiddenRect = new Rect(0, 0, 16, 16)
                {
                    center = curveButtonRect.center
                };
                if (GUI.Button(hiddenRect, Styles.hiddenIcon, GUIStyle.none))
                {
                    track.IsActive = !track.IsActive;
                }
            }

            if (track.IsLocked)
            {
                var lockRect = new Rect(0, 0, 16, 16)
                {
                    center = curveButtonRect.center
                };
                if (!track.IsActive)
                {
                    lockRect.center -= new Vector2(16, 0);
                }

                if (GUI.Button(lockRect, Styles.lockIcon, GUIStyle.none))
                {
                    track.IsLocked = !track.IsLocked;
                }
            }

            GUI.color = Color.white;
            GUI.enabled = wasEnable;
        }
        
        public static void DrawTrackContextMenu(Track track, Event e, Rect posRect, float cursorTime)
        {
            var clipsPosRect =
                Rect.MinMaxRect(posRect.xMin, posRect.yMin, posRect.xMax, posRect.yMin + track.ShowHeight);
            if (e.type == EventType.ContextClick && clipsPosRect.Contains(e.mousePosition))
            {
                var attachableTypeInfos = new List<EditorTools.TypeMetaInfo>();

                var existing = track.Clips.FirstOrDefault();
                var existingCatAtt =
                    existing?.GetType().GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as
                        CategoryAttribute;

                foreach (var clip in EditorTools.GetTypeMetaDerivedFrom(typeof(ActionClip)))
                {
                    if (!clip.attachableTypes.Contains(track.GetType()))
                    {
                        continue;
                    }

                    if (existingCatAtt != null)
                    {
                        if (existingCatAtt.category == clip.category)
                        {
                            attachableTypeInfos.Add(clip);
                        }
                    }
                    else
                    {
                        attachableTypeInfos.Add(clip);
                    }
                }

                if (attachableTypeInfos.Count > 0)
                {
                    var menu = new GenericMenu();
                    foreach (var _info in attachableTypeInfos)
                    {
                        var info = _info;
                        var category = string.IsNullOrEmpty(info.category) ? string.Empty : (info.category + "/");
                        var tName = info.name;
                        menu.AddItem(new GUIContent(category + tName), false,
                            () => { track.AddAction(info.type, cursorTime); });
                    }

                    var copyType = DirectorUtility.GetCopyType();
                    if (copyType != null && attachableTypeInfos.Select(i => i.type).Contains(copyType))
                    {
                        menu.AddSeparator("/");
                        menu.AddItem(new GUIContent(string.Format(Lan.ClipPaste, copyType.Name)), false,
                            () => { track.PasteClip(DirectorUtility.CopyClip, cursorTime); });
                    }

                    menu.ShowAsContext();
                    e.Use();
                }
            }
        }
    }
}