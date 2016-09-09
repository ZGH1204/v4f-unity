// <copyright file="Form.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using V4F.Prototype.Map;

namespace V4F.MapEditor
{

    [InitializeOnLoad]
    public class Form : EditorWindow
    {
        #region Types
        public delegate void ChangeToolHandler(Form sender, Tools last, Tools next);
        #endregion

        #region Events
        public event ChangeToolHandler OnChangeTool;
        #endregion

        #region Fields
        private static readonly Tools[]         __tools = null;
        private static readonly GUIStyle[]      __styles = null;
        private static readonly GUIContent[]    __content = null;
        private static readonly GUIContent[][]  __toolbar = null;
        private static readonly Color[]         __colour = null;
        private static readonly Rect[]          __rect = null;
        private static readonly Texture         __background = null;
        private static readonly Texture         __mapTexture = null;
        private static readonly Rect[]          __mapSprites = null;
        private static readonly Rect[]          __rectMarker = null;

        private Dictionary<Tools, ITool> _toolMap = null;
        private int _toolSelected = -1;
        private int _toolPrevious = -1;
        private Vector2 _displayOffset;
        private Vector3 _mousePosition;
        private int _indexSelected;
        private bool _hoverRoomEnabled;
        private bool _hoverTransitionEnabled;
        private bool _closestEnabled;
        private bool _selectTrigger;
        private bool _activateTrigger;
        private bool _removeTrigger;

        private Data _data = null;
        private Dictionary<Node, Rect> _activeRooms;
        private Dictionary<Node, Rect> _activeTransitions;
        #endregion

        #region Properties        
        public static GUIStyle toolbarStyle
        {
            get
            {
                if (__styles[0] == null)
                {
                    var style = new GUIStyle(GUI.skin.button);
                    style.imagePosition = ImagePosition.ImageOnly;
                    style.normal.background = null;
                    style.active.background = null;

                    __styles[0] = style;
                }
                return __styles[0];
            }
        }

        public static GUIStyle buttonStyle
        {
            get
            {
                if (__styles[1] == null)
                {
                    var style = new GUIStyle(GUI.skin.button);
                    style.imagePosition = ImagePosition.ImageLeft;
                    style.alignment = TextAnchor.MiddleLeft;

                    __styles[1] = style;
                }
                return __styles[1];
            }
        }

        public ITool currentTool
        {
            get { return _toolMap[__tools[_toolSelected + 1]]; }
        }

        public Vector2 displayOffset
        {
            get { return _displayOffset; }
            set
            {
                var displayRect = __rect[1];
                _displayOffset.x = Mathf.Clamp(value.x, 0f, displayRect.width);
                _displayOffset.y = Mathf.Clamp(value.y, 0f, displayRect.height);
            }
        }

        public Vector3 mousePosition
        {
            get { return _mousePosition; }
        }

        public bool hoverRoomEnabled
        {
            get { return _hoverRoomEnabled; }
            set { _hoverRoomEnabled = value; }
        }

        public bool hoverTransitionEnabled
        {
            get { return _hoverTransitionEnabled; }
            set { _hoverTransitionEnabled = value; }
        }

        public bool closestEnabled
        {
            get { return _closestEnabled; }
            set { _closestEnabled = value; }
        }

        private Data data
        {
            set
            {
                if (_data != value)
                {
                    if (value != null)
                    {
                        _activeRooms = new Dictionary<Node, Rect>(Data.RoomsCount * Data.RoomsCount);
                        _activeTransitions = new Dictionary<Node, Rect>(Data.RowCount * Data.RowCount);

                        var entry = value.entry;

                        _activeRooms.Add(value[entry], __rectMarker[entry]);
                        for (var i = 0; i < value.length; ++i)
                        {
                            if (i != entry)
                            {
                                var room = value[i];
                                switch (room.type)
                                {
                                    case NodeType.Room:
                                        _activeRooms.Add(room, __rectMarker[i]);
                                        break;

                                    case NodeType.TransitionHor:
                                    case NodeType.TransitionVer:
                                        _activeTransitions.Add(room, __rectMarker[i]);
                                        break;

                                    default: break;
                                }
                            }
                        }

                        titleContent = new GUIContent(string.Format("{0} - {1}", __content[0].text, value.name));
                    }
                    else
                    {
                        _activeRooms = new Dictionary<Node, Rect>(0);
                        _activeTransitions = new Dictionary<Node, Rect>(0);

                        titleContent = __content[0];
                    }

                    _indexSelected = -1;
                }                

                _data = value;
            }
        }

