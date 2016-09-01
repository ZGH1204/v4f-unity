// <copyright file="Form.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

using V4F.Prototype.Dungeon;

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
        private static readonly Tools[] __tools = null;
        private static readonly GUIStyle[] __styles = null;
        private static readonly GUIContent[] __content = null;
        private static readonly GUIContent[] __toolbar = null;        
        private static readonly Color[] __colour = null;
        private static readonly Rect[] __rect = null;
        private static readonly Texture __background = null;
        private static readonly Texture __mapTexture = null;
        private static readonly Rect[] __mapSprites = null;
        private static readonly Rect[] __rectHalls = null;

        private Dictionary<Tools, ITool> _toolMap = null;        
        private int _toolSelected = -1;
        private int _toolPrevious = -1;
        private Vector2 _displayOffset;
        private Vector3 _mousePosition;
        private int _indexSelected;
        private bool _hoverEnabled;
        private bool _closestEnabled;
        private bool _selectTrigger;
        private bool _activateTrigger;
        private bool _removeTrigger;

        private Preset _editable = null;
        private int[] _stateHalls;
        private Dictionary<int, Rect> _activeHalls;
        private Dictionary<Link, Rect> _linkHalls;
        private int _entryPoint;
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

        public bool hoverEnabled
        {
            get { return _hoverEnabled; }
            set { _hoverEnabled = value; }
        }

        public bool closestEnabled
        {
            get { return _closestEnabled; }
            set { _closestEnabled = value; }
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

            __toolbar = new GUIContent[]
            {
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_move.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_select.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_brush.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/MapEditor/Icons/tool_erase.png")),
            };

            __rect = new Rect[]
            {
                new Rect(0f, 0f, 1024f, 768f),      // main window
                new Rect(0f, 0f, 768f, 768f),       // map editor
                new Rect(768f, 0f, 256f, 768f),     // toolbar panel
                new Rect(772f, 4f, 152f, 32f),      // tools
                new Rect(772f, 42f, 248f, 722f),    // modify panel
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

            __rectHalls = new Rect[19 * 19];
            for (var j = -9; j < 10; ++j)
            {
                var y = j + 9;
                for (var i = -9; i < 10; ++i)
                {
                    var x = i + 9;
                    var rect = new Rect(0f, 0f, 32f, 32f);
                    rect.center = new Vector2(i * 78f, j * 78f);
                    __rectHalls[y * 19 + x] = rect;
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

        private void Initialize()
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
            _hoverEnabled = false;
            _closestEnabled = false;

            _entryPoint = 9 * 19 + 9;
            _activeHalls = new Dictionary<int, Rect>(__rectHalls.Length);
            _activeHalls.Add(_entryPoint, __rectHalls[_entryPoint]);
            _stateHalls = new int[19 * 19];
            _stateHalls[_entryPoint] = 1;

            _linkHalls = new Dictionary<Link, Rect>(__rectHalls.Length, new Link.Comparer());

            _selectTrigger = false;
            _activateTrigger = false;
            _removeTrigger = false;

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

        private bool DrawClosestHall(int x, int y, out int hoverIndex)
        {
            hoverIndex = -1;

            var index = y * 19 + x;
            if ((index == Mathf.Clamp(index, 0, __rectHalls.Length)) && (_stateHalls[index] == 0))
            {
                var rect = __rectHalls[index];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[5], true);

                if (rect.Contains(_mousePosition))
                {
                    hoverIndex = index;
                }
            }

            return (hoverIndex != -1);
        }

        private bool RemoveLink(int index, int dx, int dy)
        {
            var x = index % 19;
            var y = index / 19;

            var pairIndex = (y + dy) * 19 + (x + dx);
            if ((pairIndex == Mathf.Clamp(pairIndex, 0, __rectHalls.Length)) && (_stateHalls[pairIndex] == 1))
            {
                var vertical = Mathf.Abs(dx) < Mathf.Abs(dy);
                return _linkHalls.Remove(new Link(index, pairIndex, vertical));
            }

            return false;                
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

        private void MapEditorHandler()
        {            
            var hoverIndex = -1;

            foreach (KeyValuePair<Link, Rect> pair in _linkHalls)
            {
                var rect = pair.Value;
                rect.center += _displayOffset;
                if (__rect[1].Overlaps(rect))
                {
                    var index = pair.Key.vertical ? 2 : 1;
                    GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
                }
            }

            foreach (KeyValuePair<int, Rect> pair in _activeHalls)
            {
                var rect = pair.Value;
                rect.center += _displayOffset;
                if (__rect[1].Overlaps(rect))
                {
                    var index = (pair.Key == _entryPoint) ? 7 : 0;
                    GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[index], true);
                    if ((hoverIndex == -1) && rect.Contains(_mousePosition))
                    {
                        hoverIndex = pair.Key;
                    }
                }
            }

            if (_hoverEnabled && (hoverIndex != -1))
            {
                var rect = __rectHalls[hoverIndex];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[3], true);
            }

            if (selectTrigger)
            {
                _indexSelected = hoverIndex;                
            }

            if (_indexSelected != -1)
            {
                var rect = __rectHalls[_indexSelected];
                rect.center += _displayOffset;
                GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[4], true);

                if (_closestEnabled)
                {
                    var x = _indexSelected % 19;
                    var y = _indexSelected / 19;
                    var h = -1;

                    var hover = false;
                    
                    if (DrawClosestHall(x + 1, y, out h))
                    {
                        hover = hover || true;
                        hoverIndex = h;
                    }                    
                    if (DrawClosestHall(x, y + 1, out h))
                    {
                        hover = hover || true;
                        hoverIndex = h;
                    }                    
                    if (DrawClosestHall(x - 1, y, out h))
                    {
                        hover = hover || true;
                        hoverIndex = h;
                    }                    
                    if (DrawClosestHall(x, y - 1, out h))
                    {
                        hover = hover || true;
                        hoverIndex = h;
                    }

                    hover = hover && _hoverEnabled;

                    if (hover && (hoverIndex != -1))
                    {
                        rect = __rectHalls[hoverIndex];
                        rect.center += _displayOffset;
                        GUI.DrawTextureWithTexCoords(rect, __mapTexture, __mapSprites[6], true);
                    }                    
                }
            }

            if (activateTrigger && (_indexSelected != hoverIndex))
            {
                if ((_indexSelected != -1) && (hoverIndex != -1))
                {
                    var closest = false;

                    if (_stateHalls[hoverIndex] == 0)
                    {
                        _activeHalls.Add(hoverIndex, __rectHalls[hoverIndex]);
                        _stateHalls[hoverIndex] = 1;

                        closest = true;
                    }
                    else
                    {
                        var x1 = _indexSelected % 19;
                        var y1 = _indexSelected / 19;
                        var x2 = hoverIndex % 19;
                        var y2 = hoverIndex / 19;

                        closest = (Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1)) == 1;
                    }

                    if (closest)
                    {
                        var rc1 = __rectHalls[_indexSelected];
                        var rc2 = __rectHalls[hoverIndex];

                        var diff = rc2.center - rc1.center;
                        var vertical = Mathf.Abs(diff.x) < Mathf.Abs(diff.y);
                        var length = diff.magnitude;

                        var link = new Link(_indexSelected, hoverIndex, vertical);
                        if (!_linkHalls.Keys.Contains(link))
                        {
                            var rect = vertical ? new Rect(0f, 0f, 10f, 46f) : new Rect(0f, 0f, 46f, 10f);
                            rect.center = rc1.center + diff.normalized * (length * 0.5f);

                            _linkHalls.Add(link, rect);                            
                        }                        
                    }

                    _indexSelected = hoverIndex;
                }
                else if((_indexSelected == -1) && (hoverIndex != -1))
                {
                    _indexSelected = hoverIndex;
                }                
            }

            if (removeTrigger && (hoverIndex != -1) && (hoverIndex != _entryPoint))
            {
                if (_stateHalls[hoverIndex] != 0)
                {
                    _activeHalls.Remove(hoverIndex);
                    _stateHalls[hoverIndex] = 0;

                    RemoveLink(hoverIndex, 1, 0);
                    RemoveLink(hoverIndex, 0, 1);
                    RemoveLink(hoverIndex, -1, 0);
                    RemoveLink(hoverIndex, 0, -1);

                    if (_indexSelected == hoverIndex)
                    {
                        _indexSelected = -1;
                    }
                }                
            }
        }

        private void ToolbarHandler()
        {
            EditorGUI.DrawRect(__rect[2], __colour[1]);

            var selected = GUI.SelectionGrid(__rect[3], _toolSelected, __toolbar, __toolbar.Length, toolbarStyle);
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

        private void CreateOrOpenPresetHandler()
        {
            GUI.Button(__rect[7], __content[2], buttonStyle);
            EditorGUI.ObjectField(__rect[6], null, typeof(Preset), false);            
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUI.DrawTextureTransparent(__rect[1], __background);
            /*
            if (_editable != null)
            {
                EventsHandler();
                MapEditorHandler();
                ToolbarHandler();
            }
            else
            {
                CreateOrOpenPresetHandler();
            }
            */

            EventsHandler();
            MapEditorHandler();
            ToolbarHandler();
        }
        #endregion
    }

}
