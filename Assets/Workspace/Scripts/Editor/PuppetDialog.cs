// <copyright file="CreatePuppetDialoge.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

using V4F.Character;

namespace V4F
{

    [InitializeOnLoad]
    public class PuppetDialog : EditorWindow
    {
        #region Types
        public delegate void DialogEventHandler(PuppetDialog sender, PuppetEventArgs args);
        private delegate void DragAndDropEventHandler(Vector3 mousePosition, bool dragPerform);
        #endregion

        #region Fields
        private static readonly GUIContent[] __content = null;
        private static readonly Rect[] __rect = null;
        private static readonly Color[] __colour = null;
        private static readonly GUIStyle[] __styles = null;
        private static readonly GUIContent[] __table = null;

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
                new GUIContent("Create Puppet"),
                new GUIContent("Specification"),
                new GUIContent("Resistances"),
                new GUIContent("Skillset"),
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
                new GUIContent("Editing Puppet"),
                new GUIContent("Icon"),
                new GUIContent("Save"),
                GUIContent.none,
                new GUIContent("Class:"),
                new GUIContent("Prefab UI:"),
            };

            __rect = new Rect[]
            {
                new Rect(0f, 0f, 597, 494f),
                new Rect(8f, 8f, 404f, 363f),
                new Rect(0f, 0f, 0f, 0f),
                new Rect(8f, 378f, 404f, 108f),
                new Rect(420f, 0f, 1f, 494f),
                new Rect(461f, 434f, 96f, 24f),
                new Rect(461f, 462f, 96f, 24f),
                new Rect(461f, 8f, 96f, 96f),
                new Rect(463f, 10f, 92f, 92f),
                new Rect(429f, 108f, 160f, 200f),
            };

            __colour = new Color[]
            {
                new Color(0.2f, 0.2f, 0.2f, 1f),
                new Color(0.5f, 0.5f, 0.5f, 0.25f),
                new Color(0f, 0.75f, 0f, 0.25f),

                new Color32(41, 128, 185, 255),
                new Color32(236, 240, 241, 255),
                new Color32(189, 195, 199, 255),
            };

            __styles = new GUIStyle[5];

