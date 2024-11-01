using UnityEngine;

namespace NBC.ActionEditor
{
    public static class DirectableAssetExtensions
    {
        public static float SnapTime(this IDirector asset, float time)
        {
            return Mathf.Round(time / Prefs.SnapInterval) * Prefs.SnapInterval;
        }

        public static float TimeToPos(this IDirector asset, float time, float width)
        {
            return (time - asset.ViewTimeMin) / asset.ViewTime * width;
        }

        public static float PosToTime(this IDirector asset, float pos, float width)
        {
            return pos / width * asset.ViewTime + asset.ViewTimeMin;
        }
        
        public static float WidthToTime(this IDirector asset, float pos, float width)
        {
            return pos / width * asset.ViewTime;
        }
    }
}