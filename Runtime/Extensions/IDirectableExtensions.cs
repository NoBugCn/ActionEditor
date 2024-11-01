using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NBC.ActionEditor
{
    public static class IDirectableExtensions
    {
        #region 长度 and 时间转换

        public static float GetLength(this IDirectable directable)
        {
            return directable.EndTime - directable.StartTime;
        }


        public static float ToLocalTime(this IDirectable directable, float time)
        {
            return Mathf.Clamp(time - directable.StartTime, 0, directable.GetLength());
        }


        public static float ToLocalTimeUnclamped(this IDirectable directable, float time)
        {
            return time - directable.StartTime;
        }

        #endregion

        #region 操作判定

        /// <summary>
        /// 切片能否混合
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool CanCrossBlend(this IDirectable directable, IDirectable other)
        {
            if (directable == null || other == null) return false;

            if ((directable.CanCrossBlend || other.CanCrossBlend) && directable.GetType() == other.GetType())
                return true;

            return false;
        }

        public static bool CanBlendIn(this IDirectable directable)
        {
            var blendInProp = directable.GetType().GetProperty("BlendIn", BindingFlags.Instance | BindingFlags.Public);
            return blendInProp != null && blendInProp.CanWrite && Math.Abs(directable.BlendIn - -1) > 0.0001f &&
                   blendInProp.DeclaringType != typeof(Clip);
        }

        public static bool CanBlendOut(this IDirectable directable)
        {
            var blendOutProp =
                directable.GetType().GetProperty("BlendOut", BindingFlags.Instance | BindingFlags.Public);
            return blendOutProp != null && blendOutProp.CanWrite && Math.Abs(directable.BlendOut - -1) > 0.0001f &&
                   blendOutProp.DeclaringType != typeof(Clip);
        }

        public static bool CanScale(this IDirectable directable)
        {
            var lengthProp = directable.GetType().GetProperty("Length", BindingFlags.Instance | BindingFlags.Public);
            return lengthProp != null && lengthProp.CanWrite && lengthProp.DeclaringType != typeof(Clip);
        }

        /// <summary>
        /// 当前开始时间是否可用
        /// </summary>
        /// <param name="directable"></param>
        /// <returns></returns>
        public static bool CanValidTime(this IDirectable directable)
        {
            return CanValidTime(directable, directable.StartTime, directable.EndTime);
        }

        /// <summary>
        /// 当前开始时间是否可用
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool CanValidTime(this IDirectable directable, float startTime, float endTime)
        {
            if (directable.Parent != null)
            {
                return CanValidTime(directable, directable.Parent, startTime, endTime);
            }

            return true;
        }

        /// <summary>
        /// 当前开始时间是否可用
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="parent"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool CanValidTime(this IDirectable directable, IDirectable parent, float startTime, float endTime)
        {
            var prevDirectable = directable.GetPreviousSibling(parent);
            var nextDirectable = directable.GetNextSibling(parent);

            var limitStartTime = 0f;
            var limitEndTime = float.MaxValue;

            if (prevDirectable != null)
            {
                limitStartTime = prevDirectable.EndTime;
                if (directable.CanCrossBlend(prevDirectable))
                {
                    limitStartTime = prevDirectable.StartTime;

                    //如果完全包含
                    if (startTime > limitStartTime && endTime < prevDirectable.EndTime)
                    {
                        return false;
                    }
                }
            }

            if (nextDirectable != null)
            {
                limitEndTime = nextDirectable.StartTime;
                if (directable.CanCrossBlend(nextDirectable))
                {
                    limitEndTime = nextDirectable.EndTime;
                }
            }

            if (limitStartTime - startTime > 0.0001f) //直接比大小存在精度问题
            {
                return false;
            }

            if (endTime - limitEndTime > 0.0001f)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 切片获取

        // parent.Children.Select()
        // public static T[] GetCoincideSibling<T>(this IDirectable directable) where T : IDirectable
        // {
        //     return GetCoincideSibling(directable, directable.Parent) as T[];
        // }

        public static IDirectable[] GetCoincideSibling(this IDirectable directable)
        {
            return GetCoincideSibling(directable, directable.Parent);
        }

        public static IDirectable[] GetCoincideSibling(this IDirectable directable, IDirectable parent)
        {
            if (parent != null)
            {
                return parent.Children.Where(child => child != directable).Where(child =>
                        child.StartTime == directable.StartTime && child.EndTime == directable.EndTime)
                    .ToArray();
            }

            return Array.Empty<IDirectable>();
        }

        public static T GetPreviousSibling<T>(this IDirectable directable) where T : IDirectable
        {
            return (T)GetPreviousSibling(directable, directable.Parent);
        }

        public static IDirectable GetPreviousSibling(this IDirectable directable)
        {
            return GetPreviousSibling(directable, directable.Parent);
        }

        public static IDirectable GetPreviousSibling(this IDirectable directable, IDirectable parent)
        {
            if (parent != null)
            {
                return parent.Children.LastOrDefault(d =>
                    d != directable && (d.StartTime < directable.StartTime));
            }

            return null;
        }

        /// <summary>
        /// 返回父对象的下个同级
        /// </summary>
        /// <param name="directable"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetNextSibling<T>(this IDirectable directable) where T : IDirectable
        {
            return (T)GetNextSibling(directable, directable.Parent);
        }

        /// <summary>
        /// 返回父对象的下个同级
        /// </summary>
        /// <param name="directable"></param>
        /// <returns></returns>
        public static IDirectable GetNextSibling(this IDirectable directable)
        {
            return GetNextSibling(directable, directable.Parent);
        }

        /// <summary>
        /// 返回指定轨道的下个子对象
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IDirectable GetNextSibling(this IDirectable directable, IDirectable parent)
        {
            if (parent != null)
            {
                return parent.Children.FirstOrDefault(d =>
                    d != directable && d.StartTime > directable.StartTime);
            }

            return null;
        }

        #endregion

        #region 混合权重

        /// <summary>
        /// 根据其混合特性在指定当地时间的权重。
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float GetWeight(this IDirectable directable, float time)
        {
            return GetWeight(directable, time, directable.BlendIn, directable.BlendOut);
        }

        /// <summary>
        /// 基于所提供的覆盖混合入/出属性在指定本地时间的权重
        /// </summary>
        /// <param name="directable"></param>
        /// <param name="time"></param>
        /// <param name="blendInOut"></param>
        /// <returns></returns>
        public static float GetWeight(this IDirectable directable, float time, float blendInOut)
        {
            return GetWeight(directable, time, blendInOut, blendInOut);
        }

        public static float GetWeight(this IDirectable directable, float time, float blendIn, float blendOut)
        {
            var length = GetLength(directable);
            if (time <= 0) return blendIn <= 0 ? 1 : 0;

            if (time >= length) return blendOut <= 0 ? 1 : 0;

            if (time < blendIn) return time / blendIn;

            if (time > length - blendOut) return (length - time) / blendOut;

            return 1;
        }

        #endregion

        #region 循环长度

        /// <summary>
        /// 返回剪辑的上一个循环长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static float GetPreviousLoopLocalTime(this ISubClipContainable clip)
        {
            var clipLength = clip.GetLength();
            var loopLength = clip.SubClipLength / clip.SubClipSpeed;
            if (clipLength > loopLength)
            {
                var mod = (clipLength - clip.SubClipOffset) % loopLength;
                var aproxZero = Mathf.Abs(mod) < 0.01f;
                return clipLength - (aproxZero ? loopLength : mod);
            }

            return clipLength;
        }

        /// <summary>
        /// 返回剪辑的下一个循环长度
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public static float GetNextLoopLocalTime(this ISubClipContainable clip)
        {
            var clipLength = clip.GetLength();
            var loopLength = clip.SubClipLength / clip.SubClipSpeed;
            var mod = (clipLength - clip.SubClipOffset) % loopLength;
            var aproxZero = Mathf.Abs(mod) < 0.01f || Mathf.Abs(loopLength - mod) < 0.01f;
            return clipLength + (aproxZero ? loopLength : loopLength - mod);
        }

        #endregion
    }
}