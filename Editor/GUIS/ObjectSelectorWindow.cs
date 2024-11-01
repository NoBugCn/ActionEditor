using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NBC.ActionEditor
{
    /// <summary>
    /// 对象选择器窗口
    /// </summary>
    public class ObjectSelectorWindow : EditorWindow
    {
        private class Styles
        {
            public GUIStyle smallStatus = "ObjectPickerSmallStatus";
            public GUIStyle largeStatus = "ObjectPickerLargeStatus";
            public GUIStyle toolbarBack = "ObjectPickerToolbar";
            public GUIStyle tab = "ObjectPickerTab";
            public GUIStyle bottomResize = "WindowBottomResize";
            public GUIStyle background = "ObjectPickerBackground";
            public GUIStyle previewBackground = "PopupCurveSwatchBackground";
            public GUIStyle previewTextureBackground = "ObjectPickerPreviewBackground";

            public GUIStyle resultsGridLabel = "ProjectBrowserGridLabel";
            public GUIStyle resultsLabel = "PR Label";
            public GUIStyle iconAreaBg = "ProjectBrowserIconAreaBg";
            public GUIStyle previewBg = "ProjectBrowserPreviewBg";

            public GUIStyle preButton = "preButton";
            public GUIStyle preToolbar = "preToolbar";
            public GUIStyle dragHandle = "RL DragHandle";
        }

        private class BuiltinRes
        {
            public string name;
            public Texture icon;
            public string path;
            public int id;
        }

        private Styles _styles;
        private float _toolbarHeight = 44f;
        private float _previewSize = 101f;
        private float _topSize;
        private string _searchFilter;
        private bool _focusSearchFilter;
        private int _lastSelectedIdx;
        private BuiltinRes[] _currentBuiltinResources;
        private BuiltinRes[] _activeBuiltinList;
        private string _folderPath;
        private Object _lastSelectedObject;
        private Texture2D _lastSelectedObjectIcon;
        private Action<Object> _itemSelectedCallback;
        private BuiltinRes _noneBuiltinRes;
        private bool _showNoneItem;
        private Vector2 _scrollPosition;
        private EditorWrapperCache _editorWrapperCache;
        private SerializedProperty _cacheProperty;
        private PreviewResizer _previewResizer = new PreviewResizer();
        private static ObjectSelectorWindow _sharedObjectSelector;

        static string SearchField(Rect position, string text)
        {
            Rect position2 = position;
            position2.width -= 15f;
            text = EditorGUI.TextField(position2, text, new GUIStyle("SearchTextField"));
            Rect position3 = position;
            position3.x += position.width - 15f;
            position3.width = 15f;
            if (GUI.Button(position3, GUIContent.none,
                    string.IsNullOrEmpty(text) ? "SearchCancelButtonEmpty" : "SearchCancelButton"))
            {
                text = string.Empty;
                GUIUtility.keyboardControl = 0;
            }

            return text;
        }

        public static ObjectSelectorWindow get
        {
            get
            {
                if (_sharedObjectSelector == null)
                {
                    Object[] array = Resources.FindObjectsOfTypeAll(typeof(ObjectSelectorWindow));
                    if (array != null && array.Length > 0)
                    {
                        _sharedObjectSelector = (ObjectSelectorWindow)array[0];
                    }

                    if (_sharedObjectSelector == null)
                    {
                        _sharedObjectSelector = CreateInstance<ObjectSelectorWindow>();
                    }
                }

                return _sharedObjectSelector;
            }
        }

        /// <summary>
        /// 列表项绘制区域
        /// </summary>
        private Rect listPosition =>
            new Rect(0f, _toolbarHeight, position.width, Mathf.Max(0f, _topSize - _toolbarHeight));

        /// <summary>
        /// 完全文件夹路径
        /// </summary>
        private string folderFullPath =>
            Path.Combine(Application.dataPath, _folderPath.Length > 6 ? _folderPath.Substring(7) : string.Empty);

        /// <summary>
        /// 显示的列表项数量
        /// </summary>
        private int itemCount
        {
            get
            {
                int num2 = _activeBuiltinList.Length;
                int num3 = (!_showNoneItem) ? 0 : 1;
                return num2 + num3;
            }
        }

        /// <summary>
        /// 显示对象选择器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">初始的对象</param>
        /// <param name="itemSelectedCallback">列表项选中回调</param>
        /// <param name="folderPath">所属的文件夹路径</param>
        /// <param name="allowedInstanceIDs"></param>
        public static void ShowObjectPicker<T>(Object obj, Action<Object> itemSelectedCallback,
            string folderPath = "Assets", List<int> allowedInstanceIDs = null) where T : Object
        {
            Type typeFromHandle = typeof(T);
            get.Show(obj, typeFromHandle, null, itemSelectedCallback, folderPath, allowedInstanceIDs);
        }

        /// <summary>
        /// 显示对象选择器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">初始的对象</param>
        /// <param name="requiredTypeName"></param>
        /// <param name="itemSelectedCallback">列表项选中回调</param>
        /// <param name="folderPath">所属的文件夹路径</param>
        /// <param name="allowedInstanceIDs"></param>
        public static void ShowObjectPicker<T>(Object obj, string requiredTypeName,
            Action<Object> itemSelectedCallback,
            string folderPath = "Assets", List<int> allowedInstanceIDs = null) where T : Object
        {
            get.Show(obj, requiredTypeName, null, itemSelectedCallback, folderPath, allowedInstanceIDs);
        }

        public static void ShowObjectPicker<T>(Object obj, Action<Object> itemSelectedCallback,
            string folderPath, Object[] allowedInstanceObjects) where T : Object
        {
            List<int> allowedInstanceIDs = null;
            if (allowedInstanceObjects != null)
            {
                allowedInstanceIDs = new List<int>(allowedInstanceObjects.Length);
                foreach (var allowedInstanceObject in allowedInstanceObjects)
                {
                    allowedInstanceIDs.Add(allowedInstanceObject.GetInstanceID());
                }
            }

            ShowObjectPicker<T>(obj, itemSelectedCallback, folderPath, allowedInstanceIDs);
        }

        public static void ShowObjectPicker(SerializedProperty property,
            Action<Object> itemSelectedCallback, string folderPath = "Assets")
        {
            get.Show(null, typeof(Object), property, itemSelectedCallback, folderPath);
        }

        public void Show(Object obj, Type requiredType, SerializedProperty property,
            Action<Object> itemSelectedCallback, string folderPath, List<int> allowedInstanceIDs = null)
        {
            string requiredTypeName = string.Empty;
            if (property != null)
            {
                obj = property.objectReferenceValue;
                requiredTypeName = property.objectReferenceValue.GetType().Name;
            }
            else
            {
                requiredTypeName = requiredType.Name;
            }

            Show(obj, requiredTypeName, property, itemSelectedCallback, folderPath, allowedInstanceIDs);
        }

        public void Show(Object obj, string requiredType, SerializedProperty property,
            Action<Object> itemSelectedCallback, string folderPath, List<int> allowedInstanceIDs = null)
        {
            _folderPath = folderPath;
            if (!Directory.Exists(folderFullPath))
            {
                Debug.LogError(folderPath + " is not a Directory!");
                return;
            }

            _cacheProperty = property;
            _itemSelectedCallback = itemSelectedCallback;
            InitIfNeeded();


            InitBuiltinList(requiredType, allowedInstanceIDs);
            titleContent = new GUIContent("Select " + requiredType);
            _focusSearchFilter = true;
            _showNoneItem = true;
            _searchFilter = String.Empty;
            ListItemFrame(obj, true);
            ShowAuxWindow();
        }

        /// <summary>
        /// 初始化所指定的文件夹路径里的对象列表
        /// </summary>
        /// <param name="requiredTypeName"></param>
        /// <param name="allowedInstanceIDs"></param>
        private void InitBuiltinList(string requiredTypeName, List<int> allowedInstanceIDs)
        {
            int lenFolderPath = _folderPath.Length; // + 1;
            List<BuiltinRes> builtinResList = new List<BuiltinRes>();

            if (allowedInstanceIDs == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:" + requiredTypeName, new[] { _folderPath });
                foreach (var guid in guids)
                {
                    BuiltinRes builtinRes = new BuiltinRes();
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    builtinRes.name = assetPath.Substring(lenFolderPath, assetPath.LastIndexOf('.') - lenFolderPath);
                    builtinRes.icon = AssetDatabase.GetCachedIcon(assetPath);
                    builtinRes.path = assetPath;
                    builtinResList.Add(builtinRes);
                }
            }
            else
            {
                foreach (var allowedInstanceID in allowedInstanceIDs)
                {
                    string assetPath = AssetDatabase.GetAssetPath(allowedInstanceID);
                    Object obj = EditorUtility.InstanceIDToObject(allowedInstanceID);
                    bool isSub = AssetDatabase.IsSubAsset(allowedInstanceID);
                    string assetName = isSub
                        ? obj.name
                        : assetPath.Substring(lenFolderPath, assetPath.LastIndexOf('.') - lenFolderPath);
                    BuiltinRes builtinRes = new BuiltinRes();
                    builtinRes.name = assetName;
                    builtinRes.icon =
                        isSub ? AssetPreview.GetMiniThumbnail(obj) : AssetDatabase.GetCachedIcon(assetPath);
                    builtinRes.path = assetPath;
                    builtinRes.id = allowedInstanceID;
                    builtinResList.Add(builtinRes);
                }
            }

            _currentBuiltinResources = _activeBuiltinList = builtinResList.ToArray();
        }

        private void OnEnable()
        {
            _previewResizer.Init("ObjectSelectorWindow");
            _previewSize = _previewResizer.GetPreviewSize();
        }

        private void OnDisable()
        {
            _itemSelectedCallback = null;
            _currentBuiltinResources = null;
            _activeBuiltinList = null;
            _lastSelectedObject = null;
            _lastSelectedObjectIcon = null;
            if (_sharedObjectSelector == this)
            {
                _sharedObjectSelector = null;
            }

            if (_editorWrapperCache != null)
            {
                _editorWrapperCache.Dispose();
            }
        }

        private void OnGUI()
        {
            OnObjectListGUI();
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape)
            {
                Cancel();
            }
        }

        private void InitIfNeeded()
        {
            if (_styles == null)
            {
                _styles = new Styles();
            }

            if (_noneBuiltinRes == null)
            {
                _noneBuiltinRes = new BuiltinRes();
                _noneBuiltinRes.name = "None";
            }

            _topSize = position.height - _previewSize;
        }

        private void Cancel()
        {
            Close();
            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        private void OnObjectListGUI()
        {
            InitIfNeeded();
            ResizeBottomPartOfWindow();
            HandleKeyboard();
            GUI.BeginGroup(new Rect(0f, 0f, position.width, position.height), GUIContent.none);
            SearchArea();
            GridListArea();
            PreviewArea();
            GUI.EndGroup();

            GUI.Label(new Rect(position.width * 0.5f - 16f,
                    position.height - _previewSize + 2f, 32f,
                    _styles.bottomResize.fixedHeight),
                GUIContent.none, _styles.bottomResize);
        }

        private void SearchArea()
        {
            GUI.Label(new Rect(0f, 0f, position.width, _toolbarHeight), GUIContent.none, _styles.toolbarBack);
            bool flag = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
            GUI.SetNextControlName("SearchFilter");
            string text = SearchField(new Rect(5f, 5f, position.width - 10f, 15f), _searchFilter);
            if (flag && Event.current.type == EventType.Used)
            {
                if (_searchFilter == string.Empty)
                {
                    Cancel();
                }

                _focusSearchFilter = true;
            }

            if (text != _searchFilter || _focusSearchFilter)
            {
                _searchFilter = text;
                FilterSettingsChanged();
                Repaint();
            }

            if (_focusSearchFilter)
            {
                EditorGUI.FocusTextInControl("SearchFilter");
                _focusSearchFilter = false;
            }

            GUILayout.BeginArea(new Rect(0f, 26f, position.width, _toolbarHeight - 26f));
            GUILayout.BeginHorizontal();
            GUILayout.Toggle(true, _folderPath, _styles.tab);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void GridListArea()
        {
            Rect totalRect = listPosition;
            float itemHeight = itemCount * 16f;
            Rect viewRect = new Rect(0f, 0f, 1f, itemHeight);
            GUI.Label(totalRect, GUIContent.none, _styles.iconAreaBg);
            _scrollPosition = GUI.BeginScrollView(totalRect, _scrollPosition, viewRect);

            int num = FirstVisibleRow(0f, _scrollPosition);
            if (num >= 0 && num < itemCount)
            {
                int num3 = num;
                int num4 = Math.Min(itemCount, 2147483647);
                float num5 = 16f;
                int num6 = (int)Math.Ceiling(position.height / num5);
                num4 = Math.Min(num4, num3 + num6 * 1 + 1);
                DrawListInternal(num3, num4);
            }

            GUI.EndScrollView();

            if (_lastSelectedObject && !_lastSelectedObjectIcon &&
                AssetPreview.IsLoadingAssetPreview(_lastSelectedObject.GetInstanceID()))
            {
                _lastSelectedObjectIcon = AssetPreview.GetAssetPreview(_lastSelectedObject);
                Repaint();
            }
        }

        private void DrawListInternal(int beginIndex, int endIndex)
        {
            int num = beginIndex;
            int num2 = 0;
            if (_showNoneItem)
            {
                if (beginIndex < 1)
                {
                    DrawListItemInternal(ListItemCalcRect(num), _noneBuiltinRes, num);
                    num++;
                }

                num2++;
            }

            if (_activeBuiltinList.Length > 0)
            {
                int num4 = beginIndex - num2;
                num4 = Math.Max(num4, 0);
                int num5 = num4;
                while (num5 < _activeBuiltinList.Length && num <= endIndex)
                {
                    DrawListItemInternal(ListItemCalcRect(num), _activeBuiltinList[num5], num);
                    num++;
                    num5++;
                }
            }
        }

        private void DrawListItemInternal(Rect rect4, BuiltinRes builtinResource, int itemIdx)
        {
            Event current = Event.current;
            float num5 = 18f;
            Rect rect5 = new Rect(num5, rect4.y, rect4.width - num5, rect4.height);
            bool selected = false;
            bool focus = true;

            if (current.type == EventType.MouseDown)
            {
                if (current.button == 0 && rect4.Contains(current.mousePosition))
                {
                    if (current.clickCount == 1)
                    {
                        SetSelectedAssetByIdx(itemIdx);
                        current.Use();
                    }
                    else if (current.clickCount == 2)
                    {
                        current.Use();
                        Close();
                        GUIUtility.ExitGUI();
                    }
                }
            }
            else if (current.type == EventType.Repaint)
            {
                if (itemIdx == _lastSelectedIdx)
                {
                    _styles.resultsLabel.Draw(rect4, GUIContent.none, false, false, true, focus);
                }

                _styles.resultsLabel.Draw(rect5, builtinResource.name, false, false, selected, focus);
                Rect rect6 = rect5;
                rect6.width = 16f;
                rect6.x = 16f;
                if (builtinResource.icon != null)
                {
                    GUI.DrawTexture(rect6, builtinResource.icon);
                }
            }
        }

        /// <summary>
        /// 每个项的矩形区域
        /// </summary>
        /// <param name="itemIdx"></param>
        /// <returns></returns>
        private Rect ListItemCalcRect(int itemIdx)
        {
            return new Rect(0f, itemIdx * 16f, listPosition.width, 16f);
        }

        private void PreviewArea()
        {
            GUI.Box(new Rect(0f, _topSize, position.width, _previewSize), string.Empty, _styles.previewBackground);

            if (_editorWrapperCache == null)
            {
                _editorWrapperCache = new EditorWrapperCache(EditorFeatures.PreviewGUI);
            }

            Object currentObject = _lastSelectedObject;
            EditorWrapper editorWrapper = null;

            if (_previewSize < 75f)
            {
                string text;
                if (currentObject != null)
                {
                    editorWrapper = _editorWrapperCache[currentObject];
                    string str = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
                    if (editorWrapper != null)
                    {
                        text = editorWrapper.name + " (" + str + ")";
                    }
                    else
                    {
                        text = currentObject.name + " (" + str + ")";
                    }

                    text = text + "      " + AssetDatabase.GetAssetPath(currentObject);
                }
                else
                {
                    text = "None";
                }

                LinePreview(text, currentObject, editorWrapper);
            }
            else
            {
                string text3;
                if (currentObject != null)
                {
                    editorWrapper = _editorWrapperCache[currentObject];
                    string text2 = ObjectNames.NicifyVariableName(currentObject.GetType().Name);
                    if (editorWrapper != null)
                    {
                        text3 = editorWrapper.GetInfoString().Replace("\n", "  ");
                        if (text3 != string.Empty)
                        {
                            text3 = string.Concat(new string[]
                            {
                                editorWrapper.name,
                                "\n",
                                text2,
                                "\n",
                                text3
                            });
                        }
                        else
                        {
                            text3 = editorWrapper.name + "\n" + text2;
                        }
                    }
                    else
                    {
                        text3 = currentObject.name + "\n" + text2;
                    }

                    text3 = text3 + "\n" + AssetDatabase.GetAssetPath(currentObject);
                }
                else
                {
                    text3 = "None";
                }

                WidePreview(_previewSize, text3, currentObject, editorWrapper);
                _editorWrapperCache.CleanupUntouchedEditors();
            }
        }

        private void LinePreview(string s, Object o, EditorWrapper p)
        {
            if (_lastSelectedObjectIcon != null)
            {
                GUI.DrawTexture(new Rect(2f, (int)(_topSize + 2f), 16f, 16f), _lastSelectedObjectIcon,
                    ScaleMode.StretchToFill);
            }

            Rect pos = new Rect(20f, _topSize + 1f, position.width - 22f, 18f);
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DropShadowLabel(pos, s, _styles.smallStatus);
            }
            else
            {
                GUI.Label(pos, s, _styles.smallStatus);
            }
        }

        private void WidePreview(float actualSize, string s, Object o, EditorWrapper p)
        {
            float num = 5f;
            Rect pos = new Rect(num, _topSize + num, actualSize - num * 2f, actualSize - num * 2f);
            Rect position2 = new Rect(_previewSize + 3f, _topSize + (_previewSize - 75f) * 0.5f,
                position.width - _previewSize - 3f - num, 75f);
            if (p != null && p.HasPreviewGUI())
            {
                p.OnInteractivePreviewGUI(pos, _styles.previewTextureBackground);

                Rect rect = new Rect(_previewSize + 3f, position.height - 22f, position.width, 16f);
                GUI.BeginGroup(rect);
                EditorGUILayout.BeginHorizontal(GUIStyle.none, GUILayout.Height(17f));
                p.OnPreviewSettings();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUI.EndGroup();
            }
            else
            {
                if (o != null)
                {
                    DrawObjectIcon(pos, _lastSelectedObjectIcon);
                }
            }

            if (EditorGUIUtility.isProSkin)
            {
                EditorGUI.DropShadowLabel(position2, s, _styles.smallStatus);
            }
            else
            {
                GUI.Label(position2, s, _styles.smallStatus);
            }
        }

        private void ResizeBottomPartOfWindow()
        {
            GUI.changed = false;
            float x = 5f + _previewSize - 5f * 2f;
            Rect dragRect = new Rect(x, 0, position.width - x, 0f);
            _previewSize = _previewResizer.ResizeHandle(position, 65f, 270f, 20f, dragRect) + 20f;
            _topSize = position.height - _previewSize;
        }

        private void HandleKeyboard()
        {
            if (!GUI.enabled || Event.current.type != EventType.KeyDown)
            {
                return;
            }

            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    if (_lastSelectedIdx > 0)
                    {
                        _lastSelectedIdx--;
                        SetSelectedAssetByIdx(_lastSelectedIdx);
                    }

                    break;
                case KeyCode.DownArrow:
                    if (_lastSelectedIdx < itemCount - 1)
                    {
                        _lastSelectedIdx++;
                        SetSelectedAssetByIdx(_lastSelectedIdx);
                    }

                    break;
            }
        }

        /// <summary>
        /// 设置选中的索引
        /// </summary>
        /// <param name="selectedIdx"></param>
        /// <param name="callback"></param>
        private void SetSelectedAssetByIdx(int selectedIdx, bool callback = true)
        {
            _lastSelectedIdx = selectedIdx;

            if (_showNoneItem && selectedIdx == 0)
            {
                _lastSelectedObject = null;
                _lastSelectedObjectIcon = null;
            }
            else
            {
                if (_showNoneItem)
                {
                    selectedIdx--;
                }

                if (_activeBuiltinList[selectedIdx].id > 0)
                {
                    _lastSelectedObject = EditorUtility.InstanceIDToObject(_activeBuiltinList[selectedIdx].id);
                }
                else
                {
                    _lastSelectedObject =
                        AssetDatabase.LoadAssetAtPath<Object>(_activeBuiltinList[selectedIdx].path);
                }

                _lastSelectedObjectIcon = AssetPreview.GetAssetPreview(_lastSelectedObject);

                if (_editorWrapperCache != null && _lastSelectedObject)
                {
                    _editorWrapperCache.CleanupUntouchedEditors();
                    EditorWrapper editorWrapper = _editorWrapperCache[_lastSelectedObject];
                    if (editorWrapper != null)
                    {
                    }
                }
            }

            Rect r = ListItemCalcRect(selectedIdx);
            ScrollToPosition(AdjustRectForFraming(r));
            Repaint();

            if (callback && _itemSelectedCallback != null)
            {
                _itemSelectedCallback(_lastSelectedObject);
            }
            else if (callback && _cacheProperty != null)
            {
                _cacheProperty.objectReferenceValue = _lastSelectedObject;
                _cacheProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 绘制对象的预览图标
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="icon"></param>
        private void DrawObjectIcon(Rect rect, Texture icon)
        {
            if (icon == null)
            {
                return;
            }

            int num = Mathf.Min((int)rect.width, (int)rect.height);
            if (num >= icon.width * 2)
            {
                num = icon.width * 2;
            }

            FilterMode filterMode = icon.filterMode;
            icon.filterMode = FilterMode.Point;
            GUI.DrawTexture(
                new Rect(rect.x + ((int)rect.width - num) / 2, rect.y + ((int)rect.height - num) / 2, num, num), icon,
                ScaleMode.ScaleToFit);
            icon.filterMode = filterMode;
        }

        /// <summary>
        /// 找到第一个可见的列表项
        /// </summary>
        /// <param name="yOffset"></param>
        /// <param name="scrollPos"></param>
        /// <returns></returns>
        private int FirstVisibleRow(float yOffset, Vector2 scrollPos)
        {
            float num = scrollPos.y - yOffset;
            int result = 0;
            if (num > 0f)
            {
                float num2 = 16f; // 列表项高度
                result = (int)Mathf.Max(0f, Mathf.Floor(num / num2));
            }

            return result;
        }

        /// <summary>
        /// 搜索字符串变化
        /// </summary>
        private void FilterSettingsChanged()
        {
            BuiltinRes[] array = _currentBuiltinResources;
            if (array != null && array.Length > 0 && !string.IsNullOrEmpty(_searchFilter))
            {
                List<BuiltinRes> list3 = new List<BuiltinRes>();
                string value = _searchFilter.ToLower();
                BuiltinRes[] array2 = array;
                for (int j = 0; j < array2.Length; j++)
                {
                    BuiltinRes builtinResource = array2[j];
                    if (builtinResource.name.ToLower().Contains(value))
                    {
                        list3.Add(builtinResource);
                    }
                }

                array = list3.ToArray();
            }

            _activeBuiltinList = array;

            if (_lastSelectedObject)
            {
                ListItemFrame(_lastSelectedObject, true);
            }
        }

        /// <summary>
        /// 列表项滚动到指定矩形区域
        /// </summary>
        /// <param name="r"></param>
        private void ScrollToPosition(Rect r)
        {
            float y = r.y;
            float yMax = r.yMax;
            float height = listPosition.height;
            if (yMax > height + _scrollPosition.y)
            {
                _scrollPosition.y = yMax - height;
            }

            if (y < _scrollPosition.y)
            {
                _scrollPosition.y = y;
            }

            _scrollPosition.y = Mathf.Max(_scrollPosition.y, 0f);
        }

        /// <summary>
        /// 指定矩形定位后的区域
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private static Rect AdjustRectForFraming(Rect r)
        {
            r.height += _sharedObjectSelector._styles.resultsGridLabel.fixedHeight * 2f;
            r.y -= _sharedObjectSelector._styles.resultsGridLabel.fixedHeight;
            return r;
        }

        /// <summary>
        /// 列表项定位
        /// </summary>
        /// <param name="assetPath"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        private bool ListItemFrame(string assetPath, bool frame)
        {
            int num = ListItemIndexOf(assetPath);
            if (num != -1)
            {
                if (frame)
                {
                    SetSelectedAssetByIdx(num, false);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 列表项定位
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        private bool ListItemFrame(Object obj, bool frame)
        {
            if (obj == null || !AssetDatabase.Contains(obj))
            {
                return false;
            }

            return ListItemFrame(AssetDatabase.GetAssetPath(obj), frame);
        }

        /// <summary>
        /// 根据路径查找所在的索引
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        private int ListItemIndexOf(string assetPath)
        {
            int num = 0;
            if (_showNoneItem)
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    return 0;
                }

                num++;
            }
            else
            {
                if (string.IsNullOrEmpty(assetPath))
                {
                    return -1;
                }
            }

            BuiltinRes[] activeBuiltinList = _activeBuiltinList;
            for (int j = 0; j < activeBuiltinList.Length; j++)
            {
                BuiltinRes builtinResource = activeBuiltinList[j];
                if (assetPath == builtinResource.path)
                {
                    return num;
                }

                num++;
            }

            return -1;
        }
    }


    internal class EditorWrapper : IDisposable
    {
        public delegate void VoidDelegate(SceneView sceneView);

        private Editor editor;
        public string name => editor.target.name;

        private EditorWrapper()
        {
        }

        public void OnEnable()
        {
            MethodInfo method = editor.GetType().GetMethod("OnEnable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(editor, null);
            }
        }

        public void OnDisable()
        {
            MethodInfo method = editor.GetType().GetMethod("OnDisable",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method != null)
            {
                method.Invoke(editor, null);
            }
        }

        public bool HasPreviewGUI()
        {
            return editor.HasPreviewGUI();
        }

        public void OnPreviewSettings()
        {
            editor.OnPreviewSettings();
        }

        public void OnPreviewGUI(Rect position, GUIStyle background)
        {
            editor.OnPreviewGUI(position, background);
        }

        public void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            if (editor != null)
            {
                editor.OnInteractivePreviewGUI(r, background);
            }
        }

        public string GetInfoString()
        {
            return editor.GetInfoString();
        }

        public static EditorWrapper Make(Object obj, EditorFeatures requirements)
        {
            EditorWrapper editorWrapper = new EditorWrapper();
            if (editorWrapper.Init(obj, requirements))
            {
                return editorWrapper;
            }

            editorWrapper.Dispose();
            return null;
        }

        private bool Init(Object obj, EditorFeatures requirements)
        {
            editor = Editor.CreateEditor(obj);
            if (editor == null)
            {
                return false;
            }

            if ((requirements & EditorFeatures.PreviewGUI) > EditorFeatures.None && !editor.HasPreviewGUI())
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            if (editor != null)
            {
                Object.DestroyImmediate(editor);
                editor = null;
            }

            GC.SuppressFinalize(this);
        }
    }


    enum EditorFeatures
    {
        None = 0,
        PreviewGUI = 1,
        OnSceneDrag = 4
    }

    class EditorWrapperCache : IDisposable
    {
        private readonly Dictionary<Object, EditorWrapper> _editorCache;
        private readonly Dictionary<Object, bool> _usedEditors;
        private readonly EditorFeatures _requirements;

        public EditorWrapper this[Object o]
        {
            get
            {
                _usedEditors[o] = true;
                if (_editorCache.TryGetValue(o, out var item))
                {
                    return item;
                }

                EditorWrapper editorWrapper = EditorWrapper.Make(o, _requirements);
                EditorWrapper editorWrapper2 = editorWrapper;
                _editorCache[o] = editorWrapper2;
                return editorWrapper2;
            }
        }

        public EditorWrapperCache(EditorFeatures requirements)
        {
            _requirements = requirements;
            _editorCache = new Dictionary<Object, EditorWrapper>();
            _usedEditors = new Dictionary<Object, bool>();
        }

        public void CleanupUntouchedEditors()
        {
            List<Object> list = new List<Object>();
            foreach (Object current in _editorCache.Keys)
            {
                if (!_usedEditors.ContainsKey(current))
                {
                    list.Add(current);
                }
            }

            if (_editorCache != null)
            {
                foreach (Object current2 in list)
                {
                    EditorWrapper editorWrapper = _editorCache[current2];
                    _editorCache.Remove(current2);
                    if (editorWrapper != null)
                    {
                        editorWrapper.Dispose();
                    }
                }
            }

            _usedEditors.Clear();
        }

        public void CleanupAllEditors()
        {
            _usedEditors.Clear();
            CleanupUntouchedEditors();
        }

        public void Dispose()
        {
            CleanupAllEditors();
            GC.SuppressFinalize(this);
        }
    }
}