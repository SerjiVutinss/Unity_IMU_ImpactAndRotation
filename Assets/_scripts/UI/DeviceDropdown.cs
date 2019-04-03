using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceDropdown : MonoBehaviour
{
    // UI elements
    public Dropdown deviceDropdown;
    public Button btnRefresh;
    //public Button btnConnect;

    // dropdown items
    private List<string> options;
    // all devices found <displayName, device>
    private Dictionary<string, ComDevice> deviceMap;
    // selected device from dropdown
    private ComDevice selectedDevice = null;
    public ComDevice SelectedDevice { get => selectedDevice; }

    private void Awake()
    {
        //btnConnect.enabled = false;
        //btnConnect.onClick.AddListener(ConnectToShimmer);
        btnRefresh.onClick.AddListener(PopulateList);
        deviceDropdown.onValueChanged.AddListener(Dropdown_IndexChanged);

        PopulateList();
    }

    // dropdown selected item changed handler
    public void Dropdown_IndexChanged(int index)
    {
        //btnConnect.enabled = true;
        var o = options[index];
        if (deviceMap.ContainsKey(o))
        {
            selectedDevice = deviceMap[o];
        }
        //var btn = btnConnect.GetComponentInChildren<Text>();
        //if (selectedDevice != null)
        //{
        //    btn.text = "Connect to " + selectedDevice.DisplayName;
        //}
        //else
        //{
        //    btn.text = "Connect";
        //}
    }

    private void ConnectToShimmer()
    {
        Debug.Log("Connecting to " + selectedDevice.DisplayName);
    }

    private void PopulateList()
    {
        selectedDevice = null;
        options = new List<string>();
        options.Add("Please select a device:");
        deviceMap = new Dictionary<string, ComDevice>();
        deviceDropdown.ClearOptions();

        var devices = ShimmerDiscovery.devices;
        if (devices.Count == 0)
        {
            WaitForSeconds(3);
        }

        foreach (var d in devices)
        {
            deviceMap.Add(d.DisplayName, d);
            options.Add(d.DisplayName);
        }
        deviceDropdown.AddOptions(options);
    }

    private IEnumerator WaitForSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
