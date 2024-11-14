using UnityEngine;

public class FishingAreaTrigger : MonoBehaviour
{
    public SeaType seaType; // —сылка на объект Sea дл€ данной зоны
    public bool IsInFishingArea { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            IsInFishingArea = true;
            playerInteraction.SetCurrentFishingArea(this);
            Debug.Log($"»грок в зоне: {seaType}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            IsInFishingArea = false;
            playerInteraction.SetCurrentFishingArea(null);
        }
    }
}
