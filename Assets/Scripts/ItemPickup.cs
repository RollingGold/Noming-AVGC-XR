using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField]
    private ItemData item;

    private void OnTriggerEnter(
        Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory =
            other.GetComponent<Inventory>();

        inventory.AddItem(item);

        Destroy(gameObject);
    }
}