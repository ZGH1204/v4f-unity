// <copyright file="CreatePuppetDialoge.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

using V4F.Puppets;

namespace V4F
{

    [InitializeOnLoad]
    public class CreatePuppetDialog : EditorWindow
    {
        public delegate void DialogEventHandler(CreatePuppetDialog sender, CreatePuppetEventArgs args);

        #region Fields
        private static readonly GUIContent[]    __content = null;        
        private static readonly Rect[]          __rect = null;
        private static readonly Color[]         __colour = null;
        private static readonly GUIStyle[]      __styles = null;

        public static event DialogEventHandler Result;

        private PuppetSpec _spec = null;
        private PuppetSkillSet _skillSet = null;
        private Sprite _icon = null;
        private string _properName = null;
        private GameObject _prefab = null;
        private int[] _dropArea = null;
        private string _path = null;
        private bool _saveCreated = false;
        #endregion

        #region Properties
        private static GUIStyle skillSetStyle
        {
            get
            {
                if (__styles[0] == null)
                {
                    var style = new GUIStyle(GUI.skin.box);
                    style.alignment = TextAnchor.LowerCenter;
                    style.imagePosition = ImagePosition.ImageAbove;
                    style.onNormal = style.normal;
                    style.normal.background = null;
                    style.fixedWidth = 85;
                    style.fixedHeight = 60;

                    __styles[0] = style;
                }

                return __styles[0];
            }
        }
        #endregion

        #region Constructors
        static CreatePuppetDialog()
        {
            __content = new GUIContent[]
            {
                new GUIContent("Create New Personage"),
                new GUIContent("Specification"),
                new GUIContent("Resistances"),
                new GUIContent("Skill Set"),
                GUIContent.none,
                new GUIContent("Create"),
                new GUIContent("Cancel"),
                GUIContent.none,
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_edit.png")),
                new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>("Assets/Workspace/EditorExtensions/Icons/common_button_close.png")),
                GUIContent.none,
                new GUIContent("Name:"),
                new GUIContent("Prefab:"),
            };

            __rect = new Rect[]
            {
                new Rect(0f, 0f, 597, 276f),
                new Rect(8f, 8f, 200f, 160f),
                new Rect(212f, 8f, 200f, 160f),
                new Rect(8f, 172f, 404f, 96f),
                new Rect(420f, 0f, 1f, 276f),
                new Rect(461f, 216f, 96f, 24f),
                new Rect(461f, 244f, 96f, 24f),                
                new Rect(461f, 8f, 96f, 96f),
                new Rect(463f, 10f, 92f, 92f),
                new Rect(429f, 108f, 160f, 104f),
            };

            __colour = new Color[]
            {
                new Color(0.2f, 0.2f, 0.2f, 1f),
                new Color(0.5f, 0.5f, 0.5f, 0.25f),
                new Color(0f, 0.75f, 0f, 0.25f),
            };

            __styles = new GUIStyle[5];
        }
        #endregion