        private bool selectTrigger
        {
            get
            {
                if (_selectTrigger)
                {
                    _selectTrigger = false;
                    return true;
                }                                
                return false;
            }
        }

        private bool activateTrigger
        {
            get
            {
                if (_activateTrigger)
                {
                    _activateTrigger = false;
                    return true;
                }
                return false;
            }
        }

        private bool removeTrigger
        {
            get
            {
                if (_removeTrigger)
                {
                    _removeTrigger = false;
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Constructors
        static Form()
        {
            __tools = System.Enum.GetValues(typeof(Tools)) as Tools[];
            __styles = new GUIStyle[5];

            __content = new GUIContent[]
            {
                new GUIContent("Map Editor"),
                new GUIContent("Open preset:"),
                new GUIContent(" Create New Map", AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/toolbar_button_add.png")),
            };

            var tool_1 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_move.png"));
            var tool_10 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_move_w.png"));
            var tool_2 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_select.png"));
            var tool_20 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_select_w.png"));
            var tool_3 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_brush.png"));
            var tool_30 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_brush_w.png"));
            var tool_4 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_erase.png"));
            var tool_40 = new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_erase_w.png"));

            __toolbar = new GUIContent[][]
            {
                new GUIContent[] { tool_1, tool_2, tool_3, tool_4 },
                new GUIContent[] { tool_10, tool_2, tool_3, tool_4 },
                new GUIContent[] { tool_1, tool_20, tool_3, tool_4 },
                new GUIContent[] { tool_1, tool_2, tool_30, tool_4 },
                new GUIContent[] { tool_1, tool_2, tool_3, tool_40 },
            };

            __rect = new Rect[]
            {
                new Rect(0f, 0f, 1024f, 768f),      // main window
                new Rect(0f, 0f, 768f, 768f),       // map editor
                new Rect(768f, 0f, 256f, 768f),     // toolbar panel
                new Rect(772f, 4f, 152f, 32f),      // tools
                new Rect(772f, 42f, 248f, 686f),    // modify panel
                new Rect(776f, 716f, 244f, 16f),     // open title
                new Rect(776f, 740f, 244f, 16f),     // open dialog
                new Rect(775f, 716f, 228f, 16f),     // open dialog
            };

            __colour = new Color[]
            {
                new Color(0.4f, 0.4f, 0.4f, 1f),
                new Color32(0xA2, 0xA2, 0xA2, 255),
            };

            __background = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/background.png");
            __mapTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Workspace/EditorExtensions/MapEditor/map_ui.png");
            
            var sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(__mapTexture)).OfType<Sprite>().ToArray();
            __mapSprites = new Rect[sprites.Length];
            for (var i = 0; i < sprites.Length; ++i)
            {                
                var rect = sprites[i].textureRect;
                rect.x /= __mapTexture.width;
                rect.y /= __mapTexture.height;
                rect.width /= __mapTexture.width;
                rect.height /= __mapTexture.height;
                __mapSprites[i] = rect;
            }

            __rectMarker = new Rect[Data.RowCount * Data.RowCount];
            
            var delta = Data.RowCount / 2;
            for (var j = 0; j < Data.RowCount; ++j)
            {
                var horizontal = ((j % 2) == 0);
                for (var i = 0; i < Data.RowCount; ++i)
                {
                    var rect = new Rect(0f, 0f, 0f, 0f);
                    var even = (i % 2) == 0;

                    if (even)
                    {
                        if (horizontal)
                        {
                            rect.size = new Vector2(32f, 32f);
                        }
                        else
                        {
                            rect.size = new Vector2(10f, 46f);
                        }
                    }
                    else
                    {
                        if (horizontal)
                        {
                            rect.size = new Vector2(46f, 10f);
                        }                        
                    }

                    rect.center = new Vector2((i - delta) * 39f, (j - delta) * 39f);

                    __rectMarker[j * Data.RowCount + i] = rect;
                }                                                
            }
        }
        #endregion

        #region Methods
        [MenuItem("V4F/Map Editor", false, 200)]
        public static void ShowWindow()
        {
            CreateInstance<Form>().Initialize();
        }

        [MenuItem("Assets/V4F/Edit map", false, -100)]
        public static void DoEditMap()
        {
            CreateInstance<Form>().Initialize(Selection.activeObject as Data);
        }

        [MenuItem("Assets/V4F/Edit map", true, -100)]
        public static bool DoEditMapValidate()
        {            
            return (Selection.activeObject.GetType() == typeof(Data));
        }

        public void TrySelectHall()
        {            
            _selectTrigger = true;            
        }

        public void TryActivateHall()
        {
            _activateTrigger = true;
        }

        public void TryRemoveHall()
        {
            _removeTrigger = true;
        }

        public void ResetSelect()
        {
            _indexSelected = -1;
        }
        
        private void Initialize(Data editable = null)
        {
            titleContent = __content[0];
            minSize = __rect[0].size;
            maxSize = __rect[0].size;

            _toolMap = new Dictionary<Tools, ITool>(__tools.Length)
            {
                {Tools.None, null },
                {Tools.Move, new MoveTool(Tools.Move) },
                {Tools.Select, new SelectTool(Tools.Select) },
                {Tools.Brush, new BrushTool(Tools.Brush) },
                {Tools.Erase, new EraseTool(Tools.Erase) },
            };

            _toolSelected = -1;

            var displayRect = __rect[1];
            _displayOffset = new Vector2(displayRect.width * 0.5f, displayRect.height * 0.5f);

            _indexSelected = -1;
            _hoverRoomEnabled = false;
            _hoverTransitionEnabled = false;
            _closestEnabled = false;                       

            _selectTrigger = false;
            _activateTrigger = false;
            _removeTrigger = false;

            data = editable;            

            ShowUtility();
        }

        private void OnChangeToolCallback(Tools last, Tools next)
        {
            var lastTool = _toolMap[last];
            if (lastTool != null)
            {
                lastTool.Disable(this);
            }

            var nextTool = _toolMap[next];
            if (nextTool != null)
            {
                nextTool.Enable(this);
            }                

            if (OnChangeTool != null)
            {
                OnChangeTool(this, last, next);
            }
        }
        
        private bool DrawClosestRoom(int x, int y, out int hoverIndex)
        {
            hoverIndex = -1;

            var room = _data.GetRoom(x, y);
            if ((room != null) && (room.type == NodeType.None))
            {
                var rect = __rectMarker[room.index];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[5], true);

                if (rect.Contains(_mousePosition))
                {
                    hoverIndex = room.index;
                }
            }

            return (hoverIndex != -1);
        }        

        private void EventsHandler()
        {
            var currentEvent = Event.current;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            var eventType = currentEvent.GetTypeForControl(controlID);

            if (eventType == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlID);
                return;
            }

            _mousePosition = currentEvent.mousePosition;
            
            var mouseWhitin = __rect[1].Contains(_mousePosition);
            var key = ModKey.None;
            
            if (currentTool.Exists() && currentEvent.isMouse)
            {                
                if (currentEvent.shift)
                {
                    FlagsHelper.Set(ref key, ModKey.Shift);
                }
                if (currentEvent.control)
                {
                    FlagsHelper.Set(ref key, ModKey.Ctrl);
                }
                if (currentEvent.alt)
                {
                    FlagsHelper.Set(ref key, ModKey.Alt);
                }

                if ((eventType == EventType.MouseDown) && (HandleUtility.nearestControl == controlID) && mouseWhitin)
                {
                    if ((currentEvent.button == 2) && !currentTool.IsTool(Tools.Move))
                    {
                        _toolPrevious = _toolSelected;
                        _toolSelected = ((int)Tools.Move) - 1;
                        OnChangeToolCallback(__tools[_toolPrevious + 1], __tools[_toolSelected + 1]);
                    }

                    if (currentTool.OnMouseDown(this, key, currentEvent.button))
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }                    
                }

                if ((eventType == EventType.MouseDrag) && (GUIUtility.hotControl == controlID))
                {
                    if (currentTool.OnMouseDrag(this, key, currentEvent.button))
                    {
                        GUI.changed = true;
                        Event.current.Use();
                    }
                }

                if ((eventType == EventType.MouseUp) && (GUIUtility.hotControl == controlID))
                {                    
                    if (currentTool.OnMouseUp(this, key, currentEvent.button))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }

                    if ((_toolPrevious != -1) && (currentEvent.button == 2))
                    {                        
                        OnChangeToolCallback(__tools[_toolSelected + 1], __tools[_toolPrevious + 1]);
                        _toolSelected = _toolPrevious;
                        _toolPrevious = -1;
                    }
                }                
            }
        }
        
