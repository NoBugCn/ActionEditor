using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public sealed class HeaderTimeInfoGUI : ICustomized
    {
        private AssetPlayer player => AssetPlayer.Inst;

        public void OnGUI()
        {
            var asset = App.AssetData;
            if (asset == null) return;
            var topMiddleRect = G.TopMiddleRect;
            GUI.color = Color.white.WithAlpha(0.2f);
            GUI.Box(topMiddleRect, string.Empty, EditorStyles.toolbarButton);
            GUI.color = Color.black.WithAlpha(0.2f);
            GUI.Box(topMiddleRect, string.Empty, Styles.timeBoxStyle);
            GUI.color = Color.white;

            var timeInfoInterval = 1000000f;
            var timeInfoHighMod = timeInfoInterval;
            var lowMod = 0.01f;
            var modulos = new float[]
                { 0.1f, 0.5f, 1, 5, 10, 50, 100, 500, 1000, 5000, 10000, 50000, 100000, 250000, 500000 }; //... O.o
            for (var i = 0; i < modulos.Length; i++)
            {
                var count = asset.ViewTime / modulos[i];
                if (G.CenterRect.width / count > 50)
                {
                    timeInfoInterval = modulos[i];
                    lowMod = i > 0 ? modulos[i - 1] : lowMod;
                    timeInfoHighMod = i < modulos.Length - 1 ? modulos[i + 1] : timeInfoHighMod;
                    break;
                }
            }

            var doFrames = Prefs.timeStepMode == Prefs.TimeStepMode.Frames;
            var timeStep = doFrames ? (1f / Prefs.frameRate) : lowMod;

            var timeInfoStart = Mathf.FloorToInt(asset.ViewTimeMin / timeInfoInterval) * timeInfoInterval;
            var timeInfoEnd = Mathf.CeilToInt(asset.ViewTimeMax / timeInfoInterval) * timeInfoInterval;
            G.timeInfoStart = Mathf.Round(timeInfoStart * 10) / 10;
            G.timeInfoEnd = Mathf.Round(timeInfoEnd * 10) / 10;
            G.timeInfoInterval = timeInfoInterval;
            G.timeInfoHighMod = timeInfoHighMod;

            GUI.BeginGroup(topMiddleRect);
            {
                //步长间隔
                if (G.CenterRect.width / (asset.ViewTime / timeStep) > 6)
                {
                    for (var i = timeInfoStart; i <= timeInfoEnd; i += timeStep)
                    {
                        var posX = asset.TimeToPos(i);
                        var frameRect = Rect.MinMaxRect(posX - 1, Styles.TOP_MARGIN - 2, posX, Styles.TOP_MARGIN - 5);
                        GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                        GUI.DrawTexture(frameRect, Styles.whiteTexture);
                        GUI.color = Color.white;
                    }
                }

                //时间间隔
                for (var i = timeInfoStart; i <= timeInfoEnd; i += timeInfoInterval)
                {
                    var posX = asset.TimeToPos(i);
                    var rounded = Mathf.Round(i * 10) / 10;

                    GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                    var markRect = Rect.MinMaxRect(posX - 1, Styles.TOP_MARGIN - 2, posX, Styles.TOP_MARGIN - 15);
                    GUI.DrawTexture(markRect, Styles.whiteTexture);
                    GUI.color = Color.white;

                    var text = doFrames ? (rounded * Prefs.frameRate).ToString("0") : rounded.ToString("0.00");
                    var size = GUI.skin.GetStyle("label").CalcSize(new GUIContent(text));
                    var stampRect = new Rect(markRect.x + 2, markRect.y - 18, size.x, size.y);
                    GUI.color = rounded % timeInfoHighMod == 0 ? Color.white : Color.white.WithAlpha(0.5f);
                    GUI.Box(stampRect, text, "label");
                    GUI.color = Color.white;
                }

                //当前播放帧数字
                if (player.CurrentTime > 0)
                {
                    var label = doFrames
                        ? (player.CurrentTime * Prefs.frameRate).ToString("0")
                        : player.CurrentTime.ToString("0.00");
                    var text = "<b><size=16>" + label + "</size></b>";
                    var size = Styles.headerBoxStyle.CalcSize(new GUIContent(text));
                    var posX = asset.TimeToPos(player.CurrentTime);
                    var stampRect = new Rect(0, 0, size.x, size.y);
                    stampRect.center = new Vector2(posX, Styles.TOP_MARGIN - size.y / 2);

                    GUI.backgroundColor = EditorGUIUtility.isProSkin
                        ? Color.black.WithAlpha(0.4f)
                        : Color.black.WithAlpha(0.7f);
                    GUI.color = AssetPlayer.Inst.GetScriberColor();
                    GUI.Box(stampRect, text, Styles.headerBoxStyle);
                }

                //长度位置箭头图片和预退出长度指示
                var lengthPos = asset.TimeToPos(asset.Length);
                var lengthRect = new Rect(0, 0, 18, 18);
                lengthRect.center = new Vector2(lengthPos - 1, Styles.TOP_MARGIN - 2);
                GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                GUI.DrawTexture(lengthRect, Styles.carretIcon);
                GUI.color = Color.white;
            }
            GUI.EndGroup();
        }
    }
}