// <copyright file="CreatePuppetDialoge.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

using V4F.Puppets;

namespace V4F
{

    [InitializeOnLoad]
    public class PuppetDialog : EditorWindow
    {
        public delegate void DialogEventHandler(PuppetDialog sender, PuppetEventArgs args);

        private delegate void DragAndDropEventHandler(Vector3 mousePosition, bool dragPerform);

        #region Fields
        private static readonly GUIContent[]    __content = null;        
        private static readonly Rect[]          __rect = null;
        private static readonly Color[]         __colour = null;
        private static readonly GUIStyle[]      __styles = null;

        public static event DialogEventHandler OnCreate;
        public static event DialogEventHandler OnEdit;

        private event DragAndDropEventHandler _DragAndDrop;

        private PuppetEventArgs _args = null;        
        private bool _editMode = false;
        private int[] _colourIndices = null;                
        private bool _saved = false;
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
        static PuppetDialog()
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
                GUIContent.none,
                new GUIContent("Editing Personage"),
                new GUIContent("Icon"),
                new GUIContent("Save"),
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
        private static void ShowCreateDialog()
        {            
            var dialog = CreateInstance<PuppetDialog>();            
            dialog.titleContent = __content[0];

            var dialogRect = __rect[0];
            dialog.minSize = dialogRect.size;
            dialog.maxSize = dialogRect.size;

            var args = new PuppetEventArgs();
            args.puppet = null;
            args.path = AssetDatabase.GetAssetPath(Selection.activeObject);

            dialog._args = args;            
            dialog._editMode = false;

            dialog.ShowUtility();
        }

        [MenuItem("CONTEXT/Puppet/Edit Data", false)]
        private static void ShowEditDialog(MenuCommand menuCommand)
        {
            var dialog = CreateInstance<PuppetDialog>();
            dialog.titleContent = __content[14];

            var dialogRect = __rect[0];
            dialog.minSize = dialogRect.size;
            dialog.maxSize = dialogRect.size;


            var puppet = menuCommand.context as Puppet;
            var args = new PuppetEventArgs();
            args.puppet = puppet;
            args.path = AssetDatabase.GetAssetPath(Selection.activeObject);
            args.spec = puppet.spec;
            args.skillSet = puppet.skillSet;
            args.properName = puppet.properName;
            args.icon = puppet.icon;
            args.prefab = puppet.prefab;

            dialog._args = args;
            dialog._editMode = true;

            dialog.ShowUtility();
        }        

        private void SpecDropArea(Vector3 mousePosition, bool dragPerform)
        {
            var index = 1;

            if (__rect[1].Contains(mousePosition))
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
                    if (dragPerform)
                    {
                        DragAndDrop.AcceptDrag();                        
                        _args.spec = spec;                        
                    }
                    else
                    {
                        index = 2;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;                    
                }                
            }

            _colourIndices[1] = index;
        }

        private void SkillSetDropArea(Vector3 mousePosition, bool dragPerform)
        {
            var index = 1;

            if (__rect[3].Contains(mousePosition))
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
                    if (dragPerform)
                    {
                        DragAndDrop.AcceptDrag();                        
                        _args.skillSet = skillSet;                        
                    }
                    else
                    {
                        index = 2;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;                    
                }                
            }

