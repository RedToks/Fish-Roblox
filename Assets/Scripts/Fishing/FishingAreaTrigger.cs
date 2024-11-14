using UnityEngine;

public class FishingAreaTrigger : MonoBehaviour
{
    public SeaType seaType; // ������ �� ������ Sea ��� ������ ����
    public bool IsInFishingArea { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            IsInFishingArea = true;
            playerInteraction.SetCurrentFishingArea(this);
            Debug.Log($"����� � ����: {seaType}");
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
