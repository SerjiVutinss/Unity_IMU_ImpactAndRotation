using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>
/// Reads detected Shimmer devices into a list, used to populate the dropdown menu
/// </summary>

public class ShimmerDiscovery : MonoBehaviour
{
    private static readonly string CURRENT_DIR = Environment.CurrentDirectory;
    // path to external exe used to populate the data file
    private readonly string EXE_PATH = CURRENT_DIR + @"\Assets\Plugins\shimmer_discovery\UnitySandbox.exe";
    // path to the json data file created by external exe
    private static readonly string DATA_PATH = CURRENT_DIR + @"\devices.json";
    // static list of all devices found in json data file
    public static List<ComDevice> devices = new List<ComDevice>();

    private void Start()
    {
        UpdateDeviceList();
    }

    public void UpdateDeviceList()
    {
        // run the external exe
        StartCoroutine(RunExternalDeviceFinder());
        // read the data from the file into the static list
        ReadDeviceDataFromFile();
    }

    private IEnumerator RunExternalDeviceFinder()
    {
        // run the external exe which populates the data file
        Process deviceFinder = new Process();
        deviceFinder.StartInfo.FileName = EXE_PATH;
        deviceFinder.Start();
        // 
        yield return new WaitForSeconds(1);
    }

    private void ReadDeviceDataFromFile()
    {
        // Read any found devices from devices.json
        StreamReader reader = new StreamReader(DATA_PATH);
        string s = "";
        while ((s = reader.ReadLine()) != null)
        {
            // deserialise each device into an object
            var d = JsonUtility.FromJson<ComDevice>(s);
            devices.Add(d); // add to static list of devices
        }
        reader.Close();
    }
}
