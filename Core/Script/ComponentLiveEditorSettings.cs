using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ComponentLiveEditor
{



    public class ComponentLiveEditorSettings : ScriptableObject
    {
       
        public GUISkin testGUIskin;         
        private static ComponentLiveEditorSettings Instance;

        [Header("Misc")]
        public bool ShowUnsupported = false;
        /// <summary>
        /// output format for float numbers
        /// </summary>
        public string DecimalFormat = "0.##";
        /// <summary>
        /// load the Settings ScriptableObject
        /// </summary>
        /// <returns></returns>
        public static ComponentLiveEditorSettings Load()
        {

            if (Instance == null)
            {
                var t = Resources.Load<ComponentLiveEditorSettings>("ComponentLiveEditorSettings");

                if (t == null)
                {
                    Debug.Log("Could not find ComponentLiveEditorSettings asset. Will use default settings instead.");

                    Instance = ScriptableObject.CreateInstance<ComponentLiveEditorSettings>();
                }
                else
                {
                    Instance = t;

                }
            }
            return Instance;
        }
    }
}
