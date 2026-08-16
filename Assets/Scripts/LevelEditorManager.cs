using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public static LevelEditorManager Instance;

    [Header("Room Database")]
    [SerializeField] private RoomDatabase roomDatabase;

    private RoomConnector selectedConnector;

    public RoomDatabase RoomDatabase => roomDatabase;

    public RoomConnector SelectedConnector =>
        selectedConnector;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectConnector(
        RoomConnector connector)
    {
        selectedConnector = connector;
    }

    public void ClearConnectorSelection()
    {
        selectedConnector = null;
    }
}