        private void DataHandler()
        {                        
            var hoverTransition = -1;
            var hoverRoom = -1;

            foreach (KeyValuePair<Node, Rect> pair in _activeTransitions)
            {
                var rect = pair.Value;
                rect.center += _displayOffset;
                if (__rect[1].Overlaps(rect))
                {
                    var index = (pair.Key.type == NodeType.TransitionVer) ? 2 : 1;
                    GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
                    if ((hoverTransition == -1) && rect.Contains(_mousePosition))
                    {
                        hoverTransition = pair.Key.index;
                    }
                }
            }

            foreach (KeyValuePair<Node, Rect> pair in _activeRooms)
            {
                var rect = pair.Value;
                rect.center += _displayOffset;
                if (__rect[1].Overlaps(rect))
                {
                    var index = (pair.Key.index == _data.entry) ? 7 : 0;
                    GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
                    if ((hoverRoom == -1) && rect.Contains(_mousePosition))
                    {
                        hoverRoom = pair.Key.index;
                    }
                }
            }

            if (_hoverRoomEnabled && hoverRoom != -1)
            {
                var rect = __rectMarker[hoverRoom];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[3], true);
            }

            if (_hoverTransitionEnabled && hoverTransition != -1)
            {
                var hover = _data[hoverTransition];
                var index = (hover.type == NodeType.TransitionVer) ? 10 : 11;

                var rect = __rectMarker[hoverTransition];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
            }

