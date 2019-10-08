using System.Collections.Generic;
using UnityEngine;
using ShimmerInterfaceTest;
using ShimmerRT.models;
using UnityEngine.UI;
using ShimmerRT;
using System.Collections;
using System;
using Assets._Scripts;
using UnityEditor;

public class ShimmerDevice : MonoBehaviour, IFeedable
{
    public GameObject ShimmerGameObject;

    #region Fields and Properties
    private string comPort;
    private ShimmerController sc;

    // data structure used to shared data between this Shimmer 
    // and ShimmerJointOrientation

    public Queue<Shimmer3DModel> Queue { get; private set; }
    public List<Shimmer3DModel> RecordList { get; set; }

    public bool IsConnecting { get; set; }
    // True if and only if this Shimmer has paired successfully, at which point it will 
    // provide data and a connection with it will be maintained when possible.
    public bool IsPaired
    {
        get { return sc != null && sc.ShimmerDevice.IsConnected(); }
    }

    public bool IsRecording { get; private set; }
    public bool IsStreaming { get; private set; }
    public bool IsPlayback = false;

    #region UI Elements
    private DeviceDropdown deviceDropdown; // get reference in Start()

    // buttons with handlers
    public Button btnConnect;
    public Button btnDisconnect;
    public Button btnStartStream;
    public Button btnStopStream;

    public Button btnStartRecord;
    public Button btnStopRecord;

    public Button btnSaveToFile;
    public Button btnLoadFile;
    public Button btnPlaybackSession;

    // UI feedback, output, etc
    public GameObject pairedPanel;
    public Text txtIsPaired;
    public GameObject streamingPanel;
    public Text txtIsStreaming;
    public GameObject recordingPanel;
    public Text txtIsRecording;

    public Text inputFilePath;
    public Text txtOutput;

    // private fields
    private Image isPairedBackground;
    private Image isStreamingBackground;
    private Image isRecordingBackground;
    private ComDevice selectedItem;
    #endregion

    #endregion

    #region Unity methods
    void Start()
    {
        isPairedBackground = pairedPanel.GetComponent<Image>();
        isStreamingBackground = streamingPanel.GetComponent<Image>();
        isRecordingBackground = recordingPanel.GetComponent<Image>();
        // get a reference to the script on the device dropdown
        deviceDropdown = gameObject.GetComponentInParent<DeviceDropdown>();

        // Add UI Button click handlers
        btnConnect.onClick.AddListener(Connect);
        btnDisconnect.onClick.AddListener(Disconnect);
        btnStartStream.onClick.AddListener(StartStream);
        btnStopStream.onClick.AddListener(StopStream);
        btnStartRecord.onClick.AddListener(StartRecord);
        btnStopRecord.onClick.AddListener(StopRecord);
        btnSaveToFile.onClick.AddListener(SaveToFile);
        btnLoadFile.onClick.AddListener(LoadFromFile);
        btnPlaybackSession.onClick.AddListener(PlaybackSession);

        Queue = new Queue<Shimmer3DModel>();
    }

    private void PlaybackSession()
    {

        // reset the transform
        ShimmerGameObject.transform.position = new Vector3(0, 0, 0);
        IsPlayback = true;
        Debug.Log("Playback True");
    }

    private void SaveToFile()
    {
        if (RecordList != null && RecordList.Count > 0)
        {
            if (inputFilePath.text != null && inputFilePath.text.Length > 0)
            {
                FileHandler.SaveModelToFile(inputFilePath.text, RecordList);
            }
            else
            {
                FileHandler.SaveModelToFile("XXXX", RecordList);
            }
        }
    }

    private void LoadFromFile()
    {
        string path = EditorUtility.OpenFilePanel("", "", "csv");
        RecordList = FileHandler.LoadModelFromFile(path);
    }

    private void Update()
    {
        if (IsConnecting)
        {
            if (IsPaired)
            {
                btnStartStream.enabled = IsPaired;
                txtIsPaired.text = "Paired with " + selectedItem.DisplayName;
                isPairedBackground.color = Color.green;
                IsConnecting = false;
            }
        }

        if (IsRecording)
        {
            //btnPlaybackSession.enabled = IsRecording;
            txtIsRecording.text = "Recording..";
            isRecordingBackground.color = Color.green;
        }
    }
    #endregion

    #region Get data
    // this method is called for each row of data received from the Shimmer
    public void UpdateFeed(List<double> data)
    {
        Shimmer3DModel s;
        if (data.Count > 0)
        {
            // put this data as a model on the shared Queue
            s = Shimmer3DModel.GetModelFromArray(data.ToArray());
            Queue.Enqueue(s);
            if (IsRecording)
            {
                // save to list aswell
                RecordList.Add(s);
            }
        }
    }
    #endregion

    #region Shimmer device methods

    // connect to a shimmer, comPort must be set
    public void Connect()
    {
        selectedItem = deviceDropdown.SelectedDevice;
        if (selectedItem == null)
        {
            Debug.Log("NO DEVICE SELECTED!");
            return;
        }

        Debug.Log("CONNECT AND STREAM CLICKED");
        this.comPort = selectedItem.ComPort.ToString();
        Debug.Log("USING COM" + this.comPort);

        txtOutput.text = "Connecting...";
        sc = new ShimmerController(this);
        txtOutput.text += "\nTrying to connect on COM" + this.comPort;
        sc.Connect("COM" + this.comPort);
        IsConnecting = true;
    }

    // attempt to start streaming
    public void StartStream()
    {
        if (!IsPaired)
        {
            Debug.Log("NOT PAIRED!");
            return;
        }
        if (IsStreaming)
        {
            Debug.Log("ALREADY STREAMING");
        }
        IsStreaming = true;
        Debug.Log("PAIRED!");
        sc.ShimmerDevice.WriteBaudRate(230000);

        txtOutput.text += "\nConnected";
        // set options, start streaming
        sc.ShimmerDevice.Set3DOrientation(true);


        txtOutput.text += "\nStarting stream...";


        sc.StartStream();
        isStreamingBackground.color = Color.green;
        txtIsStreaming.text = "Streaming";

    }

    private void StopStream()
    {
        if (!IsStreaming)
        {
            Debug.Log("NOT STREAMING");
        }

        txtOutput.text += "\nStopping stream";
        sc.StopStream(); // stop the stream
        txtOutput.text += "\nStream Stopped";

        isStreamingBackground.color = Color.red;
        txtIsStreaming.text = "Not Streaming";
    }

    // stop the stream and disconnect from paired shimmer
    public void Disconnect()
    {
        sc.ShimmerDevice.Disconnect(); // disconnect from the Shimmer
        sc = null; // set controller to null
        txtOutput.text += "\nDisconnected";
        isPairedBackground.color = Color.red;
    }

    public void StartRecord()
    {
        if (IsStreaming)
        {
            if (!IsRecording)
            {
                RecordList = new List<Shimmer3DModel>();
                IsRecording = true;
                Debug.Log("Started Recording");
            }
        }
        else
        {
            Debug.Log("Not streaming - cannot record");
        }
    }

    public void StopRecord()
    {
        if (IsRecording)
        {
            IsRecording = false;
            txtIsRecording.text = "Not Recording";
            isRecordingBackground.color = Color.red;
            Debug.Log("Stopped Recording");
        }
        else
        {
            Debug.Log("Not recording - cannot stop recording");
        }
    }

    #endregion
}