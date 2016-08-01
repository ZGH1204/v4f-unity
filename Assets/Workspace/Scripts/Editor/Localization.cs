// <copyright file="Localization.cs" company="Maxim Mikulski">Copyright (c) 2016 All Rights Reserved</copyright>
// <author>Maxim Mikulski</author>

using System.Collections.Generic;
using System.Text;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace V4F
{

    [InitializeOnLoad]
    public sealed class Localization
    {
        #region Constants
        private const string _MSGID = "msgid \"#";
        private const string _BADID = "#BADKEY";
        #endregion

        #region Fields
        private static readonly Localization __instance = null;

        private Dictionary<string, int> _keysIndices = null;
        private string[] _keys = null;
        #endregion

        #region Properties
        public static Localization singleton
        {
            get { return __instance; }
        }

        public static string[] keys
        {
            get { return __instance._keys; }
        }
        #endregion

        #region Indexer
        public int this[string key]
        {
            get
            {
                var index = -1;
                if (_keysIndices.TryGetValue(key, out index))
                {
                    return index;
                }
                return -1;
            }            
        }

        public string this[int index]
        {
            get
            {                
                if ((_keys.Length > 0) && (index == Mathf.Clamp(index, 0, _keys.Length - 1)))
                {
                    return _keys[index];
                }
                return _BADID;
            }
        }
        #endregion

        #region Constructors
        static Localization()
        {
            __instance = new Localization();
            __instance.LoadDefaultFile();

            Debug.Log("Localization()");
        }

        private Localization()
        {
            _keys = new string[0];
        }
        #endregion

        #region Methods
        public static int GetKeyIndex(string key)
        {            
            return __instance[key];
        }

        public static string GetKey(int index)
        {
            return __instance[index];
        }

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            Debug.Log("Localization.Initialize()");
        }

        [DidReloadScripts]
        private static void OnReloadScript()
        {
            Debug.Log("Localization.OnReloadScript()");
        }

        [MenuItem("V4F/Localization/Reload", false, 100)]
        private static void OnReload()
        {
            __instance.LoadDefaultFile();
        }

        private void LoadDefaultFile()
        {            
            try
            {                
                var localizationPath = "Assets/Workspace/Localization/Default.pot";
                var localizationFile = new StreamReader(localizationPath, Encoding.UTF8);

                using (localizationFile)
                {
                    var keys = new List<string>(512);
                    var line = localizationFile.ReadLine();

                    while (line != null)
                    {
                        if (line.StartsWith(_MSGID))
                        {
                            var key = line.Substring(8, (line.Length - 9));
                            keys.Add(key);
                        }                        
                        line = localizationFile.ReadLine();
                    }
                                        
                    localizationFile.Close();
                    if (keys.Count > 0)
                    {                        
                        _keys = keys.ToArray();
                        _keysIndices = new Dictionary<string, int>(_keys.Length);
                        for (int i = 0; i < _keys.Length; ++i)
                        {
                            _keysIndices.Add(_keys[i], i);
                        }
                    }
                }
            }   
                     
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("Localization.LoadDefaultFile(): {0}", e.Message);
            }                        
        }
        #endregion
    }

}
