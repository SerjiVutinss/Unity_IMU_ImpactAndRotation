using UnityEngine;
using ShimmerRT.models;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Linq;
using Assets._Scripts;

public class SamplePlaybackCodeSnippet : MonoBehaviour
{// Myo game object to connect with.
    // This object must have a ThalmicMyo script attached.
    public GameObject shimmerDevice = null;

    // A rotation that compensates for the Myo armband's orientation parallel to the ground, i.e. yaw.
    // Once set, the direction the Myo armband is facing becomes "forward" within the program.
    // Set by making the fingers spread pose or pressing "r".
    private Quaternion _antiYaw = Quaternion.identity;

    // A reference angle representing how the armband is rotated about the wearer's arm, i.e. roll.
    // Set by making the fingers spread pose or pressing "r".
    private float _referenceRoll = 0.0f;
    private ShimmerFeedManager shimmerFeed;
    private Vector3 accelerometer;
    private Vector3 gyroscope;

    private Shimmer3DModel lastShimmerModel = null;

    public Text txtImpact;

    public float impactThreshold = 1.0f;
    public float isMovingThreshold = 1.0f;
    public Button btnSnapshot;
    public Button btnLoad;
    public Button btnPlayback;
    //name value for accel + gyro from running rugby guy
    //Dictionary<string, Vector3> snapshots = new Dictionary<string, Vector3>();
    List<Shimmer3DModel> playback = new List<Shimmer3DModel>();
    List<Shimmer3DModel> loadFile = new List<Shimmer3DModel>();

    void Start()
    {
        // get the script from the ShimmerDevice object
        shimmerFeed = shimmerDevice.GetComponent<ShimmerFeedManager>();
        //ResetTransform();
        btnSnapshot.onClick.AddListener(SaveFile);
        btnLoad.onClick.AddListener(LoadFile);

    }

    private bool isPlayback = true;

    private void Update()
    {
        if (!isPlayback)
        {


            // if data is available, use it
            if (shimmerFeed.Queue.Count > 0)
            {
                var s = shimmerFeed.Queue.Dequeue();
                playback.Add(s);
                // see if there was an 'impact' between this data and the last received data
                if (lastShimmerModel != null)
                {
                    Debug.Log("Checking Impact");
                    if (CheckImpact(s))
                    {
                        txtImpact.text = "--IMPACT--\n" + Time.time;
                    }

                    if (CheckMoving(s))
                    {
                        txtImpact.text = "--IsMoving--\n" + "LN Acc X: " + accelerometer.x + "\nLN Acc Y: " + accelerometer.y + "\nLN Acc Z: " + accelerometer.z;

                    }
                }
                UpdateTransform(s);
                lastShimmerModel = s;
            }
        }
        else
        {
            if (loadFile.Count > 0 && loadFile != null)
            {
                var s = loadFile[0];
                UpdateTransform(s);
                loadFile.Remove(s);
            }
        }
    }


    #region == ImpactCheck ==

