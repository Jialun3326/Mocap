using TMPro;
using UnityEngine;

public class DataDisplay : MonoBehaviour
{
    public TMP_Text receivedInputText;

    void Update()
    {
        Debug.Log("CounterDisplay Update: " + BLE.counter);
        receivedInputText.text = BLE.counter.ToString();
    }
}
