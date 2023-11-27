using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

public class BluetoothScene : MonoBehaviour
{
    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("Scene 1");
    }
}
