using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour {
    public GameObject body;
    
    public Orientate orientateUpperChest;
    public Orientate orientateLeftLowerArm;
    public Orientate orientateRightLowerArm;
    public Orientate orientateLeftUpperArm;
    public Orientate orientateRightUpperArm;

    private RigBone upperChest;
    private RigBone leftLowerArm;
    private RigBone rightLowerArm;
    private RigBone leftUpperArm;
    private RigBone rightUpperArm;

    void Start() {
        upperChest = new RigBone(body, HumanBodyBones.UpperChest);
        leftLowerArm = new RigBone(body, HumanBodyBones.LeftLowerArm);
        rightLowerArm = new RigBone(body, HumanBodyBones.RightLowerArm);
        leftUpperArm = new RigBone(body, HumanBodyBones.LeftUpperArm);
        rightUpperArm = new RigBone(body, HumanBodyBones.RightUpperArm);
    }

    void Update() {
        if (Input.GetKey(KeyCode.R)) {
            orientateUpperChest.SetReference();
            orientateLeftLowerArm.SetReference();
            orientateRightLowerArm.SetReference();
            orientateLeftUpperArm.SetReference();
            orientateRightUpperArm.SetReference();
        }

        upperChest.SetLocalRotation(orientateUpperChest.GetOrientation());

        leftLowerArm.SetLocalRotation(Quaternion.Inverse(orientateLeftUpperArm.GetOrientation()) * orientateLeftLowerArm.GetOrientation());
        leftUpperArm.SetLocalRotation(orientateLeftUpperArm.GetOrientation());

        rightLowerArm.SetLocalRotation(Quaternion.Inverse(orientateRightUpperArm.GetOrientation()) * orientateRightLowerArm.GetOrientation());
        rightUpperArm.SetLocalRotation(orientateRightUpperArm.GetOrientation());
    }
}
