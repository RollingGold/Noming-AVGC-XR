using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField]
    private string interactionText = "Open Chest";

    [Header("Items")]
    [SerializeField]
    private List<ItemData> items = new List<ItemData>();

    [Header("Inventory")]
    [SerializeField]
    private Inventory inventory;

    [Header("Chest")]
    [Tooltip("The top/lid part of the chest.")]
    [SerializeField]
    private Transform chestTop;

    [SerializeField]
    private float openDuration = 0.5f;

    private bool opened;
    private bool opening;

    private Collider collider;

    public string InteractionText =>
        interactionText;

    private void Start()
    {
        if(inventory == null)
        {
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }

        collider = gameObject.GetComponent<Collider>();

    }

    private void Update()
    {
        if( opened || opening)
        {
            collider.enabled = false;
        }
        else
        {
            collider.enabled = true;
        }
    }

    public void Interact()
    {
        if (opened || opening)
            return;

        if (inventory == null)
        {
            Debug.LogWarning(
                "Chest has no Inventory assigned.",
                this);

            return;
        }

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning(
                "Chest has no items assigned.",
                this);

            return;
        }

        // Add every item to the inventory
        foreach (ItemData item in items)
        {
            if (item == null)
                continue;

            inventory.AddItem(item);
        }

        StartCoroutine(OpenChest());

        
    }


    private IEnumerator OpenChest()
    {
        opening = true;

        if (chestTop == null)
        {
            Debug.LogWarning(
                "Chest Top is not assigned.",
                this);

            opened = true;
            opening = false;

            yield break;
        }

        Vector3 startEuler =
            chestTop.localEulerAngles;

        float startX = -90f;
        float endX = -180f;

        float y = startEuler.y;
        float z = startEuler.z;

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / openDuration);

            t = Mathf.SmoothStep(
                0f,
                1f,
                t);

            float x =
                Mathf.Lerp(
                    startX,
                    endX,
                    t);

            chestTop.localRotation =
                Quaternion.Euler(
                    x,
                    y,
                    z);

            yield return null;
        }

        chestTop.localRotation =
            Quaternion.Euler(
                endX,
                y,
                z);

        opened = true;
        opening = false;
    }
}