using UnityEngine;
using UnityEditor;

namespace V4F
{

    [InitializeOnLoad]
    public sealed class CustomStyles
    {
        #region Fields
        private static readonly GUIStyle[] __styles = null;
        #endregion

        #region Properties
        public static GUIStyle italicLabel
        {
            get
            {
                if (__styles[0] == null)
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.fontStyle = FontStyle.Italic;

                    __styles[0] = style;
                }
                return __styles[0];
            }
        }

        public static GUIStyle boldFoldout
        {
            get
            {
                if (__styles[1] == null)
                {
                    var style = new GUIStyle(EditorStyles.foldout);
                    style.fontStyle = FontStyle.Bold;

                    __styles[1] = style;
                }
                return __styles[1];
            }
        }

        public static GUIStyle boldItalicFoldout
        {
            get
            {
                if (__styles[3] == null)
                {
                    var style = new GUIStyle(EditorStyles.foldout);
                    style.fontStyle = FontStyle.BoldAndItalic;

                    __styles[3] = style;
                }
                return __styles[3];
            }
        }

        public static GUIStyle italicFoldout
        {
            get
            {
                if (__styles[4] == null)
                {
                    var style = new GUIStyle(EditorStyles.foldout);
                    style.fontStyle = FontStyle.Italic;

                    __styles[4] = style;
                }
                return __styles[4];
            }
        }

        public static GUIStyle infoDrop
        {
            get
            {
                if (__styles[2] == null)
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.normal.textColor = Color.white;

                    __styles[2] = style;
                }
                return __styles[2];
            }
        }

        public static GUIStyle italicToggle
        {
            get
            {
                if (__styles[5] == null)
                {
                    var style = new GUIStyle(EditorStyles.toggle);
                    style.fontStyle = FontStyle.Italic;

                    __styles[5] = style;
                }
                return __styles[5];
            }
        }

        public static GUIStyle tableHeader
        {
            get
            {
                if (__styles[6] == null)
                {
                    var style = new GUIStyle(EditorStyles.boldLabel);                    
                    style.alignment = TextAnchor.MiddleCenter;
                    style.normal.textColor = Color.white;

                    __styles[6] = style;
                }
                return __styles[6];
            }
        }

        public static GUIStyle tableFirstField
        {
            get
            {
                if (__styles[7] == null)
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleLeft;
                    style.fontStyle = FontStyle.Italic;

                    __styles[7] = style;
                }
                return __styles[7];
            }
        }

        public static GUIStyle tableOtherField
        {
            get
            {
                if (__styles[8] == null)
                {
                    var style = new GUIStyle(EditorStyles.label);
                    style.alignment = TextAnchor.MiddleRight;

                    __styles[8] = style;
                }
                return __styles[8];
            }
        }
        #endregion

        #region Constructors
        static CustomStyles()
        {
            __styles = new GUIStyle[12];
        }
        #endregion
    }

}
