using UnityEngine;

namespace NBC.ActionEditor
{
    public static class AssetPlayerExtension
    {
        public static Color GetScriberColor(this AssetPlayer player)
        {
            return player.IsActive ? Color.yellow : new Color(1, 0.3f, 0.3f);
        }
    }
}