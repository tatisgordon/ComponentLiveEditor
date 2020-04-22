using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
/// <summary>
/// 
/// </summary>
namespace ComponentLiveEditor
{
 

    public class ComponentLiveEditor : MonoBehaviour
    {
        #region Fields
        static int UnColapsedWindows;
      
        public MonoBehaviour selectedMonoBehavior;

        public static int CountComponentEditorid;
        public static bool colapsed = true;
        public float F_IncrementByTimePressed = 1;
        public int Int_IncrementByTimePressed = 1;

        private const float minimumTimePerTick = 0.1f;

        private int componentEditorid;
        private float TimeLastTick;

        private ComponentLiveEditorSettings setting;

        private Rect ContentLabel;
        private Rect ContentColapse;
        private Rect ContentGroup;
        private Vector2 scrollPosition;
        bool open = false;
        #endregion

        #region MonoBehavior
        private void Awake()
        {

            componentEditorid = CountComponentEditorid++;
            ContentLabel = new Rect((componentEditorid * 150    ) + 30, 0, 150, 60);
            ContentColapse = new Rect(0, 0, 30, 60);
            ContentGroup = new Rect(0, 60, Screen.width, Screen.height);
            setting = ComponentLiveEditorSettings.Load();
            setting.testGUIskin.box.fixedWidth = Screen.width-15;
        }
        private void OnGUI()
        {

            if (selectedMonoBehavior != null)
            {
                //the first component will hold the Colapse Button
                if (this.componentEditorid == 0)
                {
                    if (GUI.Button(ContentColapse, colapsed ? ">" : "<"))
                    {
                        colapsed = !colapsed;

                    }
                }
                if (!colapsed)
                {
                    
                    if (GUI.Button(ContentLabel, this.selectedMonoBehavior.GetType().Name,
                        UnColapsedWindows == this.componentEditorid ? setting.testGUIskin.customStyles[1] : setting.testGUIskin.customStyles[2])) { OnClick(); 
                    }
                    if (UnColapsedWindows == this.componentEditorid)
                    {
                        GUI.BeginGroup(ContentGroup, "");
                        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width ), GUILayout.Height(Screen.height));
                        Component component = gameObject.GetComponent(selectedMonoBehavior.GetType());
                        GUILayout.BeginHorizontal(setting.testGUIskin.box);
                        GUILayout.Label(component.GetType().Name, setting.testGUIskin.customStyles[3]);
                        GUILayout.EndHorizontal();
                        foreach (var fieldInfo in component.GetType().GetFields())
                        {
                            printProperty(fieldInfo, component);
                        }
                        GUI.EndScrollView();
                        GUI.EndGroup();
                    }
                }

            }


        }
        #endregion


        /// <summary>
        /// when a ComponentButton is Pressed
        /// </summary>
        private void OnClick()
        {
            if (UnColapsedWindows == this.componentEditorid)
            {
                UnColapsedWindows = -1;
            }
            else { UnColapsedWindows = this.componentEditorid; }


        }

        Dictionary<string, PropertyStatus> PropertyStatusDictionary= new Dictionary<string, PropertyStatus>();
        private void printProperty(FieldInfo properyfield, Component serializedObject)
        {
            System.Object obj = (System.Object)serializedObject;
            var key = properyfield.Name;

            if (!PropertyStatusDictionary.ContainsKey(key)) {
                PropertyStatusDictionary.Add(key, new PropertyStatus());
            }
            HandleType(properyfield, serializedObject,    PropertyStatusDictionary[key]);

        }     


        private void HandleType(FieldInfo properyfield, Component component,  PropertyStatus propertyStatus)
        {
            System.Object obj = (System.Object)component;
            Type type = properyfield.FieldType;


            //when no button down Restart de Multiplier
            if (Time.time - TimeLastTick > 0.11) { F_IncrementByTimePressed = 1;Int_IncrementByTimePressed = 1; }

            if (!HandleHideInInspectorAttribute(properyfield,obj)) { return; }
            if (!HandleHeaderAttribute(properyfield,obj)) { return; }
            if (type.IsEnum)
            {
                GUILayout.BeginVertical(setting.testGUIskin.box);

                GUILayout.BeginHorizontal();
  
                if (GUILayout.Button(properyfield.GetValue(obj).ToString(), setting.testGUIskin.button))
                {
                    propertyStatus.Visible = !propertyStatus.Visible;

                };
                GUILayout.EndHorizontal();
                if (propertyStatus.Visible)
                {

                    Array EnumTypesValues = type.GetEnumValues();



                    for (int i = 0; i < EnumTypesValues.Length; i++)
                    {
                        object enumval = EnumTypesValues.GetValue(i);

                        if (GUILayout.Button(enumval.ToString(), setting.testGUIskin.button))
                        {
                            open = !open;
                            properyfield.SetValue(obj, enumval);
                        }
                    }

                }
                GUILayout.EndVertical();
            }
            else if (type == typeof(bool))
            {

             
                GUILayout.BeginHorizontal(setting.testGUIskin.box);

                properyfield.SetValue(obj, GUILayout.Toggle((bool)properyfield.GetValue(obj), properyfield.Name, setting.testGUIskin.toggle));
                GUILayout.EndHorizontal();
            }
            else if (type == typeof(float))
            {
                if (!HandleRangeAttribute(properyfield, obj)) { return; }
                GUILayout.BeginHorizontal(setting.testGUIskin.box);
                GUILayout.Label(properyfield.Name, setting.testGUIskin.label);

                CustomRepeatButton("-10", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) - 10 * F_IncrementByTimePressed); });
                CustomRepeatButton("-1", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) - 1 * F_IncrementByTimePressed); });
                CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) - 0.1f * F_IncrementByTimePressed); });

                GUILayout.Label(((float)properyfield.GetValue(obj)).ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
                CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) + 0.1f * F_IncrementByTimePressed); });
                CustomRepeatButton("1", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) + 1 * F_IncrementByTimePressed); });
                CustomRepeatButton("10", () => { properyfield.SetValue(obj, ((float)properyfield.GetValue(obj)) + 10 * F_IncrementByTimePressed); });


                GUILayout.EndHorizontal();
            }
            else if (type == typeof(int))
            {
             
                GUILayout.BeginHorizontal(setting.testGUIskin.box);
                GUILayout.Label(properyfield.Name, setting.testGUIskin.customStyles[4]);

                CustomRepeatButton("-10", () => { properyfield.SetValue(obj, ((int)properyfield.GetValue(obj)) - 10 * Int_IncrementByTimePressed); });
                CustomRepeatButton("-1", () => { properyfield.SetValue(obj, ((int)properyfield.GetValue(obj)) - 1 * Int_IncrementByTimePressed); });
         

                GUILayout.Label(((int)properyfield.GetValue(obj)).ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
              
                CustomRepeatButton("1", () => { properyfield.SetValue(obj, ((int)properyfield.GetValue(obj)) + 1 * Int_IncrementByTimePressed); });
                CustomRepeatButton("10", () => { properyfield.SetValue(obj, ((int)properyfield.GetValue(obj)) + 10 * Int_IncrementByTimePressed); });


                GUILayout.EndHorizontal();
            }
            else if (type == typeof(Vector2))
            {
                GUILayout.BeginVertical(setting.testGUIskin.box);

                if (GUILayout.Button(properyfield.Name, setting.testGUIskin.customStyles[0]))
                {
                    propertyStatus.Visible = !propertyStatus.Visible;
                };
                if (propertyStatus.Visible)
                {
                    

               

                    if (propertyStatus.Visible)
                    {



                        Vector2 vector2 = (Vector2)properyfield.GetValue(obj);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("X:", setting.testGUIskin.customStyles[4]);

                        CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2(vector2.x - 1 * F_IncrementByTimePressed, vector2.y)); });
                        CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, new Vector2(vector2.x - 0.1f * F_IncrementByTimePressed, vector2.y)); });
                        GUILayout.Label(vector2.x.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
                        CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, new Vector2(vector2.x + 0.1f * F_IncrementByTimePressed, vector2.y)); });
                        CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector2(vector2.x + 1 * F_IncrementByTimePressed, vector2.y)); });
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        GUILayout.Label("Y:", setting.testGUIskin.customStyles[4]);
                        CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2(vector2.x, vector2.y - 1 * F_IncrementByTimePressed)); });
                        CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, new Vector2(vector2.x, vector2.y - 0.1f * F_IncrementByTimePressed)); });
                        GUILayout.Label(vector2.y.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
                        CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, new Vector2(vector2.x, vector2.y + 0.1f * F_IncrementByTimePressed)); });
                        CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector2(vector2.x, vector2.y + 1 * F_IncrementByTimePressed)); });
                        GUILayout.EndHorizontal();

                    }
                 

                }
                GUILayout.EndVertical();
            }
            else if (type == typeof(Vector3))
            {
                GUILayout.BeginVertical(setting.testGUIskin.box);
               
                if (GUILayout.Button(properyfield.Name, setting.testGUIskin.customStyles[0]))
                {
                    propertyStatus.Visible = !propertyStatus.Visible;
                };
                if (propertyStatus.Visible) {
                    GUILayout.BeginHorizontal();

                    Vector3 vector2 = (Vector3)properyfield.GetValue(obj);
                    GUILayout.Label("X:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3(vector2.x - 1 * F_IncrementByTimePressed, vector2.y, vector2.z)); });
                    CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x - 0.1f * F_IncrementByTimePressed, vector2.y, vector2.z)); });
                    GUILayout.Label(vector2.x.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x + 0.1f * F_IncrementByTimePressed, vector2.y, vector2.z)); });
                    CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector3(vector2.x + 1 * F_IncrementByTimePressed, vector2.y, vector2.z)); });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Y:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y - 1 * F_IncrementByTimePressed, vector2.z)); });
                    CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y - 0.1f * F_IncrementByTimePressed, vector2.z)); });
                    GUILayout.Label(vector2.y.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y + 0.1f * F_IncrementByTimePressed, vector2.z)); });
                    CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y + 1 * F_IncrementByTimePressed, vector2.z)); });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Z:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y, vector2.z - 1 * F_IncrementByTimePressed)); });
                    CustomRepeatButton("-0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y, vector2.z - 0.1f * F_IncrementByTimePressed)); });
                    GUILayout.Label(vector2.z.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);

                    CustomRepeatButton("0.1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y, vector2.z + 0.1f * F_IncrementByTimePressed)); });
                    CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector3(vector2.x, vector2.y, vector2.z + 1 * F_IncrementByTimePressed)); });
                    GUILayout.EndHorizontal();
                }


                GUILayout.EndVertical();
             
            }
            else if (type == typeof(Vector3Int))
            {
                GUILayout.BeginVertical(setting.testGUIskin.box);
                if (GUILayout.Button(properyfield.Name, setting.testGUIskin.button))
                {
                    propertyStatus.Visible = !propertyStatus.Visible;
                };
                if (propertyStatus.Visible)
                {
                    GUILayout.BeginHorizontal();

                    Vector3Int vector2 = (Vector3Int)properyfield.GetValue(obj);
                    GUILayout.Label("X:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x - 1 * Int_IncrementByTimePressed, vector2.y, vector2.z)); });

                    GUILayout.Label(vector2.x.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);

                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x + 1 * Int_IncrementByTimePressed, vector2.y, vector2.z)); });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Y:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x, vector2.y - 1 * Int_IncrementByTimePressed, vector2.z)); });

                    GUILayout.Label(vector2.y.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);

                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x, vector2.y + 1 * Int_IncrementByTimePressed, vector2.z)); });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Z:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x, vector2.y, vector2.z - 1 * Int_IncrementByTimePressed)); });

                    GUILayout.Label(vector2.z.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);


                    CustomRepeatButton("1", () => { properyfield.SetValue(obj, new Vector3Int(vector2.x, vector2.y, vector2.z + 1 * Int_IncrementByTimePressed)); });
                    GUILayout.EndHorizontal();
                }


                GUILayout.EndVertical();

            }
            else if (type == typeof(Vector2Int))
            {
                GUILayout.BeginVertical(setting.testGUIskin.box);
                if (GUILayout.Button(properyfield.Name, setting.testGUIskin.button))
                {
                    propertyStatus.Visible = !propertyStatus.Visible;
                };
                if (propertyStatus.Visible)
                {
                    GUILayout.BeginHorizontal();

                    Vector2Int vector2 = (Vector2Int)properyfield.GetValue(obj);
                    GUILayout.Label("X:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2Int(vector2.x - 1 * Int_IncrementByTimePressed, vector2.y)); });

                    GUILayout.Label(vector2.x.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);

                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2Int(vector2.x + 1 * Int_IncrementByTimePressed, vector2.y)); });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();

                    GUILayout.Label("Y:", setting.testGUIskin.customStyles[4]);
                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2Int(vector2.x, vector2.y - 1 * Int_IncrementByTimePressed)); });

                    GUILayout.Label(vector2.y.ToString(setting.DecimalFormat), setting.testGUIskin.customStyles[4]);

                    CustomRepeatButton("-1", () => { properyfield.SetValue(obj, new Vector2Int(vector2.x, vector2.y + 1 * Int_IncrementByTimePressed)); });
                    GUILayout.EndHorizontal();

                }

                GUILayout.EndVertical();

            }
            else
            {

                if (setting.ShowUnsupported) GUILayout.Label("Unsuported:" + properyfield.FieldType.ToString(), setting.testGUIskin.customStyles[4]);

            }







        }
        /// <summary>
        /// print a Gui.Button with ButtonDownSupport Support
        /// </summary>
        /// <param name="label"> button label </param>
        /// <param name="onPressed"> action when presed</param>
        private void CustomRepeatButton(  string label,Action onPressed)
        {
            if (GUILayout.RepeatButton(label, setting.testGUIskin.button))
            {
                if (Time.time - TimeLastTick > minimumTimePerTick) {
                    onPressed();
                    F_IncrementByTimePressed += minimumTimePerTick;
                    Int_IncrementByTimePressed += 1;
                    TimeLastTick = Time.time; }


            }
        }
        #region AttributesHandlers
        //return true if   interrupt the render
   
        private bool HandleRangeAttribute(FieldInfo properyfield, object obj )
        { 

            RangeAttribute RangeAtt = properyfield.GetCustomAttribute<RangeAttribute>();
            if (RangeAtt != null)
            {


                GUILayout.BeginVertical(setting.testGUIskin.box);
                GUILayout.BeginHorizontal();
                GUILayout.Label(properyfield.Name, setting.testGUIskin.customStyles[4]);
                GUILayout.Label(" [", setting.testGUIskin.customStyles[4]);

                GUILayout.Label(properyfield.GetValue(obj).ToString(), setting.testGUIskin.customStyles[4]);

                GUILayout.Label("]", setting.testGUIskin.customStyles[4]);

                GUILayout.EndHorizontal();

                properyfield.SetValue(obj, GUILayout.HorizontalSlider((float)properyfield.GetValue(obj), RangeAtt.min, RangeAtt.max, slider: setting.testGUIskin.horizontalSlider, thumb: setting.testGUIskin.horizontalSliderThumb));
                GUILayout.EndVertical();

                return false;
            }
            return true;

        }
        private bool HandleHeaderAttribute(FieldInfo properyfield, object obj )
        {


            HeaderAttribute headerAtt = properyfield.GetCustomAttribute<HeaderAttribute>();
            if (headerAtt != null) {



                GUILayout.BeginHorizontal(setting.testGUIskin.box);               
                GUILayout.Label(headerAtt.header, setting.testGUIskin.customStyles[3]);
                GUILayout.EndHorizontal();

            }
            return true;



        }
        private bool HandleHideInInspectorAttribute(FieldInfo properyfield, object obj )
        {
            
            HideInInspector HideAttribute = properyfield.GetCustomAttribute<HideInInspector>();
            if (HideAttribute != null) { return false; }
            return true;

        }
        #endregion
    }
}