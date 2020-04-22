using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDemo : MonoBehaviour
{
    public   Vector3 RotationAngle;
 
    public   Vector3 HidePropety;
    public   Vector3 HidePropety2;
    public   Vector3 HidePropety3;
    public   Vector3 HidePropety4;
    public   Vector3 HidePropety5;
    public   Vector3 HidePropety51;
    public   Vector3 HidePropety55;
    public   Vector3 HidePropety58;
    public float RotationSpeed;
    void Start()
    {
        
    }

   
    void Update()
    {
        this.transform.Rotate(RotationAngle);
    }
}
