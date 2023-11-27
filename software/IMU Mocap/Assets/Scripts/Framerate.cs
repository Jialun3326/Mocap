using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Framerate : MonoBehaviour {
    public int targetFrameRate = 30;

    void Start() {
        QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = targetFrameRate;
    }
}