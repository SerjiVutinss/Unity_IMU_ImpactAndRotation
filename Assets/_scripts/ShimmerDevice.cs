using System.Collections.Generic;
using UnityEngine;
using ShimmerInterfaceTest;
using ShimmerRT.models;
using UnityEngine.UI;
using ShimmerRT;
using System.Collections;

public class ShimmerDevice : MonoBehaviour, IFeedable
{
    #region Fields and Properties
    private string comPort;
    private ShimmerController sc;

    // data structure used to shared data between this Shimmer 
    // and ShimmerJointOrientation

    public Queue<Shimmer3DModel> Queue { get; private set; }

    // True if and only if this Shimmer has paired successfully, at which point it will 
    // provide data and a connection with it will be maintained when possible.
    public bool IsPaired
    {
        get { return sc != null && sc.ShimmerDevice.IsConnected(); }
    }

    #region UI Elements
    private DeviceDropdown deviceDropdown; // get reference in Start()

    public Button btnConnect;
    public Button btnStream;
    public Button btnStop;
    public Text txtOutput;
    #endregion

    #endregion

    #region Unity methods
    void Start()
    {
        // get a reference to the script on the device dropdown
        deviceDropdown = gameObject.GetComponentInParent<DeviceDropdown>();

        // Add UI Button click handlers
        btnConnect.onClick.AddListener(Connect);
        btnStream.onClick.AddListener(StartStreaming);
        btnStop.onClick.AddListener(Disconnect);

        Queue = new Queue<Shimmer3DModel>();
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
        }
    }
    #endregion

    #region Shimmer device methods

    // connect to a shimmer, comPort must be set
    public void Connect()
    {
        var selectedItem = deviceDropdown.SelectedDevice;
        if (selectedItem == null)
        {
            Debug.Log(selectedItem.DisplayName);
            return;
        }
        Debug.Log("NO DEVICE SELECTED!");



        Debug.Log("CONNECT AND STREAM CLICKED");
        this.comPort = selectedItem.ComPort.ToString();
        Debug.Log("USING COM" + this.comPort);

        txtOutput.text = "Connecting...";
        sc = new ShimmerController(this);
        txtOutput.text += "\nTrying to connect on COM" + this.comPort;
        sc.Connect("COM" + this.comPort);
    }

    // attempt to start streaming
    public void StartStreaming()
    {
        if (!IsPaired)
        {
            Debug.Log("NOT PAIRED!");
            return;
        }

        Debug.Log("PAIRED!");
        sc.ShimmerDevice.WriteBaudRate(230000);

        txtOutput.text += "\nConnected";
        // set options, start streaming
        sc.ShimmerDevice.Set3DOrientation(true);
        txtOutput.text += "\nStarting stream...";
        sc.StartStream();
    }

    // stop the stream and disconnect from paired shimmer
    public void Disconnect()
    {
        print("Stopping stream...");
        txtOutput.text += "\nStopping stream";
        sc.StopStream(); // stop the stream
        sc.ShimmerDevice.Disconnect(); // disconnect from the Shimmer
        sc = null; // set controller to null

        txtOutput.text += "\nStream Stopped";
        txtOutput.text += "\nDisconnected";
    }

    #endregion
}