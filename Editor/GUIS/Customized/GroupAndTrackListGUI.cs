using System.Linq;
using NBC.ActionEditor.Draws;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public class GroupAndTrackListGUI : ICustomized
    {
        private static readonly Color LIST_SELECTION_COLOR = new Color(0.5f, 0.5f, 1, 0.3f);
        private static readonly Color GROUP_COLOR = new Color(0f, 0f, 0f, 0.25f);

        private AssetPlayer player => AssetPlayer.Inst;
        public Asset asset => App.AssetData;

        private Rect leftRect;
        private bool isResizingLeftMargin;
        private Group pickedGroup;
        private Track _copyTrack;
        private Track _pickedTrack;

        /// <summary>
        /// 轨道/轨道组 列表
        /// </summary>
        public void OnGUI()
        {
            leftRect = G.LeftRect;
            var e = Event.current;

            var scaleRect = new Rect(leftRect.xMax - 4, leftRect.yMin, 4, leftRect.height);
            ActionEditorWindow.current.AddCursorRect(scaleRect, MouseCursor.ResizeHorizontal);
            if (e.type == EventType.MouseDown && e.button == 0 && scaleRect.Contains(e.mousePosition))
            {
                isResizingLeftMargin = true;
                e.Use();
            }

            if (isResizingLeftMargin)
            {
                Styles.LEFT_MARGIN = e.mousePosition.x + 2;
            }

            if (e.rawType == EventType.MouseUp)
            {
                isResizingLeftMargin = false;
            }

            GUI.enabled = player.CurrentTime <= 0;

            var nextYPos = Styles.FIRST_GROUP_TOP_MARGIN;
            var wasEnabled = GUI.enabled;
            GUI.enabled = true;
            var collapseAllRect = Rect.MinMaxRect(leftRect.x + 5, leftRect.y + 4, 20, leftRect.y + 20 - 1);
            var searchRect = Rect.MinMaxRect(leftRect.x + 20, leftRect.y + 4, leftRect.xMax - 18, leftRect.y + 20 - 1);
            var searchCancelRect = Rect.MinMaxRect(searchRect.xMax, searchRect.y, leftRect.xMax - 4, searchRect.yMax);
            var anyExpanded = asset.groups.Any(g => !g.IsCollapsed);
            ActionEditorWindow.current.AddCursorRect(collapseAllRect, MouseCursor.Link);
            GUI.color = Color.white.WithAlpha(0.5f);
            if (GUI.Button(collapseAllRect, anyExpanded ? "▼" : "►", (GUIStyle)"label"))
            {
                foreach (var group in asset.groups)
                {
                    group.IsCollapsed = anyExpanded;
                }
            }

            GUI.color = Color.white;
            G.SearchString = EditorGUI.TextField(searchRect, G.SearchString, (GUIStyle)"ToolbarSeachTextField");
            if (GUI.Button(searchCancelRect, string.Empty, "ToolbarSeachCancelButton"))
            {
                G.SearchString = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            GUI.enabled = wasEnabled;

            GUI.BeginGroup(leftRect);
            ShowListGroups(e, ref nextYPos);
            GUI.EndGroup();

            G.TotalHeight = nextYPos;

            var addButtonY = G.TotalHeight + Styles.TOP_MARGIN + Styles.TOOLBAR_HEIGHT + 20;
            var addRect = Rect.MinMaxRect(leftRect.xMin + 10, addButtonY, leftRect.xMax - 10, addButtonY + 20);
            GUI.color = Color.white.WithAlpha(0.5f);
            if (GUI.Button(addRect, Lan.GroupAdd))
            {
                var newGroup = asset.AddGroup<Group>();
                DirectorUtility.selectedObject = newGroup;
            }

            //clear picks
            if (e.rawType == EventType.MouseUp)
            {
                pickedGroup = null;
                _pickedTrack = null;
            }

            GUI.enabled = true;
            GUI.color = Color.white;
        }


        private void ShowListGroups(Event e, ref float nextYPos)
        {
            for (int g = 0; g < asset.groups.Count; g++)
            {
                var group = asset.groups[g];

                if (G.IsFilteredOutBySearch(group))
                {
                    group.IsCollapsed = true;
                    continue;
                }

                var groupRect = new Rect(4, nextYPos, leftRect.width - Styles.GROUP_RIGHT_MARGIN - 4,
                    Styles.GROUP_HEIGHT - 3);
                ActionEditorWindow.current?.AddCursorRect(groupRect,
                    pickedGroup == null ? MouseCursor.Link : MouseCursor.MoveArrow);
                nextYPos += Styles.GROUP_HEIGHT;

                var groupSelected = (ReferenceEquals(group, DirectorUtility.selectedObject) || group == pickedGroup);
                GUI.color = groupSelected ? LIST_SELECTION_COLOR : GROUP_COLOR;
                GUI.Box(groupRect, string.Empty, Styles.headerBoxStyle);
                GUI.color = Color.white;


                var plusClicked = false;
                GUI.color = EditorGUIUtility.isProSkin ? Color.white.WithAlpha(0.5f) : new Color(0.2f, 0.2f, 0.2f);
                var plusRect = new Rect(groupRect.xMax - 20, groupRect.y + 6, 16, 16);

                if (GUI.Button(plusRect, Styles.menuIcon, GUIStyle.none))
                {
                    plusClicked = true;
                }

                if (!group.IsActive)
                {
                    var disableIconRect = new Rect(plusRect.xMin - 20, groupRect.y + 6, 16, 16);
                    if (GUI.Button(disableIconRect, Styles.hiddenIcon, GUIStyle.none))
                    {
                        group.IsActive = true;
                    }
                }

                if (group.IsLocked)
                {
                    var lockIconRect = new Rect(plusRect.xMin - (group.IsActive ? 20 : 36), groupRect.y + 6, 16, 16);
                    if (GUI.Button(lockIconRect, Styles.lockIcon, GUIStyle.none))
                    {
                        group.IsLocked = false;
                    }
                }

                GUI.color = EditorGUIUtility.isProSkin ? Color.yellow : Color.white;
                GUI.color = group.IsActive ? GUI.color : Color.grey;
                var foldRect = new Rect(groupRect.x + 2, groupRect.y + 1, 20, groupRect.height);
                group.IsCollapsed =
                    !EditorGUI.Foldout(foldRect, !group.IsCollapsed, $"<b>{@group.Name}</b>");
                GUI.color = Color.white;

                //右键菜单
                if ((e.type == EventType.ContextClick && groupRect.Contains(e.mousePosition)) || plusClicked)
                {
                    var menu = new GenericMenu();
                    foreach (var _info in EditorTools.GetTypeMetaDerivedFrom(typeof(Track)))
                    {
                        var info = _info;
                        if (info.attachableTypes == null || !info.attachableTypes.Contains(group.GetType()))
                        {
                            continue;
                        }

                        var canAdd = !info.isUnique ||
                                     (group.Tracks.Find(track => track.GetType() == info.type) == null);
                        if (group.IsLocked)
                        {
                            canAdd = false;
                        }

                        var finalPath = string.IsNullOrEmpty(info.category)
                            ? info.name
                            : info.category + "/" + info.name;
                        if (canAdd)
                        {
                            menu.AddItem(new GUIContent($"{Lan.MenuAddTrack}/" + finalPath), false,
                                () => { group.AddTrack(info.type); });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent($"{Lan.MenuAddTrack}/" + finalPath));
                        }
                    }

                    menu.AddSeparator("");
                    if (group.CanAddTrack(_copyTrack))
                    {
                        menu.AddItem(new GUIContent(Lan.MenuPasteTrack), false, () =>
                        {
                            var t = group.PasteTrack(_copyTrack);
                            DirectorUtility.selectedObject = t;
                            ActionEditorWindow.current.InitClipWrappers();
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Lan.MenuPasteTrack));
                    }

                    menu.AddItem(new GUIContent(Lan.GroupDisable), !group.IsActive,
                        () => { group.IsActive = !group.IsActive; });
                    menu.AddItem(new GUIContent(Lan.GroupLocked), group.IsLocked,
                        () => { group.IsLocked = !group.IsLocked; });

                    menu.AddSeparator("");

                    menu.AddItem(new GUIContent(Lan.GroupReplica), false, () =>
                    {
                        asset.PasteGroup(group);
                        ActionEditorWindow.current.InitClipWrappers();
                    });

                    menu.AddSeparator("");
                    if (group.IsLocked)
                    {
                        menu.AddDisabledItem(new GUIContent(Lan.GroupDelete));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(Lan.GroupDelete), false, () =>
                        {
                            if (EditorUtility.DisplayDialog(Lan.GroupDelete, Lan.GroupDeleteTips, Lan.TipsConfirm,
                                    Lan.TipsCancel))
                            {
                                asset.DeleteGroup(group);
                                ActionEditorWindow.current.InitClipWrappers();
                            }
                        });
                    }

                    menu.ShowAsContext();
                    e.Use();
                }


                if (e.type == EventType.MouseDown && e.button == 0 && groupRect.Contains(e.mousePosition))
                {
                    DirectorUtility.selectedObject = group;

                    pickedGroup = group;
                    
                    e.Use();
                }

                if (pickedGroup != null && pickedGroup != group) // && !(group is DirectorGroup))
                {
                    if (groupRect.Contains(e.mousePosition))
                    {
                        var markRect = new Rect(groupRect.x,
                            (asset.groups.IndexOf(pickedGroup) < g) ? groupRect.yMax - 2 : groupRect.y,
                            groupRect.width, 2);
                        GUI.color = Color.grey;
                        GUI.DrawTexture(markRect, Styles.whiteTexture);
                        GUI.color = Color.white;
                    }

                    if (e.rawType == EventType.MouseUp && e.button == 0 && groupRect.Contains(e.mousePosition))
                    {
                        asset.groups.Remove(pickedGroup);
                        asset.groups.Insert(g, pickedGroup);
                        pickedGroup = null;
                        e.Use();
                    }
                }

                if (!group.IsCollapsed)
                {
                    ShowListTracks(e, group, ref nextYPos);
                    GUI.color = groupSelected ? LIST_SELECTION_COLOR : GROUP_COLOR;
                    var verticalRect = Rect.MinMaxRect(groupRect.x, groupRect.yMax, groupRect.x + 3, nextYPos - 2);
                    GUI.DrawTexture(verticalRect, Styles.whiteTexture);
                    GUI.color = Color.white;
                }
            }
        }

        /// <summary>
        /// 显示轨道列表
        /// </summary>
        void ShowListTracks(Event e, Group group, ref float nextYPos)
        {
            for (int t = 0; t < group.Tracks.Count; t++)
            {
                var track = group.Tracks[t];
                var yPos = nextYPos;

                var trackRect = new Rect(10, yPos, leftRect.width - Styles.TRACK_RIGHT_MARGIN - 10, track.ShowHeight);
                nextYPos += track.ShowHeight + Styles.TRACK_MARGINS;

                GUI.color = ColorUtility.Grey(EditorGUIUtility.isProSkin
                    ? (track.IsActive ? 0.25f : 0.2f)
                    : (track.IsActive ? 0.9f : 0.8f));
                GUI.DrawTexture(trackRect, Styles.whiteTexture);
                GUI.color = Color.white.WithAlpha(0.25f);
                GUI.Box(trackRect, string.Empty, (GUIStyle)"flow node 0");
                if (ReferenceEquals(track, DirectorUtility.selectedObject) || track == _pickedTrack)
                {
                    GUI.color = LIST_SELECTION_COLOR;
                    GUI.DrawTexture(trackRect, Styles.whiteTexture);
                }

                if (track.IsActive && track.Color != Color.white && track.Color.a > 0.2f)
                {
                    GUI.color = track.Color;
                    var colorRect = new Rect(trackRect.xMax + 1, trackRect.yMin, 2, track.ShowHeight);
                    GUI.DrawTexture(colorRect, Styles.whiteTexture);
                }

                GUI.color = Color.white;

                GUI.BeginGroup(trackRect);
                TrackDraw.Draw(track, trackRect);
                GUI.EndGroup();

                ActionEditorWindow.current.AddCursorRect(trackRect,
                    _pickedTrack == null ? MouseCursor.Link : MouseCursor.MoveArrow);

                //右键菜单
                if (e.type == EventType.ContextClick && trackRect.Contains(e.mousePosition))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(Lan.TrackDisable), !track.IsActive,
                        () => { track.IsActive = !track.IsActive; });
                    menu.AddItem(new GUIContent(Lan.TrackLocked), track.IsLocked,
                        () => { track.IsLocked = !track.IsLocked; });

                    menu.AddSeparator("");

                    menu.AddItem(new GUIContent(Lan.TrackCopy), false, () => { _copyTrack = track; });

                    if (track.GetType().RTGetAttribute<UniqueAttribute>(true) == null)
                    {
                        menu.AddItem(new GUIContent(Lan.TrackReplica), false, () =>
                        {
                            var t1 = group.PasteTrack(track);
                            ActionEditorWindow.current.InitClipWrappers();
                            DirectorUtility.selectedObject = t1;
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(Lan.TrackReplica));
                    }

                    menu.AddSeparator("/");
                    if (track.IsLocked)
                    {
                        menu.AddDisabledItem(new GUIContent(Lan.TrackDelete));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent(Lan.TrackDelete), false, () =>
                        {
                            if (EditorUtility.DisplayDialog(Lan.TrackDelete, Lan.TrackDeleteTips, Lan.TipsConfirm,
                                    Lan.TipsCancel))
                            {
                                group.DeleteTrack(track);
                                ActionEditorWindow.current.InitClipWrappers();
                            }
                        });
                    }

                    menu.ShowAsContext();
                    e.Use();
                }

                //选中
                if (e.type == EventType.MouseDown && e.button == 0 && trackRect.Contains(e.mousePosition))
                {
                    DirectorUtility.selectedObject = track;
                    _pickedTrack = track;
                    e.Use();
                }

                if (_pickedTrack != null && _pickedTrack != track && ReferenceEquals(_pickedTrack.Parent, group))
                {
                    if (trackRect.Contains(e.mousePosition))
                    {
                        var markRect = new Rect(trackRect.x,
                            (group.Tracks.IndexOf(_pickedTrack) < t) ? trackRect.yMax - 2 : trackRect.y,
                            trackRect.width,
                            2);
                        GUI.color = Color.grey;
                        GUI.DrawTexture(markRect, Styles.whiteTexture);
                        GUI.color = Color.white;
                    }

                    if (e.rawType == EventType.MouseUp && e.button == 0 && trackRect.Contains(e.mousePosition))
                    {
                        group.Tracks.Remove(_pickedTrack);
                        group.Tracks.Insert(t, _pickedTrack);
                        _pickedTrack = null;
                        e.Use();
                    }
                }
            }
        }
    }
}