            if (selectTrigger)
            {
                _indexSelected = Mathf.Max(hoverRoom, hoverTransition);
            }

            if (_indexSelected != -1)
            {
                var selected = _data[_indexSelected];

                var index = 4;
                index = (selected.type == NodeType.TransitionHor) ? 9 : index;
                index = (selected.type == NodeType.TransitionVer) ? 8 : index;

                var rect = __rectMarker[_indexSelected];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
                
                if (_closestEnabled && (selected.type == NodeType.Room))
                {
                    var hover = false;
                    var h = -1;
                    var x = 0;
                    var y = 0;

                    _data.IndexToPoint(_indexSelected, out x, out y);
                    
                    if (DrawClosestRoom(x + 1, y, out h))
                    {
                        hover = hover || true;
                        hoverRoom = h;
                    }                    
                    if (DrawClosestRoom(x, y + 1, out h))
                    {
                        hover = hover || true;
                        hoverRoom = h;
                    }                    
                    if (DrawClosestRoom(x - 1, y, out h))
                    {
                        hover = hover || true;
                        hoverRoom = h;
                    }                    
                    if (DrawClosestRoom(x, y - 1, out h))
                    {
                        hover = hover || true;
                        hoverRoom = h;
                    }

                    hover = hover && _hoverRoomEnabled;

                    if (hover && (hoverRoom != -1))
                    {
                        rect = __rectMarker[hoverRoom];
                        rect.center += _displayOffset;
                        GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[6], true);
                    }                    
                }
            }

            if (activateTrigger)
            {                
                var selected = (_indexSelected != -1) ? _data[_indexSelected] : null;
                if (hoverRoom != -1)
                {
                    if ((selected != null) && (selected.type == NodeType.Room))
                    {
                        var closest = false;

                        var x1 = 0;
                        var y1 = 0;
                        _data.IndexToPoint(hoverRoom, out x1, out y1);

                        var x2 = 0;
                        var y2 = 0;
                        _data.IndexToPoint(_indexSelected, out x2, out y2);

                        var room = _data.GetRoom(x1, y1);
                        if (room.type == NodeType.None)
                        {
                            if (_data.AddRoom(x1, y1, out room))
                            {                                
                                _activeRooms.Add(room, __rectMarker[room.index]);                                
                            }
                            closest = true;
                        }
                        else
                        {
                            closest = (Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1)) == 1;
                        }

                        if (closest)
                        {
                            if (_data.AddTransition(x1, y1, x2, y2, out room))
                            {
                                _activeTransitions.Add(room, __rectMarker[room.index]);                                
                            }
                        }
                        
                    }                    

                    _indexSelected = hoverRoom;
                }                
            }

            if (removeTrigger) 
            {
                if ((hoverRoom != -1) && (hoverRoom != _data.entry))
                {
                    var x = 0;
                    var y = 0;
                    _data.IndexToPoint(hoverRoom, out x, out y);

                    Node room = null;
                    if (_data.RemoveRoom(x, y, out room))
                    {
                        _activeRooms.Remove(room);
                        if (_data.RemoveTransition(x, y, x - 1, y, out room))
                        {
                            _activeTransitions.Remove(room);
                        }
                        if (_data.RemoveTransition(x, y, x, y - 1, out room))
                        {
                            _activeTransitions.Remove(room);
                        }
                        if (_data.RemoveTransition(x, y, x + 1, y, out room))
                        {
                            _activeTransitions.Remove(room);
                        }
                        if (_data.RemoveTransition(x, y, x, y + 1, out room))
                        {
                            _activeTransitions.Remove(room);
                        }
                    }

                    if (_indexSelected == hoverRoom)
                    {
                        _indexSelected = -1;
                    }
                }
                else if (hoverTransition != -1)
                {
                    var x1 = 0;
                    var y1 = 0;
                    var x2 = 0;
                    var y2 = 0;
                    if (_data.IndexToPoints(hoverTransition, out x1, out y1, out x2, out y2))
                    {
                        Node room = null;
                        if (_data.RemoveTransition(x1, y1, x2, y2, out room))
                        {
                            _activeTransitions.Remove(room);                            
                        }
                    }

                    if (_indexSelected == hoverTransition)
                    {
                        _indexSelected = -1;
                    }
                }
            }
        }        

        private void ToolbarHandler()
        {
            EditorGUI.DrawRect(__rect[2], __colour[1]);

            var toolbar = __toolbar[_toolSelected + 1];
            var selected = GUI.SelectionGrid(__rect[3], _toolSelected, toolbar, toolbar.Length, toolbarStyle);
            if (selected != _toolSelected)
            {
                OnChangeToolCallback(__tools[_toolSelected + 1], __tools[selected + 1]);
                _toolSelected = selected;
            }

            GUILayout.BeginArea(__rect[4], GUI.skin.box);            
            if (currentTool.Exists() && currentTool.IsTool(Tools.Select) && (_indexSelected != -1))
            {
                EditorGUILayout.LabelField("Description$Data", CustomStyles.italicLabel);
            }
            GUILayout.EndArea();
        }
        
        private void CreateDataHandler()
        {
            EditorGUI.DrawRect(__rect[2], __colour[1]);

            if (GUI.Button(__rect[7], __content[2], buttonStyle))
            {
                var path = EditorUtility.SaveFilePanelInProject("Save new map", "NewMap", "asset", "Bla bla bla", "Assets/Workspace/Custom");
                if (path.Length != 0)
                {
                    var asset = CreateInstance<Data>();

                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    data = asset;
                }
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUI.DrawTextureTransparent(__rect[1], __background);            
            
            if (_data != null)
            {
                EventsHandler();
                DataHandler();
                ToolbarHandler();
            }
            else
            {
                CreateDataHandler();
            }

            data = EditorGUI.ObjectField(__rect[6], _data, typeof(Data), false) as Data;
        }
        #endregion
    }

}
