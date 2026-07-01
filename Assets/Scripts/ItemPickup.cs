using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ItemData item;

    private Inventory inventory;

    public string InteractionText => "Pick Up";

    private void Awake()
    {
        inventory = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<Inventory>();
    }

    public void Interact()
    {
        inventory.AddItem(item);

        Destroy(gameObject);
    }


}