            _colourIndices[3] = index;
        }

        private void IconDropArea(Vector3 mousePosition, bool dragPerform)
        {            
            if (__rect[7].Contains(mousePosition))
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
                    if (dragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        _args.icon = sprite;
                    }
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }             
            }            
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
                if (_DragAndDrop != null)
                {
                    _DragAndDrop(currentEvent.mousePosition, (eventType == EventType.DragPerform));
                }
            }            
        }

        private void OnCreateCallback(PuppetEventArgs args)
        {
            if (OnCreate != null)
            {
                OnCreate(this, args);
            }
        }

        private void OnEditCallback(PuppetEventArgs args)
        {
            if (OnEdit != null)
            {
                OnEdit(this, args);
            }
        }

        private void DrawSpec()
        {
            var spec = _args.spec;

            if (spec != null)
            {
                var op1 = GUILayout.Width(96f);
                var op2 = new GUILayoutOption[]
                {
                    GUILayout.Width(EditorGUIUtility.singleLineHeight),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight),
                };

                GUILayout.BeginArea(__rect[1]);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(__content[1], EditorStyles.boldLabel, op1);

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(__content[8], op2))
                        {
                            Selection.activeObject = spec;
                            EditorGUIUtility.PingObject(spec);
                        }

                        if (GUILayout.Button(__content[9], op2))
                        {
                            spec = null;
                        }
                    }                    
                    EditorGUILayout.EndHorizontal();

                    if (spec != null)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
                        {
                            EditorGUILayout.LabelField("Health Points", spec.GetStat(PuppetStats.HealthPoints).ToString());
                            EditorGUILayout.LabelField("Accuracy", spec.GetStat(PuppetStats.Accuracy).ToString());
                            EditorGUILayout.LabelField("Initiative", spec.GetStat(PuppetStats.Initiative).ToString());
                            EditorGUILayout.LabelField("Stamina", spec.GetStat(PuppetStats.Stamina).ToString());
                            EditorGUILayout.LabelField("Damage", string.Format("{0}-{1}", spec.GetStat(PuppetStats.MinDamage), spec.GetStat(PuppetStats.MaxDamage)));
                            EditorGUILayout.LabelField("Critical Chance", string.Format("{0}%", spec.GetStat(PuppetStats.CriticalChance)));
                            EditorGUILayout.LabelField("Critical Damage", string.Format("{0}%", spec.GetStat(PuppetStats.CriticalDamage)));
                        }                        
                        EditorGUILayout.EndVertical();
                    }
                }                
                GUILayout.EndArea();
            }
            else
            {
                EditorGUI.DrawRect(__rect[1], __colour[_colourIndices[1]]);
                EditorGUI.LabelField(__rect[1], __content[1], CustomStyles.infoDrop);
            }

            _args.spec = spec;
        }

        private void DrawResistances()
        {
            EditorGUI.DrawRect(__rect[2], __colour[_colourIndices[2]]);
            EditorGUI.LabelField(__rect[2], __content[2], CustomStyles.infoDrop);
        }

        private void DrawSkillSet()
        {
            var skillSet = _args.skillSet;
            var selected = -1;

            if (skillSet != null)
            {
                var op1 = GUILayout.Width(96f);
                var op2 = GUILayout.ExpandWidth(true);
                var op3 = new GUILayoutOption[]
                {
                    GUILayout.Width(EditorGUIUtility.singleLineHeight),
                    GUILayout.Height(EditorGUIUtility.singleLineHeight),
                };

                GUILayout.BeginArea(__rect[3]);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(__content[3], EditorStyles.boldLabel, op1);

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button(__content[8], op3))
                        {
                            Selection.activeObject = skillSet;
                            EditorGUIUtility.PingObject(skillSet);
                        }

                        if (GUILayout.Button(__content[9], op3))
                        {
                            skillSet = null;
                        }
                    }                    
                    EditorGUILayout.EndHorizontal();

                    if (skillSet != null)
                    {
                        var countSkills = skillSet.countSkills;
                        var skillSetContent = new GUIContent[countSkills];
                        for (int i = 0; i < countSkills; ++i)
                        {
                            var skill = skillSet[i];
                            var texture = ((skill.icon != null) ? skill.icon.texture : null);
                            skillSetContent[i] = new GUIContent(skill.title, texture);
                        }

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();

                                selected = GUILayout.SelectionGrid(selected, skillSetContent, countSkills, skillSetStyle, op2);

                                GUILayout.FlexibleSpace();
                            }                            
                            EditorGUILayout.EndHorizontal();
                        }                        
                        EditorGUILayout.EndVertical();

                        if (selected != -1)
                        {
                            var skill = skillSet[selected];
                            Selection.activeObject = skill;
                            EditorGUIUtility.PingObject(skill);
                        }
                    }
                }
                GUILayout.EndArea();
            }
            else
            {
                EditorGUI.DrawRect(__rect[3], __colour[_colourIndices[3]]);
                EditorGUI.LabelField(__rect[3], __content[3], CustomStyles.infoDrop);
            }

            _args.skillSet = skillSet;
        }

        private void DrawIcon()
        {
            var icon = _args.icon;

            GUILayout.BeginArea(__rect[7], EditorStyles.helpBox);
            {
                if (icon == null)
                {
                    var op = GUILayout.ExpandHeight(true);
                    EditorGUILayout.LabelField(__content[15], CustomStyles.infoDrop, op);
                }
            }            
            GUILayout.EndArea();

            if (icon != null)
            {
                EditorGUI.DrawTextureTransparent(__rect[8], icon.texture, ScaleMode.StretchToFill);
            }

            _args.icon = icon;
        }

        private void DrawCustomData()
        {
            GUILayout.BeginArea(__rect[9]);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[11], CustomStyles.italicLabel);

                    var properName = Regex.Replace(EditorGUILayout.DelayedTextField(_args.properName), @"[^a-zA-Z0-9_ ]", "").Trim();
                    if (!string.IsNullOrEmpty(properName) && (properName.Length > 3))
                    {
                        _args.properName = properName;
                    }

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[12], CustomStyles.italicLabel);

                    _args.prefab = EditorGUILayout.ObjectField(_args.prefab, typeof(GameObject), false) as GameObject;
                }                
                EditorGUILayout.EndVertical();
            }            
            GUILayout.EndArea();
        }

        private bool ValidateConfirm()
        {
            var confirm = (!string.IsNullOrEmpty(_args.properName) && (_args.properName.Length > 3));
            return (confirm && !(
                (_args.spec == null) ||
                (_args.skillSet == null) ||
                (_args.icon == null) ||
                (_args.prefab == null)
            ));
        }

        private void OnEnable()
        {
            _DragAndDrop += SpecDropArea;
            _DragAndDrop += SkillSetDropArea;
            _DragAndDrop += IconDropArea;

            _colourIndices = new int[] { 0, 1, 0, 1, 0, 0, 0, 1 };
            _saved = false;
        }

        private void OnDisable()
        {
            _DragAndDrop -= IconDropArea;
            _DragAndDrop -= SkillSetDropArea;
            _DragAndDrop -= SpecDropArea;            

            if (_saved)
            {
                if (_editMode)
                {
                    OnEditCallback(_args);
                }
                else
                {
                    OnCreateCallback(_args);
                }
            }            
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {            
            DrawSpec();
            DrawResistances();
            DrawSkillSet();            

            // Separator
            EditorGUI.DrawRect(__rect[4], __colour[0]);

            DrawIcon();
            DrawCustomData();

            // Events
            EventsHandler();

            var disabled = !ValidateConfirm();
            var closeAndSave = false;

            EditorGUI.BeginDisabledGroup(disabled);
            {
                var index = (_editMode ? 16 : 5);
                closeAndSave = GUI.Button(__rect[5], __content[index]);
            }            
            EditorGUI.EndDisabledGroup();

            if (closeAndSave)
            {
                _saved = true;
                Close();
            }

            if (GUI.Button(__rect[6], __content[6]))
            {
                Close();
            }            
        }
        #endregion
    }

}
