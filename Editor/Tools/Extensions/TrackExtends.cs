using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class DirectableExtends
    {
        private static readonly Dictionary<Type, Texture2D> _iconDictionary = new Dictionary<Type, Texture2D>();
        private static readonly Dictionary<Type, Color> _colorDictionary = new Dictionary<Type, Color>();
        private static readonly Dictionary<Type, string> _nameDictionary = new Dictionary<Type, string>();

        public static Texture2D GetIcon(this IDirectable track)
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
                        icon = AssetDatabase.LoadAssetAtPath<Texture2D>(att.iconPath);
                    }
                    else
                    {
                        icon = Resources.Load(att.iconPath) as Texture2D;
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

        public static Color GetColor(this IDirectable track)
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

        public static string GetName(this IDirectable track)
        {
            var type = track.GetType();
            if (_nameDictionary.TryGetValue(type, out var name))
            {
                return name;
            }

            var nameAttribute = track.GetType().GetCustomAttribute<NameAttribute>();
            if (nameAttribute != null)
            {
                _nameDictionary[type] = nameAttribute.name;
            }
            else
            {
                _nameDictionary[type] = track.GetType().Name;
            }

            return _nameDictionary[type];
        }
    }
}