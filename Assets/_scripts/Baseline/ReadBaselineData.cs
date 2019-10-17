using Assets._scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadBaselineData : MonoBehaviour
{
    private static readonly string CURRENT_DIR = Environment.CurrentDirectory;
    // path to external exe used to populate the data file
    private readonly string EXE_PATH = CURRENT_DIR + @"\Assets\Plugins\shimmer_discovery\UnitySandbox.exe";
    // path to the json data file created by external exe
    private static readonly string DATA_PATH = CURRENT_DIR + @"\output.json";

    public List<LinearAccelModel> BaseLineData { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        BaseLineData = ReadDeviceDataFromFile();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private List<LinearAccelModel> ReadDeviceDataFromFile()
    {
        //List<LinearAccelModel> models = new List<LinearAccelModel>();
        // Read any found devices from devices.json
        StreamReader reader = new StreamReader(DATA_PATH);
        var data = reader.ReadToEnd();
        LinearAccelModel[] models = null;
        try
        {
            models = JsonHelper.FromJson<LinearAccelModel>(data);
        }
        catch
        {
            Debug.Log("Error in JSON - attempting to fix the issue!");
            models = JsonHelper.FromJson<LinearAccelModel>(JsonHelper.FixJson(data));
        }
        if (models != null)
        {
            Debug.Log("Fixed! JSON loaded successfully");
        } else
        {
            Debug.Log("Error! JSON could not be loaded successfully");
        }

        reader.Close();
        return new List<LinearAccelModel>(models);
    }
}
