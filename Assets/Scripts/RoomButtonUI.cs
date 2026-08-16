using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButtonUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RawImage prefabImage;
    [SerializeField] private TMP_Text prefabName;

    private Room room;
    private LevelEditorSelection levelEditorSelection;

    public void Setup(
        Room room,
        LevelEditorSelection levelEditorSelection)
    {
        this.room = room;
        this.levelEditorSelection =
            levelEditorSelection;

        if (prefabName != null)
        {
            prefabName.text =
                room.RoomName;
        }

        if (prefabImage != null)
        {
            if (room.Thumbnail != null)
            {
                prefabImage.texture =
                    room.Thumbnail.texture;

                prefabImage.enabled = true;
            }
            else
            {
                prefabImage.texture = null;
                prefabImage.enabled = false;
            }
        }
    }

    public void OnClick()
    {
        Debug.Log(
            "ROOM BUTTON CLICKED");

        if (levelEditorSelection == null)
        {
            Debug.LogError(
                "RoomButtonUI: LevelEditorSelection is not assigned.");

            return;
        }

        if (room == null)
        {
            Debug.LogError(
                "RoomButtonUI: Room is null.");

            return;
        }

        Debug.Log(
            "Building room: " +
            room.RoomName);

        levelEditorSelection.BuildSelectedRoom(
            room);
    }
}