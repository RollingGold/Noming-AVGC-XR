using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private bool interactPressed;

    private readonly List<IInteractable> interactables =
    new List<IInteractable>();

    private readonly Dictionary<IInteractable, GameObject> prompts =
        new Dictionary<IInteractable, GameObject>();

    private IInteractable currentInteractable;

    private void Awake()
    {

        inputActions =
            new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();


        inputActions.Player.Interact.performed += ctx =>
        {
            interactPressed = true;
        };
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        UpdateCurrentInteractable();

        HandleInteraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(
            out IInteractable interactable))
            return;

        if (interactables.Contains(interactable))
            return;

        interactables.Add(interactable);

        GameObject prompt =
            InteractionUIManager.Instance.CreatePrompt(
                interactable.InteractionText);

        prompts.Add(interactable, prompt);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(
            out IInteractable interactable))
            return;

        interactables.Remove(interactable);

        if (prompts.TryGetValue(
            interactable,
            out GameObject prompt))
        {
            InteractionUIManager.Instance.RemovePrompt(
                prompt);

            prompts.Remove(interactable);
        }
    }

    private void HandleInteraction()
    {
        if (!interactPressed)
            return;

        interactPressed = false;

        interactables.RemoveAll(i => i == null);

        if (currentInteractable == null)
            return;

        currentInteractable.Interact();

        RemoveInteractable(currentInteractable);
    }

    private void RemoveInteractable(
    IInteractable interactable)
    {
        interactables.Remove(interactable);

        if (prompts.TryGetValue(
            interactable,
            out GameObject prompt))
        {
            InteractionUIManager.Instance
                .RemovePrompt(prompt);

            prompts.Remove(interactable);
        }
    }

    private void UpdateCurrentInteractable()
    {
        currentInteractable = null;

        float closestDistance = Mathf.Infinity;

        foreach (IInteractable interactable in interactables)
        {
            if (interactable == null)
                continue;

            float distance =
                Vector3.Distance(
                    transform.position,
                    ((MonoBehaviour)interactable).transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }

        foreach (var pair in prompts)
        {
            pair.Value.GetComponent<InteractionPromptUI>()
                .SetSelected(pair.Key == currentInteractable);
        }
    }
}