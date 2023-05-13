using UnityEngine;

namespace NBC.ActionEditor
{
    public static class AssetExtension
    {
        public static float TimeToPos(this Asset asset, float time)
        {
            return (time - asset.ViewTimeMin) / asset.ViewTime * G.CenterRect.width;
        }

        public static float PosToTime(this Asset asset, float pos)
        {
            return (pos - Styles.LEFT_MARGIN) / G.CenterRect.width * asset.ViewTime + asset.ViewTimeMin;
        }

        // public static float GetTimeInfoStart(this Asset asset)
        // {
        //     return Mathf.Round((Mathf.FloorToInt(asset.ViewTimeMin / G.timeInfoInterval) * G.timeInfoInterval) * 10) /
        //            10;
        // }
        //
        // public static float GetTimeInfoEnd(this Asset asset)
        // {
        //     return Mathf.Round((Mathf.CeilToInt(asset.ViewTimeMax / G.timeInfoInterval) * G.timeInfoInterval) * 10) /
        //            10;
        // }
    }
}