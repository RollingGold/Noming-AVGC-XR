using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelMapSaver : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private Transform generatedRoomsRoot;

    [Header("Save")]
    [SerializeField] private string saveFolder = "Assets/Levels";

    public void SaveMap()
    {
#if UNITY_EDITOR

        if (generatedRoomsRoot == null)
        {
            Debug.LogError(
                "LevelMapSaver: Generated Rooms Root is not assigned.");

            return;
        }

        if (generatedRoomsRoot.childCount == 0)
        {
            Debug.LogWarning(
                "LevelMapSaver: There are no generated rooms to save.");

            return;
        }

        // Make sure the folder exists.
        EnsureFolderExists(saveFolder);

        // Open Unity Save File dialog.
        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Level Map",
                "NewLevel",
                "prefab",
                "Choose where to save the level map.",
                saveFolder);

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log(
                "Save cancelled.");

            return;
        }

        EditorUtility.SetDirty(
            generatedRoomsRoot.gameObject);

        foreach (Transform child in
                 generatedRoomsRoot.GetComponentsInChildren<Transform>())
        {
            if (PrefabUtility.IsPartOfPrefabInstance(
                child.gameObject))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    child.gameObject);
            }
        }

        // Save the entire Generated Rooms hierarchy.
        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(
                generatedRoomsRoot.gameObject,
                path);

        if (prefab == null)
        {
            Debug.LogError(
                "LevelMapSaver: Failed to save prefab.");

            return;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "LEVEL MAP SAVED:\n" +
            path);

#else

        Debug.LogWarning(
            "LevelMapSaver can only save maps inside the Unity Editor.");

#endif
    }

#if UNITY_EDITOR

    private void EnsureFolderExists(
        string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string[] folders =
            folderPath.Split('/');

        string currentFolder =
            folders[0];

        for (int i = 1;
             i < folders.Length;
             i++)
        {
            string nextFolder =
                currentFolder +
                "/" +
                folders[i];

            if (!AssetDatabase.IsValidFolder(
                nextFolder))
            {
                AssetDatabase.CreateFolder(
                    currentFolder,
                    folders[i]);
            }

            currentFolder =
                nextFolder;
        }
    }

#endif
}