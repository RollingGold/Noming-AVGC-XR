using System.Collections.Generic;
using UnityEngine;

public class RoomSelectionUI : MonoBehaviour
{
    [Header("Room Database")]
    [SerializeField] private RoomDatabase roomDatabase;

    [Header("References")]
    [SerializeField] private LevelEditorSelection levelEditorSelection;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject roomButtonPrefab;

    private readonly List<GameObject>
        spawnedButtons = new();

    private void Start()
    {
        GenerateRoomButtons();
    }

    public void GenerateRoomButtons()
    {
        ClearButtons();

        if (roomDatabase == null)
        {
            Debug.LogError(
                "Room Database is not assigned.");

            return;
        }

        if (levelEditorSelection == null)
        {
            Debug.LogError(
                "LevelEditorSelection is not assigned.");

            return;
        }

        if (content == null)
        {
            Debug.LogError(
                "Content is not assigned.");

            return;
        }

        if (roomButtonPrefab == null)
        {
            Debug.LogError(
                "Room Button Prefab is not assigned.");

            return;
        }

        foreach (Room room in roomDatabase.rooms)
        {
            if (room == null)
                continue;

            GameObject button =
                Instantiate(
                    roomButtonPrefab,
                    content);

            RoomButtonUI roomButton =
                button.GetComponent<RoomButtonUI>();

            if (roomButton == null)
            {
                Debug.LogError(
                    "RoomButtonUI missing on " +
                    roomButtonPrefab.name);

                Destroy(button);

                continue;
            }

            roomButton.Setup(
                room,
                levelEditorSelection);

            spawnedButtons.Add(button);
        }
    }

    public void SelectRoom(Room room)
    {
        if (room == null)
            return;

        if (levelEditorSelection == null)
        {
            Debug.LogWarning(
                "LevelEditorSelection is not assigned.");

            return;
        }

        //levelEditorSelection.SelectPrefab(room);
    }

    private void ClearButtons()
    {
        foreach (GameObject button
                 in spawnedButtons)
        {
            if (button != null)
                Destroy(button);
        }

        spawnedButtons.Clear();
    }
}