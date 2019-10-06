using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject connectPanel;
    public Button btnToggleConnect;

    public GameObject recordPanel;
    public Button btnToggleRecord;

    public GameObject playbackPanel;
    public Button btnTogglePlayback;

    public GameObject profilePanel;
    public Button btnToggleProfile;

    private List<GameObject> panelList;

    public void Awake()
    {
        panelList = new List<GameObject>()
        {
            connectPanel,
            recordPanel,
            playbackPanel,
            profilePanel
        };

        // disable all panels
        connectPanel.gameObject.SetActive(false);
        recordPanel.gameObject.SetActive(false);
        playbackPanel.gameObject.SetActive(false);
        profilePanel.gameObject.SetActive(false);

        btnToggleConnect.onClick.AddListener(ToggleConnectPanel);
        btnToggleRecord.onClick.AddListener(ToggleRecordPanel);
        btnTogglePlayback.onClick.AddListener(TogglePlaybackPanel);
        btnToggleProfile.onClick.AddListener(ToggleProfilePanel);
    }

    public void TogglePanel(GameObject panel)
    {

        List<string> strings = new List<string>();
        var s = strings.ToArray();

        foreach (var p in panelList)
        {
            if (p != panel)
            {
                p.SetActive(false);
            }
        }

        if (panelList.Contains(panel))
        {
            panel.SetActive(!panel.activeInHierarchy);
        }
    }

    private void ToggleConnectPanel()
    {
        if (connectPanel != null)
        {
            TogglePanel(connectPanel);
        }
    }

    private void ToggleRecordPanel()
    {
        if (recordPanel != null)
        {
            TogglePanel(recordPanel);
        }
    }

    private void TogglePlaybackPanel()
    {
        if (playbackPanel != null)
        {
            TogglePanel(playbackPanel);
        }
    }

        private void ToggleProfilePanel()
    {
        if (profilePanel != null)
        {
            TogglePanel(profilePanel);
        }
    }
}
