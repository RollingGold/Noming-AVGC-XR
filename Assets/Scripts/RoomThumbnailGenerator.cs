#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class RoomThumbnailGenerator : EditorWindow
{
    private RoomDatabase database;

    private Camera previewCamera;

    private int imageSize = 512;

    private string outputFolder =
        "Assets/Room Thumbnails";

    private const string PreviewLayerName =
        "RoomPreview";

    [MenuItem("Tools/Room Thumbnail Generator")]
    public static void ShowWindow()
    {
        GetWindow<RoomThumbnailGenerator>(
            "Room Thumbnail Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Room Thumbnail Generator",
            EditorStyles.boldLabel);

        EditorGUILayout.Space();

        database =
            (RoomDatabase)EditorGUILayout.ObjectField(
                "Room Database",
                database,
                typeof(RoomDatabase),
                false);

        imageSize =
            EditorGUILayout.IntField(
                "Image Size",
                imageSize);

        outputFolder =
            EditorGUILayout.TextField(
                "Output Folder",
                outputFolder);

        EditorGUILayout.Space();

        if (GUILayout.Button(
            "Generate All Thumbnails",
            GUILayout.Height(40)))
        {
            GenerateAll();
        }
    }

    private void GenerateAll()
    {
        if (database == null)
        {
            Debug.LogError(
                "Room Database is not assigned.");

            return;
        }

        if (database.rooms == null ||
            database.rooms.Count == 0)
        {
            Debug.LogError(
                "Room Database contains no rooms.");

            return;
        }

        int previewLayer =
            LayerMask.NameToLayer(
                PreviewLayerName);

        if (previewLayer == -1)
        {
            Debug.LogError(
                "Layer '" +
                PreviewLayerName +
                "' does not exist. " +
                "Create it first in Unity's Layer settings.");

            return;
        }

        CreateOutputFolder();

        // --------------------------------
        // Create temporary camera
        // --------------------------------

        GameObject cameraObject =
            new GameObject(
                "Temporary Room Preview Camera");

        cameraObject.hideFlags =
            HideFlags.HideAndDontSave;

        previewCamera =
            cameraObject.AddComponent<Camera>();

        previewCamera.clearFlags =
            CameraClearFlags.SolidColor;

        previewCamera.backgroundColor =
            new Color(
                0.12f,
                0.12f,
                0.12f,
                1f);

        previewCamera.fieldOfView =
            40f;

        // IMPORTANT:
        // Camera sees ONLY RoomPreview layer.
        previewCamera.cullingMask =
            1 << previewLayer;

        // --------------------------------
        // Generate
        // --------------------------------

        foreach (Room room in database.rooms)
        {
            if (room == null)
                continue;

            GenerateThumbnail(
                room,
                previewLayer);
        }

        // --------------------------------
        // Cleanup
        // --------------------------------

        DestroyImmediate(
            cameraObject);

        AssetDatabase.Refresh();

        Debug.Log(
            "Room thumbnail generation complete.");
    }

    private void GenerateThumbnail(
        Room room,
        int previewLayer)
    {
        // --------------------------------
        // Create temporary room
        // --------------------------------

        GameObject instance =
            (GameObject)PrefabUtility.InstantiatePrefab(
                room.gameObject);

        if (instance == null)
            return;

        instance.hideFlags =
            HideFlags.HideAndDontSave;

        // --------------------------------
        // Put ONLY this room on
        // RoomPreview layer
        // --------------------------------

        SetLayerRecursively(
            instance,
            previewLayer);

        // --------------------------------
        // Find renderers
        // --------------------------------

        Renderer[] renderers =
            instance.GetComponentsInChildren<Renderer>(
                true);

        if (renderers.Length == 0)
        {
            DestroyImmediate(instance);
            return;
        }

        Bounds bounds =
            renderers[0].bounds;

        for (int i = 1;
             i < renderers.Length;
             i++)
        {
            bounds.Encapsulate(
                renderers[i].bounds);
        }

        // --------------------------------
        // Camera positioning
        // --------------------------------

        float size =
            Mathf.Max(
                bounds.size.x,
                bounds.size.y,
                bounds.size.z);

        Vector3 cameraOffset =
            new Vector3(
                1f,
                1f,
                -1f).normalized;

        float distance =
            size * 2f;

        previewCamera.transform.position =
            bounds.center +
            cameraOffset * distance;

        previewCamera.transform.LookAt(
            bounds.center);

        previewCamera.fieldOfView =
            40f;

        previewCamera.nearClipPlane =
            0.01f;

        previewCamera.farClipPlane =
            distance * 10f;

        // --------------------------------
        // Render texture
        // --------------------------------

        RenderTexture renderTexture =
            new RenderTexture(
                imageSize,
                imageSize,
                24,
                RenderTextureFormat.ARGB32);

        renderTexture.Create();

        previewCamera.targetTexture =
            renderTexture;

        RenderTexture.active =
            renderTexture;

        // --------------------------------
        // Render
        // --------------------------------

        previewCamera.Render();

        // --------------------------------
        // Read pixels
        // --------------------------------

        Texture2D texture =
            new Texture2D(
                imageSize,
                imageSize,
                TextureFormat.RGBA32,
                false);

        texture.ReadPixels(
            new Rect(
                0,
                0,
                imageSize,
                imageSize),
            0,
            0);

        texture.Apply();

        byte[] bytes =
            texture.EncodeToPNG();

        // --------------------------------
        // File name
        // --------------------------------

        string fileName =
            room.RoomName
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace(" ", "_");

        string path =
            Path.Combine(
                outputFolder,
                fileName + ".png");

        File.WriteAllBytes(
            path,
            bytes);

        Debug.Log(
            "Generated thumbnail: " +
            path);

        // --------------------------------
        // Cleanup
        // --------------------------------

        RenderTexture.active =
            null;

        previewCamera.targetTexture =
            null;

        renderTexture.Release();

        DestroyImmediate(
            renderTexture);

        DestroyImmediate(
            texture);

        DestroyImmediate(
            instance);
    }

    private void SetLayerRecursively(
        GameObject obj,
        int layer)
    {
        obj.layer = layer;

        foreach (
            Transform child
            in obj.transform)
        {
            SetLayerRecursively(
                child.gameObject,
                layer);
        }
    }

    private void CreateOutputFolder()
    {
        if (!AssetDatabase.IsValidFolder(
            outputFolder))
        {
            string parent =
                Path.GetDirectoryName(
                    outputFolder);

            string folder =
                Path.GetFileName(
                    outputFolder);

            AssetDatabase.CreateFolder(
                parent,
                folder);
        }
    }
}

#endif