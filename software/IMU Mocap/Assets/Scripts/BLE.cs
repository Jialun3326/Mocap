using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BLE : MonoBehaviour {
    public Button scanButton;
    public TMP_Text scanButtonText;
    public TMP_Text scanStatusText;
    public GameObject scanResult;
    public Button connectButton;
    public TMP_Text connectButtonText;
    public TMP_Text receivedInputText;
    public TMP_Text receivedInputText1;
    public TMP_Text receivedInputText2;
    public TMP_Text receivedInputText3;
    public TMP_Text receivedInputText4;
    public List<Int16> receivedInput = new List<Int16>();
    public List<Int16> receivedInput1 = new List<Int16>();
    public List<Int16> receivedInput2 = new List<Int16>();
    public List<Int16> receivedInput3 = new List<Int16>();
    public List<Int16> receivedInput4 = new List<Int16>();
    public Button writeButton;
    public TMP_InputField writeInputText;

    private bool isScanningDevices = false;
    private bool isConnected = false;
    private Transform scanResultRoot;
    private List<string> selectedDeviceIds = new List<string>();
    private string selectedServiceId = "{00000000-0000-0000-0000-000000000000}";
    private string selectedCharacteristicId = "{00000000-0000-0000-0000-000000000000}";
    private Dictionary<string, Dictionary<string, string>> devices = new Dictionary<string, Dictionary<string, string>>();
    public static int counter = 0;
    private static bool staticIsConnected = false;

    void Start() {
        DontDestroyOnLoad(gameObject);
        scanResultRoot = scanResult.transform.parent;
        scanResult.transform.SetParent(null);

        isConnected = staticIsConnected;
        UpdateConnectButtonUI();
    }

    void Update() {
        Debug.Log("BLE Update:" + counter);
        BLEApi.ScanStatus status;
        if (isScanningDevices) {
            BLEApi.DeviceUpdate res = new BLEApi.DeviceUpdate();
            do {
                status = BLEApi.PollDevice(ref res, false);
                if (status == BLEApi.ScanStatus.AVAILABLE) {
                    if (!devices.ContainsKey(res.id)) {
                        devices[res.id] = new Dictionary<string, string>() {
                        { "name", "" },
                        { "isConnectable", "False" },
                        { "isListed", "False" }
                        };
                    }
                    if (res.nameUpdated) {
                        devices[res.id]["name"] = res.name;
                    }
                    if (res.isConnectableUpdated) {
                        devices[res.id]["isConnectable"] = res.isConnectable.ToString();
                    }
                    if (devices[res.id]["name"] != "" && devices[res.id]["isConnectable"] == "True" && devices[res.id]["isListed"] == "False") {
                        GameObject g = Instantiate(scanResult, scanResultRoot);
                        g.name = res.id;
                        g.transform.GetChild(0).GetComponent<TMP_Text>().text = devices[res.id]["name"];
                        g.transform.GetChild(1).GetComponent<TMP_Text>().text = res.id;
                        devices[res.id]["isListed"] = "True";
                    }
                }
                else if (status == BLEApi.ScanStatus.FINISHED) {
                    isScanningDevices = false;
                    scanStatusText.text = "Finished!";
                }
            } while (status == BLEApi.ScanStatus.AVAILABLE);
        }
        if (isConnected)
        {   
            if (counter == 99) counter = 0;
            counter += 1;
            receivedInputText.text = counter.ToString();
            BLEApi.BLEData res = new BLEApi.BLEData();
            while (BLEApi.PollData(out res, false)) {
                if (res.deviceId == selectedDeviceIds[0]) {
                    receivedInputText.text = BitConverter.ToString(res.buf, 0, res.size);
                    receivedInput.Clear();
                    receivedInput.Add(BitConverter.ToInt16(res.buf, 0));
                    receivedInput.Add((Int16) BitConverter.ToInt16(res.buf, 2));
                    receivedInput.Add((Int16) BitConverter.ToInt16(res.buf, 4));
                    receivedInput.Add((Int16) BitConverter.ToInt16(res.buf, 6));
                    receivedInput.Add((Int16) BitConverter.ToInt16(res.buf, 8));
                    receivedInput.Add((Int16) BitConverter.ToInt16(res.buf, 10));
                }
                else if (res.deviceId == selectedDeviceIds[1]) {
                    receivedInputText1.text = BitConverter.ToString(res.buf, 0, res.size);
                    receivedInput1.Clear();
                    receivedInput1.Add(BitConverter.ToInt16(res.buf, 0));
                    receivedInput1.Add((Int16) BitConverter.ToInt16(res.buf, 2));
                    receivedInput1.Add((Int16) BitConverter.ToInt16(res.buf, 4));
                    receivedInput1.Add((Int16) BitConverter.ToInt16(res.buf, 6));
                    receivedInput1.Add((Int16) BitConverter.ToInt16(res.buf, 8));
                    receivedInput1.Add((Int16) BitConverter.ToInt16(res.buf, 10));
                }
                else if (res.deviceId == selectedDeviceIds[2]) {
                    receivedInputText2.text = BitConverter.ToString(res.buf, 0, res.size);
                    receivedInput2.Clear();
                    receivedInput2.Add(BitConverter.ToInt16(res.buf, 0));
                    receivedInput2.Add((Int16) BitConverter.ToInt16(res.buf, 2));
                    receivedInput2.Add((Int16) BitConverter.ToInt16(res.buf, 4));
                    receivedInput2.Add((Int16) BitConverter.ToInt16(res.buf, 6));
                    receivedInput2.Add((Int16) BitConverter.ToInt16(res.buf, 8));
                    receivedInput2.Add((Int16) BitConverter.ToInt16(res.buf, 10));
                }
                else if (res.deviceId == selectedDeviceIds[3]) {
                    receivedInputText3.text = BitConverter.ToString(res.buf, 0, res.size);
                    receivedInput3.Clear();
                    receivedInput3.Add(BitConverter.ToInt16(res.buf, 0));
                    receivedInput3.Add((Int16) BitConverter.ToInt16(res.buf, 2));
                    receivedInput3.Add((Int16) BitConverter.ToInt16(res.buf, 4));
                    receivedInput3.Add((Int16) BitConverter.ToInt16(res.buf, 6));
                    receivedInput3.Add((Int16) BitConverter.ToInt16(res.buf, 8));
                    receivedInput3.Add((Int16) BitConverter.ToInt16(res.buf, 10));
                }
                else if (res.deviceId == selectedDeviceIds[4]) {
                    receivedInputText4.text = BitConverter.ToString(res.buf, 0, res.size);
                    receivedInput4.Clear();
                    receivedInput4.Add(BitConverter.ToInt16(res.buf, 0));
                    receivedInput4.Add((Int16) BitConverter.ToInt16(res.buf, 2));
                    receivedInput4.Add((Int16) BitConverter.ToInt16(res.buf, 4));
                    receivedInput4.Add((Int16) BitConverter.ToInt16(res.buf, 6));
                    receivedInput4.Add((Int16) BitConverter.ToInt16(res.buf, 8));
                    receivedInput4.Add((Int16) BitConverter.ToInt16(res.buf, 10));
                }
            }
        }
    }

    public void StartStopDeviceScan() {
        if (!isScanningDevices) {
            for (int i = scanResultRoot.childCount - 1; i >= 0; i--) {
                Destroy(scanResultRoot.GetChild(i).gameObject);
            }
            BLEApi.StartDeviceScan();
            isScanningDevices = true;  
            scanButtonText.text = "Stop Scan";
            scanStatusText.text = "Scanning...";
        }
        else {
            isScanningDevices = false;
            BLEApi.StopDeviceScan();
            scanButtonText.text = "Start Scan";
            scanStatusText.text = "Stopped";
        }
    }

    public void SelectDevice(GameObject data) {
        if (!isConnected) {
            for (int i = 0; i < scanResultRoot.transform.childCount; i++) {
                var child = scanResultRoot.transform.GetChild(i).gameObject;
                if (child == data){
                    if (child.transform.GetChild(0).GetComponent<TMP_Text>().color == Color.red) {
                        child.transform.GetChild(0).GetComponent<TMP_Text>().color = scanResult.transform.GetChild(0).GetComponent<TMP_Text>().color;
                        child.transform.GetChild(1).GetComponent<TMP_Text>().color = scanResult.transform.GetChild(1).GetComponent<TMP_Text>().color;
                        selectedDeviceIds.Remove(data.name);
                    }
                    else {
                        child.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                        child.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
                        if (!selectedDeviceIds.Contains(data.name)) {
                            selectedDeviceIds.Add(data.name);
                        }
                    }
                }
                else if (child.transform.GetChild(0).GetComponent<TMP_Text>().color == Color.red) {
                    child.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                    child.transform.GetChild(1).GetComponent<TMP_Text>().color = Color.red;
                    if (!selectedDeviceIds.Contains(data.name)) {
                        selectedDeviceIds.Add(data.name);
                    }
                }
                else {
                    child.transform.GetChild(0).GetComponent<TMP_Text>().color = scanResult.transform.GetChild(0).GetComponent<TMP_Text>().color;
                    child.transform.GetChild(1).GetComponent<TMP_Text>().color = scanResult.transform.GetChild(1).GetComponent<TMP_Text>().color;
                }
            }
            connectButton.interactable = true;
            writeButton.interactable = true;
        }
    }

    public void Connect() {
        if(!isConnected)
        {
            foreach (string selectedDeviceId in selectedDeviceIds)
            {
                BLEApi.SubscribeCharacteristic(selectedDeviceId, selectedServiceId, selectedCharacteristicId, false);
            }
            isConnected = true;
            staticIsConnected = true;
            UpdateUIPostConnection();
        }

            /*foreach (string selectedDeviceId in selectedDeviceIds) {
                BLEApi.SubscribeCharacteristic(selectedDeviceId, selectedServiceId, selectedCharacteristicId, false);
            }
            isConnected = true;
            isScanningDevices = false;
            scanStatusText.text = "Finished!";
            scanButton.interactable = false;
            connectButtonText.text = "Connected";
            connectButton.interactable = false;*/
    }

    /*
    public void Write() {
        foreach (string selectedDeviceId in selectedDeviceIds) {
            byte[] payload = Encoding.ASCII.GetBytes(writeInputText.text);
            BLEApi.BLEData data = new BLEApi.BLEData();
            data.buf = new byte[512];
            data.size = (short)payload.Length;
            data.deviceId = selectedDeviceId;
            data.serviceUuid = selectedServiceId;
            data.characteristicUuid = selectedCharacteristicId;
            for (int i = 0; i < payload.Length; i++) {
                data.buf[i] = payload[i];
            }
            BLEApi.SendData(in data, false);
        }
    }
    */
    private void UpdateConnectButtonUI()
    {
        connectButtonText.text = isConnected ? "Connected" : "Connect";
        connectButton.interactable = !isConnected;
    }

    private void UpdateUIPostConnection()
    {
        isScanningDevices = false;
        scanStatusText.text = "Finished!";
        scanButton.interactable = false;
        UpdateConnectButtonUI();
    }

    

    private void OnApplicationQuit() {
        BLEApi.Quit();
    }
}
