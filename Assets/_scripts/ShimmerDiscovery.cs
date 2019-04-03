using Assets._scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShimmerDiscovery : MonoBehaviour
{
    private static readonly string CURRENT_DIR = Environment.CurrentDirectory;
    private static readonly string DATA_PATH = CURRENT_DIR + @"\devices.json";
    private readonly string EXE_PATH = CURRENT_DIR + @"\Assets\Plugins\shimmer_discovery\UnitySandbox.exe";

    public static List<CustomShimmerDevice> devices = new List<CustomShimmerDevice>();

    private void Start()
    {
        StartCoroutine(RunExternalDeviceFinder());
    }

    private IEnumerator RunExternalDeviceFinder()
    {
        Process deviceFinder = new Process();
        deviceFinder.StartInfo.FileName = EXE_PATH;
        deviceFinder.Start();
        yield return new WaitForSeconds(1);

        ReadString();
    }

    private void ReadString()
    {
        // Read any found devices from devices.json
        StreamReader reader = new StreamReader(DATA_PATH);
        string s = "";
        while ((s = reader.ReadLine()) != null)
        {
            var d = JsonUtility.FromJson<CustomShimmerDevice>(s);
            devices.Add(d);
            //txtOutput.text += Environment.NewLine + d.DisplayName;
            UnityEngine.Debug.Log(s.ToString());
        }
        reader.Close();
    }
}
