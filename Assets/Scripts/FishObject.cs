using System.Collections;
using UnityEngine;

public class FishObject : MonoBehaviour
{
    public FishData fishData;
    private Coroutine destroyCoroutine;
    private const float timeBeforeDestruction = 30f; // Время до удаления рыбы (30 секунд)

    private void Start()
    {
        // Запускаем Coroutine для отслеживания времени жизни рыбы
        destroyCoroutine = StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        float remainingTime = timeBeforeDestruction;

        while (remainingTime > 0)
        {
            Debug.Log($"Осталось времени: {remainingTime:F1} секунд");
            remainingTime -= 1f;  // Уменьшаем время на 1 секунду
            yield return new WaitForSeconds(1f);  // Ждем 1 секунду
        }

        // Если инвентарь не заполнен, рыба будет забрана
        if (Inventory.instance.IsFull())
        {
            Debug.Log("Инвентарь полон, рыба не может быть забрана.");
        }
        else
        {
            Debug.Log("Рыба забрана игроком.");
            // Добавьте логику добавления рыбы в инвентарь здесь (например, вызов метода для добавления рыбы в инвентарь)
            // Inventory.instance.AddFishItem(fishData, gameObject);
            Destroy(gameObject); // Удаляем рыбу из сцены
        }
    }

    public void InitializeFish(FishData fishData)
    {
        if (fishData == null)
        {
            Debug.LogError("fishData is null! Cannot apply fish data.");
            return;
        }

        this.fishData = fishData;

        // Устанавливаем параметры на основе уже существующих данных
        float size = fishData.RandomSize;
        transform.localScale = new Vector3(size, size, size);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.mass = size * size * size;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.drag = 3f;
            rb.angularDrag = 3f;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (fishData.RandomMaterial != null)
            {
                renderer.material = fishData.RandomMaterial;
            }
            else
            {
                Debug.LogWarning("RandomMaterial is null!");
            }
        }
    }

    public void TryPickUpFish()
    {
        // Если инвентарь полный, рыба не может быть забрана
        if (Inventory.instance.IsFull())
        {
            Debug.Log("Инвентарь полный, рыба не может быть забрана.");
            return; // Выход из метода, не забирая рыбу
        }

        // Если инвентарь не полный, пытаемся забрать рыбу
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }

        // Здесь можно добавить логику для добавления рыбы в инвентарь
        Inventory.instance.AddFishItem(fishData, gameObject);
        Debug.Log("Рыба забрана игроком.");
        Destroy(gameObject); // Удаляем рыбу из сцены, если она забрана
    }
}