        #region Methods
        [MenuItem("Assets/Create/V4F/Personage/Puppet", false, 805)]
        private static void ShowDialog()
        {            
            var dialog = CreateInstance<CreatePuppetDialog>();
            var dialogRect = __rect[0];

            dialog.titleContent = __content[0];
            dialog.minSize = dialogRect.size;
            dialog.maxSize = dialogRect.size;

            dialog._properName = "New Puppet";
            dialog._dropArea = new int[] { 0, 1, 1, 1 };
            dialog._path = AssetDatabase.GetAssetPath(Selection.activeObject);

            dialog.ShowUtility();
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

            if ((eventType == EventType.DragPerform) || (eventType == EventType.DragUpdated))
            {                
                // Specification drop area
                if (__rect[1].Contains(currentEvent.mousePosition))
                {
                    PuppetSpec spec = null;
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        spec = draggedObject as PuppetSpec;
                        if (spec != null)
                        {                            
                            break;
                        }
                    }

                    if (spec != null)
                    {                        
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        _dropArea[1] = 2;

                        if (eventType == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            _dropArea[1] = 1;
                            _spec = spec;
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                    
                    return;
                }
                else
                {
                    _dropArea[1] = 1;
                }

                // Skill Set drop area
                if (__rect[3].Contains(currentEvent.mousePosition))
                {
                    PuppetSkillSet skillSet = null;
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        skillSet = draggedObject as PuppetSkillSet;
                        if (skillSet != null)
                        {                            
                            break;
                        }
                    }

                    if (skillSet != null)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        _dropArea[3] = 2;

                        if (eventType == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            _dropArea[3] = 1;
                            _skillSet = skillSet;                            
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }

                    return;
                }
                else
                {
                    _dropArea[3] = 1;
                }

                // Icon drop area
                if (__rect[7].Contains(currentEvent.mousePosition))
                {
                    Sprite sprite = null;
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        sprite = draggedObject as Sprite;
                        if (sprite != null)
                        {
                            break;
                        }
                    }

                    if (sprite != null)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (eventType == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();                            
                            _icon = sprite;
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }

                    return;
                }                
            }            
        }

        private void OnResult(CreatePuppetEventArgs args)
        {
            if (Result != null)
            {
                Result(this, args);
            }
        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            if (_saveCreated)
            {
                var args = new CreatePuppetEventArgs();
                args.path = _path;
                args.spec = _spec;
                args.skillSet = _skillSet;
                args.icon = _icon;
                args.properName = _properName;
                args.prefab = _prefab;

                OnResult(args);
            }            
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {            
            if (_spec != null)
            {
                var op1 = GUILayout.Width(96f);
                var op2 = new GUILayoutOption[]
                {
                    GUILayout.Width(EditorGUIUtility.singleLineHeight),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight),
                };

                GUILayout.BeginArea(__rect[1]);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(__content[1], EditorStyles.boldLabel, op1);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(__content[8], op2))
                {
                    Selection.activeObject = _spec;
                    EditorGUIUtility.PingObject(_spec);
                }
                if (GUILayout.Button(__content[9], op2))
                {
                    _spec = null;
                }
                EditorGUILayout.EndHorizontal();

                if (_spec != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
                    EditorGUILayout.LabelField("Health Points", _spec.GetStat(PuppetStats.HealthPoints).ToString());
                    EditorGUILayout.LabelField("Accuracy", _spec.GetStat(PuppetStats.Accuracy).ToString());
                    EditorGUILayout.LabelField("Initiative", _spec.GetStat(PuppetStats.Initiative).ToString());
                    EditorGUILayout.LabelField("Stamina", _spec.GetStat(PuppetStats.Stamina).ToString());
                    EditorGUILayout.LabelField("Damage", string.Format("{0}-{1}", _spec.GetStat(PuppetStats.MinDamage), _spec.GetStat(PuppetStats.MaxDamage)));
                    EditorGUILayout.LabelField("Critical Chance", string.Format("{0}%", _spec.GetStat(PuppetStats.CriticalChance)));
                    EditorGUILayout.LabelField("Critical Damage", string.Format("{0}%", _spec.GetStat(PuppetStats.CriticalDamage)));
                    EditorGUILayout.EndVertical();
                }

                GUILayout.EndArea();
            }
            else
            {
                EditorGUI.DrawRect(__rect[1], __colour[_dropArea[1]]);
                EditorGUI.LabelField(__rect[1], __content[1], CustomStyles.infoDrop);
            }

            EditorGUI.DrawRect(__rect[2], __colour[_dropArea[2]]);
            EditorGUI.LabelField(__rect[2], __content[2], CustomStyles.infoDrop);

            if (_skillSet != null)
            {
                var op1 = GUILayout.Width(96f);
                var op2 = new GUILayoutOption[]
                {
                    GUILayout.Width(EditorGUIUtility.singleLineHeight),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight),
                };

                GUILayout.BeginArea(__rect[3]);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(__content[3], EditorStyles.boldLabel, op1);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(__content[8], op2))
                {
                    Selection.activeObject = _skillSet;
                    EditorGUIUtility.PingObject(_skillSet);
                }
                if (GUILayout.Button(__content[9], op2))
                {
                    _skillSet = null;
                }
                EditorGUILayout.EndHorizontal();

                if (_skillSet != null)
                {
                    var countSkills = _skillSet.countSkills;
                    var skillSetContent = new GUIContent[countSkills];
                    for (int i = 0; i < countSkills; ++i)
                    {
                        var skill = _skillSet[i];
                        skillSetContent[i] = new GUIContent(skill.title, skill.icon.texture);
                    }

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    var selected = GUILayout.SelectionGrid(-1, skillSetContent, countSkills, skillSetStyle, GUILayout.ExpandWidth(true));                    
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    if (selected != -1)
                    {
                        var skill = _skillSet[selected];
                        Selection.activeObject = skill;
                        EditorGUIUtility.PingObject(skill);
                    }
                }

                GUILayout.EndArea();
            }
            else
            {
                EditorGUI.DrawRect(__rect[3], __colour[_dropArea[3]]);
                EditorGUI.LabelField(__rect[3], __content[3], CustomStyles.infoDrop);
            }            

            // Separator
            EditorGUI.DrawRect(__rect[4], __colour[0]);
            
            // Icon
            GUILayout.BeginArea(__rect[7], EditorStyles.helpBox);
            if (_icon == null)
            {
                EditorGUILayout.LabelField("Icon", CustomStyles.infoDrop, GUILayout.ExpandHeight(true));
            }            
            GUILayout.EndArea();
            if (_icon != null)
            {
                EditorGUI.DrawTextureTransparent(__rect[8], _icon.texture, ScaleMode.StretchToFill);
            }

            // Custom data
            GUILayout.BeginArea(__rect[9]);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[11], CustomStyles.italicLabel);
            var properName = Regex.Replace(EditorGUILayout.DelayedTextField(_properName), @"[^a-zA-Z0-9_ ]", "").Trim();
            if (!string.IsNullOrEmpty(properName) && (properName.Length > 3))
            {
                _properName = properName;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(__content[12], CustomStyles.italicLabel);
            _prefab = EditorGUILayout.ObjectField(_prefab, typeof(GameObject), false) as GameObject;

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();

            // Events
            EventsHandler();

            // Buttons
            var closeDialog = false;

            var disabled = (
                (_spec == null) || 
                (_skillSet == null) || 
                (_icon == null) ||
                string.IsNullOrEmpty(_properName) ||
                (!string.IsNullOrEmpty(_properName) && (_properName.Length < 3)) ||
                (_prefab == null)
            );            

            EditorGUI.BeginDisabledGroup(disabled);
            if (GUI.Button(__rect[5], __content[5]))
            {
                _saveCreated = true;
                closeDialog = true;                
            }
            EditorGUI.EndDisabledGroup();

            if (GUI.Button(__rect[6], __content[6]))
            {
                _saveCreated = false;
                closeDialog = true;
            }

            if (closeDialog)
            {
                Close();
            }
        }
        #endregion
    }

}
