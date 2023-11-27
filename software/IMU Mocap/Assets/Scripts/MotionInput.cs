using System;
using UnityEditor;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MotionInput : MonoBehaviour {
    public BLE bluetooth;

    public TextAsset inputDataFile;

    public Orientate orientateUpperChest;
    public Orientate orientateLeftLowerArm;
    public Orientate orientateRightLowerArm;
    public Orientate orientateLeftUpperArm;
    public Orientate orientateRightUpperArm;

    private int counter;

    private string[] inputDataString;

    void Start() {
        counter = 0;
        inputDataString = inputDataFile.text.Split('\n');
    }

    void Update() {
        //Debug.Log(bluetooth.receivedInput[0]);

        float x, y, z, w;

        string line = inputDataString[counter];

        string[] variables = line.Split(',');
        x = (float)Convert.ToDouble(variables[0]);
        y = (float)Convert.ToDouble(variables[1]);
        z = (float)Convert.ToDouble(variables[2]);
        w = (float)Convert.ToDouble(variables[3]);

        orientateUpperChest.SetGlobalOrientation(new Quaternion((float)x, (float)0, (float)z, (float)w));
        orientateLeftLowerArm.SetGlobalOrientation(new Quaternion((float)x, (float)y, (float)z, (float)w));
        orientateLeftUpperArm.SetGlobalOrientation(new Quaternion((float)x, (float)(0.5 * y), (float)z, (float)w));
        orientateRightLowerArm.SetGlobalOrientation(new Quaternion((float)x, (float)-y, (float)-z, (float)w));
        orientateRightUpperArm.SetGlobalOrientation(new Quaternion((float)x, (float)(0.5 * -y), (float)-z, (float)w));
        
        if (counter < 70) {
            counter += 1;
        }
        else {
            counter = 0;
        }
    }
}