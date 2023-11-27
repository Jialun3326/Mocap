using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour {
    void Start() {
    }

    void Update() {
        if (Input.GetKey(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Mocap") {
            SceneManager.LoadScene("Bluetooth");
        }
        else if (Input.GetKey(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Bluetooth"){
            SceneManager.LoadScene("Mocap");
        }
    }
}
