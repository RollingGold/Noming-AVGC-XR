using UnityEngine;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public static bool InputLocked { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        //DontDestroyOnLoad(gameObject);
    }

    public static void LockInput()
    {
        InputLocked = true;
    }

    public static void UnlockInput()
    {
        InputLocked = false;
    }
}