    private bool CheckImpact(Shimmer3DModel s)
    {

        float dX = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_X_CAL - s.Low_Noise_Accelerometer_X_CAL));
        float dY = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Y_CAL - s.Low_Noise_Accelerometer_Y_CAL));
        float dZ = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Z_CAL - s.Low_Noise_Accelerometer_Z_CAL));

        Debug.Log(dX);
        Debug.Log(dY);
        Debug.Log(dZ);

        if (dX > impactThreshold)
        {
            return true;
        }
        if (dY > impactThreshold)
        {
            return true;
        }
        if (dZ > impactThreshold)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region == Movement Snapshots ==
    private bool CheckMoving(Shimmer3DModel s)
    {
        //if movement in any direction is above a set threshold, capture data and add to list
        float dX = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_X_CAL - s.Low_Noise_Accelerometer_X_CAL));
        float dY = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Y_CAL - s.Low_Noise_Accelerometer_Y_CAL));
        float dZ = Mathf.Abs((float)(lastShimmerModel.Low_Noise_Accelerometer_Z_CAL - s.Low_Noise_Accelerometer_Z_CAL));

        if (dX > isMovingThreshold || dY > isMovingThreshold || dZ > isMovingThreshold)
        {
            Vector3 accel = new Vector3(dX, dY, dZ);
            Vector3 gyro = new Vector3((float)s.Gyroscope_X_CAL, (float)s.Gyroscope_Y_CAL, (float)s.Gyroscope_Z_CAL);
            IsMoving(s);
            return true;
        }

        return false;
    }

    //Add snapshots of model accel and rotation to list
    private void IsMoving(Shimmer3DModel s)
    {
        ////add string key then value
        //snapshots.Add("Ac: " + Time.time, accel);
        //snapshots.Add("Gy: " + Time.time, gyro);
        //playback.Add(BuildRowFromModel(s));
    }

    #region == BuildRows ==
    public static string BuildRowFromModel(Shimmer3DModel s)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(s.Timestamp_RAW); // convert this to a readable date
        sb.Append("," + s.Timestamp_CAL);

        sb.Append("," + s.Low_Noise_Accelerometer_X_RAW);
        sb.Append("," + s.Low_Noise_Accelerometer_X_CAL);
        sb.Append("," + s.Low_Noise_Accelerometer_Y_RAW);
        sb.Append("," + s.Low_Noise_Accelerometer_Y_CAL);
        sb.Append("," + s.Low_Noise_Accelerometer_Z_RAW);
        sb.Append("," + s.Low_Noise_Accelerometer_Z_CAL);

        sb.Append("," + s.Wide_Range_Accelerometer_X_RAW);
        sb.Append("," + s.Wide_Range_Accelerometer_X_CAL);
        sb.Append("," + s.Wide_Range_Accelerometer_Y_RAW);
        sb.Append("," + s.Wide_Range_Accelerometer_Y_CAL);
        sb.Append("," + s.Wide_Range_Accelerometer_Z_RAW);
        sb.Append("," + s.Wide_Range_Accelerometer_Z_CAL);

        sb.Append("," + s.Gyroscope_X_RAW);
        sb.Append("," + s.Gyroscope_X_CAL);
        sb.Append("," + s.Gyroscope_Y_RAW);
        sb.Append("," + s.Gyroscope_Y_CAL);
        sb.Append("," + s.Gyroscope_Z_RAW);
        sb.Append("," + s.Gyroscope_Z_CAL);

        sb.Append("," + s.Magnetometer_X_RAW);
        sb.Append("," + s.Magnetometer_X_CAL);
        sb.Append("," + s.Magnetometer_Y_RAW);
        sb.Append("," + s.Magnetometer_Y_CAL);
        sb.Append("," + s.Magnetometer_Z_RAW);
        sb.Append("," + s.Magnetometer_Z_CAL);


        sb.Append("," + s.Pressure_RAW);
        sb.Append("," + s.Pressure_CAL);
        sb.Append("," + s.Temperature_RAW);
        sb.Append("," + s.Temperature_CAL);

        sb.Append("," + s.Axis_Angle_A_CAL);
        sb.Append("," + s.Axis_Angle_X_CAL);
        sb.Append("," + s.Axis_Angle_Y_CAL);
        sb.Append("," + s.Axis_Angle_Z_CAL);

        sb.Append("," + s.Quaternion_0_CAL);
        sb.Append("," + s.Quaternion_1_CAL);
        sb.Append("," + s.Quaternion_2_CAL);
        sb.Append("," + s.Quaternion_3_CAL);

        return sb.ToString();

    }
    #endregion
    #endregion

    #region == Save to File ==
    private void SaveFile()
    {
        FileHandler.SaveModelToFile("KKK", playback);
    }

    private void LoadFile()
    {
        //oper file picker and return file name
        string path = EditorUtility.OpenFilePanel("", "", "csv");
        //loadFile = FileHandler.LoadModelFromFile(path);
    }

    public static Shimmer3DModel FromCsv(string csvLine)
    {
        string[] values = csvLine.Split(',');
        Shimmer3DModel loadedModel = new Shimmer3DModel(
            Convert.ToDouble(values[0]),
            Convert.ToDouble(values[1]),
            Convert.ToDouble(values[2]),
            Convert.ToDouble(values[3]),
            Convert.ToDouble(values[4]),
            Convert.ToDouble(values[5]),
            Convert.ToDouble(values[6]),
            Convert.ToDouble(values[7]),
            Convert.ToDouble(values[8]),
            Convert.ToDouble(values[9]),
            Convert.ToDouble(values[10]),
            Convert.ToDouble(values[11]),
            Convert.ToDouble(values[12]),
            Convert.ToDouble(values[13]),
            Convert.ToDouble(values[14]),
            Convert.ToDouble(values[15]),
            Convert.ToDouble(values[16]),
            Convert.ToDouble(values[17]),
            Convert.ToDouble(values[18]),
            Convert.ToDouble(values[19]),
            Convert.ToDouble(values[20]),
            Convert.ToDouble(values[21]),
            Convert.ToDouble(values[22]),
            Convert.ToDouble(values[23]),
            Convert.ToDouble(values[24]),
            Convert.ToDouble(values[25]),
            Convert.ToDouble(values[26]),
            Convert.ToDouble(values[27]),
            Convert.ToDouble(values[28]),
            Convert.ToDouble(values[29]),
            Convert.ToDouble(values[30]),
            Convert.ToDouble(values[31]),
            Convert.ToDouble(values[32]),
            Convert.ToDouble(values[33]),
            Convert.ToDouble(values[34]),
            Convert.ToDouble(values[35]),
            Convert.ToDouble(values[36]),
            Convert.ToDouble(values[37]));

        return loadedModel;
    }
    //build list from file path.. posibly better to load direct from file w/streams?
    private static List<Shimmer3DModel> ReadFile(string path)
    {
        /*  
        The File.ReadAllLines reads all lines from the CSV file into a string array.
        The .Select(v => FromCsv(v)) uses Linq to build new shimmer model instead of for each
         */
        var loadFile = File.ReadAllLines(path).Select(row => FromCsv(row))
                                            .ToList();

        return loadFile;
    }




    #endregion

   

    private void UpdateTransform(Shimmer3DModel s)
    {

        transform.localRotation = new Quaternion(
            -(float)s.Quaternion_2_CAL,
            -(float)s.Quaternion_0_CAL,
            (float)s.Quaternion_1_CAL,
            -(float)s.Quaternion_3_CAL);


        accelerometer = new Vector3(
            (float)s.Low_Noise_Accelerometer_X_CAL,
            (float)s.Low_Noise_Accelerometer_Y_CAL,
            (float)s.Low_Noise_Accelerometer_Z_CAL);
        //Debug.Log("LN Acc X: " + accelerometer.x + "LN Acc Y: " + accelerometer.y + "LN Acc Z: " + accelerometer.z);

        gyroscope = new Vector3(
            (float)s.Gyroscope_Y_CAL,
            (float)s.Gyroscope_Z_CAL,
            -(float)s.Gyroscope_X_CAL);
    }

}
