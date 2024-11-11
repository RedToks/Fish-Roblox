using UnityEngine;

public class UIManager : MonoBehaviour, IUIManager
{
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private Canvas toolTipCanvas;

    public void HideUI()
    {
        inventoryCanvas.gameObject.SetActive(false);
        toolTipCanvas.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        inventoryCanvas.gameObject.SetActive(true);
        toolTipCanvas.gameObject.SetActive(true);
    }
}
