using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orientate : MonoBehaviour {
    private Quaternion orientation = Quaternion.identity;
    private Quaternion reference_quaternion = Quaternion.identity;

    public void SetReference() {
        reference_quaternion = orientation;
    }

    public void SetGlobalOrientation(Quaternion quaternion) {
        orientation = ConvertToUnity(quaternion);
    }

    public Quaternion GetOrientation() {
        return Quaternion.Inverse(reference_quaternion) * orientation;
    }

    private Quaternion ConvertToUnity(Quaternion input) {
        return new Quaternion(input.z, input.x, input.y, input.w);
    }
}
