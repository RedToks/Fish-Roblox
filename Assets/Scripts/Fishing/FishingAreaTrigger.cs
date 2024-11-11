using UnityEngine;

public class FishingAreaTrigger : MonoBehaviour
{
    [SerializeField] private Sea seaData; // —сылка на данные мор€

    public Sea SeaData => seaData;
    public bool IsInFishingArea { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            IsInFishingArea = true;
            playerInteraction.SetCurrentFishingArea(this);
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
