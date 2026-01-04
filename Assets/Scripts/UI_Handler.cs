using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject MenuCanvas;
    public GameObject SettingsCanvas;
    public void OnButtonClickInventory()
    {
        canvas1.SetActive(false);
        canvas2.SetActive(true);
    }

    public void OnButtonClickExitInv()
    {
        canvas1.SetActive(true);
        canvas2.SetActive(false);
    }

        public void OnButtonClickSettings()
    {
        MenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void OnButtonClickMenu()
    {
        MenuCanvas.SetActive(true);
        SettingsCanvas.SetActive(false);
    }
}