            __table = new GUIContent[]
            {
                new GUIContent("Strength"),
                new GUIContent("Dexterity"),
                new GUIContent("Magic"),
                new GUIContent("Vitality"),
                new GUIContent("Health points"),
                new GUIContent("Damage (melee)"),
                new GUIContent("Damage (range)"),
                new GUIContent("Damage (magic)"),
                new GUIContent("Chance to dodge"),
                new GUIContent("Chance to crit"),
                new GUIContent("Resistance (Fire) "),
                new GUIContent("Resistance (Ice)"),
                new GUIContent("Resistance (Lighting)"),
                new GUIContent("Resistance (Death)"),
                new GUIContent("Resistance (Poison)"),
                new GUIContent("Resistance (Stun/Move)"),
                new GUIContent("Resistance (Immune)"),
                new GUIContent("Resistance (Bleeding)"),
            };
        }
        #endregion

        #region Methods                
        public static void ShowCreateDialog()
        {
            var dialog = CreateInstance<PuppetDialog>();
            dialog.titleContent = __content[0];
            dialog.minSize = __rect[0].size;
            dialog.maxSize = __rect[0].size;

            var args = new PuppetEventArgs();            
            args.path = AssetDatabase.GetAssetPath(Selection.activeObject);
            args.puppet = null;

            dialog._args = args;            
            dialog._editMode = false;

            dialog.ShowUtility();
        }

        public static void ShowEditDialog(Puppet puppet)
        {
            var dialog = CreateInstance<PuppetDialog>();
            dialog.titleContent = __content[14];
            dialog.minSize = __rect[0].size;
            dialog.maxSize = __rect[0].size;

            var args = new PuppetEventArgs();
            args.path = AssetDatabase.GetAssetPath(Selection.activeObject);
            args.puppet = puppet;
            args.spec = puppet.spec;
            args.skillSet = puppet.skillSet;
            args.icon = puppet.icon;
            args.charClass = puppet.charClass;
            args.properName = puppet.properName;
            args.prefab = puppet.prefab;
            args.prefabUI = puppet.prefabUI;

            dialog._args = args;
            dialog._editMode = true;

            dialog.ShowUtility();
        }

        private void SpecDropArea(Vector3 mousePosition, bool dragPerform)
        {
            var index = 1;

            if (__rect[1].Contains(mousePosition))
            {
                Specification spec = null;

                foreach (Object draggedObject in DragAndDrop.objectReferences)
                {
                    spec = draggedObject as Specification;
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
                SkillSet skillSet = null;

                foreach (Object draggedObject in DragAndDrop.objectReferences)
                {
                    skillSet = draggedObject as SkillSet;
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

        private void DrawSpecTable(Rect rect)
        {
            var cellHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var cellWidth = new float[] { rect.width * 0.4f, rect.width * 0.3f, rect.width * 0.3f };            

            var rowCount = 18;
            var rectColumn1 = new Rect[rowCount];
            var rectColumn2 = new Rect[rowCount];
            var rectColumn3 = new Rect[rowCount];

            var rcHeader = new Rect(rect.x, rect.y, rect.width, cellHeight);
            EditorGUI.DrawRect(rcHeader, __colour[3]);

            var rcRowBackground = new Rect(rect.x, rect.y, rect.width, cellHeight);
            for (var i = 0; i < rowCount; ++i)
            {
                rcRowBackground.y = rect.y + cellHeight * (i + 1);
                EditorGUI.DrawRect(rcRowBackground, __colour[4 + (i % 2)]);

                rectColumn1[i].Set(rcRowBackground.x + 4f, rcRowBackground.y, cellWidth[0] - 8f, cellHeight);
                rectColumn2[i].Set(rcRowBackground.x + cellWidth[0] + 5f, rcRowBackground.y, cellWidth[1] - 8f, cellHeight);
                rectColumn3[i].Set(rcRowBackground.x + cellWidth[0] + cellWidth[1] + 6f, rcRowBackground.y, cellWidth[2] - 8f, cellHeight);
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Height(cellHeight));
            {
                rcHeader.width = cellWidth[0];
                EditorGUI.LabelField(rcHeader, "Attributes", CustomStyles.tableHeader);
                rcHeader.x = rcHeader.x + cellWidth[0] + 1f;
                rcHeader.width = cellWidth[1];
                EditorGUI.LabelField(rcHeader, "Base", CustomStyles.tableHeader);
                rcHeader.x = rcHeader.x + cellWidth[1] + 1f;
                rcHeader.width = cellWidth[2];
                EditorGUI.LabelField(rcHeader, "Сalculated", CustomStyles.tableHeader);
            }
            EditorGUILayout.EndHorizontal();

            var spec = _args.spec;

            var baseParam = new string[]
            {
                string.Format("{0}", spec.strength),
                string.Format("{0}", spec.dexterity),
                string.Format("{0}", spec.magic),
                string.Format("{0}", spec.vitality),
                string.Format("{0}", spec.healthPoints),
                string.Format("{0} - {1}", spec.minDamageMelee, spec.maxDamageMelee),
                string.Format("{0} - {1}", spec.minDamageRange, spec.maxDamageRange),
                string.Format("{0} - {1}", spec.minDamageMagic, spec.maxDamageMagic),
                string.Format("{0}%", spec.chanceToDodge),
                string.Format("{0}%", spec.chanceToCrit),
                string.Format("{0}%", spec.fireResistance),
                string.Format("{0}%", spec.iceResistance),
                string.Format("{0}%", spec.lightingResistance),
                string.Format("{0}%", spec.deathResistance),
                string.Format("{0}%", spec.poisonResistance),
                string.Format("{0}%", spec.stunMoveResistance),
                string.Format("{0}%", spec.immuneResistance),
                string.Format("{0}%", spec.bleedingResistance),
            };

            var calcParam = new string[]
            {
                string.Format("{0}", spec.strength),
                string.Format("{0}", spec.dexterity),
                string.Format("{0}", spec.magic),
                string.Format("{0}", spec.vitality),
                string.Format("{0}", spec.CalcHealthPoints()),
                string.Format("{0} - {1}", spec.CalcMinDamageMelee(), spec.CalcMaxDamageMelee()),
                string.Format("{0} - {1}", spec.CalcMinDamageRange(), spec.CalcMaxDamageRange()),
                string.Format("{0} - {1}", spec.CalcMinDamageMagic(), spec.CalcMaxDamageMagic()),
                string.Format("{0}%", spec.chanceToDodge),
                string.Format("{0}%", spec.chanceToCrit),
                string.Format("{0}%", spec.fireResistance),
                string.Format("{0}%", spec.iceResistance),
                string.Format("{0}%", spec.lightingResistance),
                string.Format("{0}%", spec.deathResistance),
                string.Format("{0}%", spec.poisonResistance),
                string.Format("{0}%", spec.stunMoveResistance),
                string.Format("{0}%", spec.immuneResistance),
                string.Format("{0}%", spec.bleedingResistance),
            };

            for (var i = 0; i < rowCount; ++i)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(cellHeight));
                {
                    EditorGUI.LabelField(rectColumn1[i], __table[i], CustomStyles.tableFirstField);
                    EditorGUI.LabelField(rectColumn2[i], baseParam[i], CustomStyles.tableOtherField);
                    EditorGUI.LabelField(rectColumn3[i], calcParam[i], CustomStyles.tableOtherField);
                }
                EditorGUILayout.EndHorizontal();
            }            

            var rcColumn = new Rect(rect.x - 1f, rect.y, 1f, rect.height);
            for (var i = 0; i < (cellWidth.Length - 1); ++i)
            {
                rcColumn.x = rcColumn.x + cellWidth[i] + 1f;
                EditorGUI.DrawRect(rcColumn, __colour[0]);
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
                        var rect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));                        
                        DrawSpecTable(rect);                                                    
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
                            var texture = skill[0].icon.texture;
                            skillSetContent[i] = new GUIContent(skill.name, texture);
                        }

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));
                        {
                            GUILayout.FlexibleSpace();

                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();

                                selected = GUILayout.SelectionGrid(selected, skillSetContent, countSkills, skillSetStyle, op2);

                                GUILayout.FlexibleSpace();
                            }                            
                            EditorGUILayout.EndHorizontal();

                            GUILayout.FlexibleSpace();
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
                    var selectedIndex = Localization.GetKeyIndex(_args.properName);
                    var select = EditorGUILayout.Popup(selectedIndex, Localization.keys);
                    if (select != selectedIndex)
                    {
                        _args.properName = Localization.GetKey(select);
                    }                    

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[18], CustomStyles.italicLabel);
                    _args.charClass = (Classes)EditorGUILayout.EnumPopup(_args.charClass);

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[12], CustomStyles.italicLabel);
                    _args.prefab = EditorGUILayout.ObjectField(_args.prefab, typeof(GameObject), false) as GameObject;

                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(__content[19], CustomStyles.italicLabel);
                    _args.prefabUI = EditorGUILayout.ObjectField(_args.prefabUI, typeof(GameObject), false) as GameObject;
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
            hideFlags = HideFlags.HideAndDontSave;

            _DragAndDrop += SpecDropArea;
            _DragAndDrop += SkillSetDropArea;
            _DragAndDrop += IconDropArea;

            _colourIndices = new int[] { 0, 1, 1, 1, 0, 0, 0, 1 };
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
