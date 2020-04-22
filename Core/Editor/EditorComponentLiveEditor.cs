#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ComponentLiveEditor
{
    [CustomEditor(typeof(ComponentLiveEditor))]
    public class EditorComponentLiveEditor : Editor
    {
        #region fields
        private const string SettingsLabel = "Open Settings";
        ComponentLiveEditor ComponentLiverEditortarget;
        GUIContent contenido = new GUIContent();
        Dictionary<int, MonoBehaviour> MonoBehaviorsList = new Dictionary<int, MonoBehaviour>();
        SerializedProperty MonoBehaviorSerializated;
        string[] MonoBehaviorListLabel ;
        int selectedMonoBehaviorIndex = -1;
        int new_SelectedMonoBehaviorIndex = -1;
        #endregion
        #region Editor
        void OnEnable()
        {
            ComponentLiverEditortarget = target as ComponentLiveEditor;
            contenido.text = "";
            
            MonoBehaviorSerializated = serializedObject.FindProperty("selectedMonoBehavior");
          
            OnInit(ComponentLiverEditortarget.gameObject);
        }
     
        override public void OnInspectorGUI()
        {

            serializedObject.Update();

            new_SelectedMonoBehaviorIndex = EditorGUILayout.Popup("MonoBehavior", selectedMonoBehaviorIndex, MonoBehaviorListLabel);
            if (new_SelectedMonoBehaviorIndex != selectedMonoBehaviorIndex)
            {
                onMonoBehaviorChanged(MonoBehaviorsList[new_SelectedMonoBehaviorIndex]);
                selectedMonoBehaviorIndex = new_SelectedMonoBehaviorIndex;
            }

            if (GUILayout.Button(SettingsLabel))
            {

                Selection.activeObject = ComponentLiveEditorSettings.Load();
            };

            serializedObject.ApplyModifiedProperties();

        }
        #endregion



        #region Internal
        private void OnInit(GameObject newgameObject)
        {


            MonoBehaviour[] monos = newgameObject.GetComponents<MonoBehaviour>();
            MonoBehaviorListLabel = new string[monos.Length];
            for (int i = 0; i < monos.Length; i++)
            {
                var mono = monos[i];
                if (mono ==   ComponentLiverEditortarget.selectedMonoBehavior) {
                    selectedMonoBehaviorIndex = i; 
                }
               
                MonoBehaviorListLabel[i] = mono.GetType().Name;
                MonoBehaviorsList.Add(i, mono);

            }

        }
      
        private void onMonoBehaviorChanged(MonoBehaviour new_monoBehaviour)
        {
            ComponentLiverEditortarget.selectedMonoBehavior = new_monoBehaviour;
        }
        #endregion

    }
}
#endif