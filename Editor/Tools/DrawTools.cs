using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class DrawTools
    {
        public static void DrawDashedLine(float x, float startY, float endY, Color color)
        {
            Handles.BeginGUI();
            Handles.color = color;

            var totalLength = Mathf.Abs(endY - startY);
            var dashes = Mathf.FloorToInt(totalLength / 10); // 每段长度为10

            for (var i = 0; i < dashes; i++)
            {
                var t1 = (float)i / dashes;
                var t2 = (i + 0.5f) / dashes;
                var point1Y = Mathf.Lerp(startY, endY, t1);
                var point2Y = Mathf.Lerp(startY, endY, t2);

                Handles.DrawLine(new Vector2(x, point1Y), new Vector2(x, point2Y));
            }

            Handles.EndGUI();
        }


        #region 绘制贴图

        private static Dictionary<AudioClip, Texture2D> audioTextures = new Dictionary<AudioClip, Texture2D>();

        public static Texture2D GetAudioClipTexture(AudioClip clip, int width, int height)
        {
            if (clip == null)
            {
                return null;
            }

            width = 8192;

            if (audioTextures.TryGetValue(clip, out var texture))
            {
                if (texture != null)
                {
                    return texture;
                }
            }

            if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
            {
                audioTextures[clip] = Styles.WhiteTexture;
                return null;
            }

            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float[] samples = new float[clip.samples * clip.channels];
            int step = Mathf.CeilToInt((clip.samples * clip.channels) / width);
            clip.GetData(samples, 0);
            Color[] xy = new Color[width * height];
            for (int x = 0; x < width * height; x++)
            {
                xy[x] = new Color(0, 0, 0, 0);
            }

            texture.SetPixels(xy);

            int i = 0;
            while (i < width)
            {
                int barHeight = Mathf.CeilToInt(Mathf.Clamp(Mathf.Abs(samples[i * step]) * height, 0, height));
                int add = samples[i * step] > 0 ? 1 : -1;
                for (int j = 0; j < barHeight; j++)
                {
                    texture.SetPixel(i,
                        Mathf.FloorToInt(height / 2) - (Mathf.FloorToInt(barHeight / 2) * add) + (j * add),
                        Color.white);
                }

                ++i;
            }

            texture.Apply();
            audioTextures[clip] = texture;
            return texture;
        }

        /// <summary>
        /// 绘制循环音频剪辑纹理
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="audioClip"></param>
        /// <param name="maxLength"></param>
        /// <param name="offset"></param>
        public static void DrawLoopedAudioTexture(Rect rect, AudioClip audioClip, float maxLength, float offset)
        {
            if (audioClip == null)
            {
                return;
            }

            var audioRect = rect;
            audioRect.width = (audioClip.length / maxLength) * rect.width;
            var t = GetAudioClipTexture(audioClip, (int)audioRect.width, (int)audioRect.height);
            if (t != null)
            {
                Handles.color = new Color(0, 0, 0, 0.2f);
                GUI.color = new Color(0.4f, 0.435f, 0.576f);
                audioRect.yMin += 2;
                audioRect.yMax -= 2;
                for (var f = offset; f < maxLength; f += audioClip.length)
                {
                    audioRect.x = (f / maxLength) * rect.width;
                    rect.x = audioRect.x;
                    GUI.DrawTexture(audioRect, t);
                }

                Handles.color = Color.white;
                GUI.color = Color.white;
            }
            else
            {
                Debug.Log("texture is null");
            }
        }

        /// <summary>
        /// 在 Rect 内绘制环形垂直线，并提供最大长度（带可选偏移量）
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="length"></param>
        /// <param name="maxLength"></param>
        /// <param name="offset"></param>
        public static void DrawLoopedLines(Rect rect, float length, float maxLength, float offset)
        {
            if (length != 0 && maxLength != 0)
            {
                length = Mathf.Abs(length);
                maxLength = Mathf.Abs(maxLength);
                Handles.color = new Color(0, 0, 0, 0.2f);
                for (var f = offset; f < maxLength; f += length)
                {
                    var posX = (f / maxLength) * rect.width;
                    Handles.DrawLine(new Vector2(posX, 0), new Vector2(posX, rect.height));
                }

                Handles.color = Color.white;
            }
        }

        #endregion
    }
}