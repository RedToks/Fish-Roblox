using UnityEngine;

public class FishObject : MonoBehaviour
{
    public FishData fishData;

    public void InitializeFish(FishData fishData)
    {
        if (fishData == null)
        {
            Debug.LogError("fishData is null! Cannot apply fish data.");
            return;
        }


        this.fishData = fishData;
        ApplyFishData();
    }

    private void ApplyFishData()
    {
        if (fishData != null)
        {
            // Устанавливаем размер рыбы
            float size = fishData.RandomSize;
            transform.localScale = new Vector3(size, size, size);

            // Устанавливаем массу рыбы (масса пропорциональна объему)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Объем рыбы можно приблизительно посчитать как куб её размера
                rb.mass = size * size * size;  // Пропорционально объему

                // Настройки физики для предотвращения толкания
                rb.useGravity = true; // Чтобы рыба не взлетала
                rb.isKinematic = false; // Не отключаем кинематику

                // Увеличиваем сопротивление
                rb.drag = 3f;  // Сопротивление воздуха
                rb.angularDrag = 3f;  // Сопротивление вращению
            }

            // Устанавливаем материалы
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Debug.LogWarning("No renderers found on fish object!");
            }

            foreach (Renderer renderer in renderers)
            {
                if (fishData.RandomMaterial != null)
                {
                    Debug.Log("Applying material: " + fishData.RandomMaterial.name);
                    renderer.material = fishData.RandomMaterial;
                }
                else
                {
                    Debug.LogWarning("RandomMaterial is null!");
                }
            }
        }
    }
}