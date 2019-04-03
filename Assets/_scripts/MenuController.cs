using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject panel;
    public Button btnToggleMenu;

    private bool isOpen = false;

    public void Awake()
    {
        panel.gameObject.SetActive(isOpen);
        btnToggleMenu.onClick.AddListener(TogglePanel);
    }

    public void TogglePanel()
    {
        if (panel != null)
        {
            isOpen = !isOpen;
            panel.SetActive(isOpen);
        }
    }
}
