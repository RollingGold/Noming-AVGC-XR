#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditorSelection))]
public class LevelEditorSelectionEditor : Editor
{
    // =========================================================
    // REFERENCES
    // =========================================================

    private SerializedProperty editorCamera;
    private SerializedProperty generatedRoomsRoot;


    // =========================================================
    // AUTO BUILD
    // =========================================================

    private SerializedProperty startingRoom;
    private SerializedProperty minimumRooms;
    private SerializedProperty maximumRooms;

    private SerializedProperty useDeadEnds;
    private SerializedProperty addCorridors;

    private SerializedProperty countLongCorridorChains;
    private SerializedProperty corridorChainLimit;


    // =========================================================
    // ROOM DATABASE
    // =========================================================

    private SerializedProperty roomDatabase;


    // =========================================================
    // REQUIRED ROOMS
    // =========================================================

    private SerializedProperty requiredRooms;


    // =========================================================
    // REBUILD
    // =========================================================

    private SerializedProperty infiniteRebuild;
    private SerializedProperty maxRebuildAttempts;


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        // -----------------------------------------------------
        // REFERENCES
        // -----------------------------------------------------

        editorCamera =
            serializedObject.FindProperty(
                "editorCamera");

        generatedRoomsRoot =
            serializedObject.FindProperty(
                "generatedRoomsRoot");


        // -----------------------------------------------------
        // AUTO BUILD
        // -----------------------------------------------------

        startingRoom =
            serializedObject.FindProperty(
                "startingRoom");

        minimumRooms =
            serializedObject.FindProperty(
                "minimumRooms");

        maximumRooms =
            serializedObject.FindProperty(
                "maximumRooms");

        useDeadEnds =
            serializedObject.FindProperty(
                "useDeadEnds");

        addCorridors =
            serializedObject.FindProperty(
                "addCorridors");

        countLongCorridorChains =
            serializedObject.FindProperty(
                "countLongCorridorChains");

        corridorChainLimit =
            serializedObject.FindProperty(
                "corridorChainLimit");


        // -----------------------------------------------------
        // ROOM DATABASE
        // -----------------------------------------------------

        roomDatabase =
            serializedObject.FindProperty(
                "roomDatabase");


        // -----------------------------------------------------
        // REQUIRED ROOMS
        // -----------------------------------------------------

        requiredRooms =
            serializedObject.FindProperty(
                "requiredRooms");


        // -----------------------------------------------------
        // REBUILD
        // -----------------------------------------------------

        infiniteRebuild =
            serializedObject.FindProperty(
                "infiniteRebuild");

        maxRebuildAttempts =
            serializedObject.FindProperty(
                "maxRebuildAttempts");
    }


    // =========================================================
    // INSPECTOR
    // =========================================================

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        // =====================================================
        // REFERENCES
        // =====================================================

        EditorGUILayout.LabelField(
            "References",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            editorCamera,
            new GUIContent(
                "Editor Camera"));

        EditorGUILayout.PropertyField(
            generatedRoomsRoot,
            new GUIContent(
                "Generated Rooms Root"));


        // =====================================================
        // ROOM DATABASE
        // =====================================================

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Room Database",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            roomDatabase,
            new GUIContent(
                "Room Database"));


        // =====================================================
        // AUTO BUILD
        // =====================================================

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Auto Build",
            EditorStyles.boldLabel);


        // -----------------------------------------------------
        // STARTING ROOM
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            startingRoom,
            new GUIContent(
                "Starting Room"));


        // -----------------------------------------------------
        // MINIMUM ROOMS
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            minimumRooms,
            new GUIContent(
                "Minimum Rooms"));


        // -----------------------------------------------------
        // MAXIMUM ROOMS
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            maximumRooms,
            new GUIContent(
                "Maximum Rooms"));


        // -----------------------------------------------------
        // DEAD ENDS
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            useDeadEnds,
            new GUIContent(
                "Use Dead Ends"));


        // -----------------------------------------------------
        // CORRIDORS
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            addCorridors,
            new GUIContent(
                "Add Corridors"));


        // -----------------------------------------------------
        // LONG CORRIDOR CHAINS
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            countLongCorridorChains,
            new GUIContent(
                "Count Long Corridor Chains"));


        // -----------------------------------------------------
        // CORRIDOR CHAIN LIMIT
        // -----------------------------------------------------

        if (countLongCorridorChains.boolValue)
        {
            EditorGUILayout.PropertyField(
                corridorChainLimit,
                new GUIContent(
                    "Corridor Chain Limit"));
        }


        // =====================================================
        // REQUIRED ROOMS
        // =====================================================

        EditorGUILayout.Space(10);

        DrawRequiredRooms();


        // =====================================================
        // REBUILD
        // =====================================================

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Rebuild",
            EditorStyles.boldLabel);


        // -----------------------------------------------------
        // INFINITE REBUILD
        // -----------------------------------------------------

        EditorGUILayout.PropertyField(
            infiniteRebuild,
            new GUIContent(
                "Infinite Rebuild"));


        // -----------------------------------------------------
        // MAX REBUILD ATTEMPTS
        // -----------------------------------------------------

        if (!infiniteRebuild.boolValue)
        {
            EditorGUILayout.PropertyField(
                maxRebuildAttempts,
                new GUIContent(
                    "Max Rebuild Attempts"));
        }


        // -----------------------------------------------------
        // INFO
        // -----------------------------------------------------

        if (infiniteRebuild.boolValue)
        {
            EditorGUILayout.HelpBox(
                "Auto Build will keep generating new maps " +
                "until the minimum room count and all required " +
                "rooms are satisfied.",
                MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Auto Build will stop after the specified number " +
                "of failed rebuild attempts.",
                MessageType.Info);
        }


        // =====================================================
        // APPLY
        // =====================================================

        serializedObject.ApplyModifiedProperties();
    }


    // =========================================================
    // REQUIRED ROOMS UI
    // =========================================================

    private void DrawRequiredRooms()
    {
        EditorGUILayout.LabelField(
            "Required Rooms",
            EditorStyles.boldLabel);


        EditorGUILayout.HelpBox(
            "These rooms must appear in the generated map. " +
            "If one is missing, Auto Build will rebuild the map.",
            MessageType.Info);


        EditorGUILayout.Space(4);


        // =====================================================
        // EXISTING REQUIRED ROOMS
        // =====================================================

        for (
            int i = 0;
            i < requiredRooms.arraySize;
            i++)
        {
            SerializedProperty element =
                requiredRooms.GetArrayElementAtIndex(
                    i);


            Room room =
                element.objectReferenceValue
                as Room;


            EditorGUILayout.BeginHorizontal();


            // -------------------------------------------------
            // ROOM NAME
            // -------------------------------------------------

            string roomName =
                room != null
                    ? room.RoomName
                    : "Missing Room";


            // -------------------------------------------------
            // ROOM SELECTION BUTTON
            // -------------------------------------------------

            if (GUILayout.Button(
                roomName,
                EditorStyles.objectField))
            {
                ShowRoomMenu(
                    element);
            }


            // -------------------------------------------------
            // REMOVE BUTTON
            // -------------------------------------------------

            if (GUILayout.Button(
                "X",
                GUILayout.Width(25)))
            {
                requiredRooms.DeleteArrayElementAtIndex(
                    i);

                serializedObject.ApplyModifiedProperties();

                GUIUtility.ExitGUI();
            }


            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.Space(4);


        // =====================================================
        // ADD REQUIRED ROOM
        // =====================================================

        if (GUILayout.Button(
            "+ Add Required Room",
            GUILayout.Height(30)))
        {
            ShowAddRoomMenu();
        }
    }


    // =========================================================
    // ADD REQUIRED ROOM MENU
    // =========================================================

    private void ShowAddRoomMenu()
    {
        serializedObject.Update();


        RoomDatabase database =
            roomDatabase.objectReferenceValue
            as RoomDatabase;


        if (database == null)
        {
            EditorUtility.DisplayDialog(
                "Room Database",
                "Room Database is not assigned.",
                "OK");

            return;
        }


        if (database.rooms == null ||
            database.rooms.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Room Database",
                "Room Database contains no rooms.",
                "OK");

            return;
        }


        GenericMenu menu =
            new GenericMenu();


        foreach (
            Room room
            in database.rooms)
        {
            if (room == null)
                continue;


            bool alreadyAdded =
                ContainsRoom(
                    room);


            Room capturedRoom =
                room;


            string label =
                capturedRoom.RoomName;


            if (alreadyAdded)
            {
                label += " ✓";
            }


            menu.AddItem(
                new GUIContent(
                    label),
                false,
                () =>
                {
                    if (!ContainsRoom(
                        capturedRoom))
                    {
                        AddRequiredRoom(
                            capturedRoom);
                    }
                });
        }


        menu.ShowAsContext();
    }


    // =========================================================
    // CHANGE EXISTING REQUIRED ROOM
    // =========================================================

    private void ShowRoomMenu(
        SerializedProperty property)
    {
        RoomDatabase database =
            roomDatabase.objectReferenceValue
            as RoomDatabase;


        if (database == null)
        {
            EditorUtility.DisplayDialog(
                "Room Database",
                "Room Database is not assigned.",
                "OK");

            return;
        }


        if (database.rooms == null ||
            database.rooms.Count == 0)
        {
            return;
        }


        GenericMenu menu =
            new GenericMenu();


        foreach (
            Room room
            in database.rooms)
        {
            if (room == null)
                continue;


            Room capturedRoom =
                room;


            bool selected =
                property.objectReferenceValue ==
                capturedRoom;


            menu.AddItem(
                new GUIContent(
                    capturedRoom.RoomName),
                selected,
                () =>
                {
                    property.objectReferenceValue =
                        capturedRoom;


                    serializedObject.ApplyModifiedProperties();


                    EditorUtility.SetDirty(
                        target);


                    Repaint();
                });
        }


        menu.ShowAsContext();
    }


    // =========================================================
    // CHECK IF ROOM ALREADY EXISTS
    // =========================================================

    private bool ContainsRoom(
        Room room)
    {
        if (room == null)
            return false;


        for (
            int i = 0;
            i < requiredRooms.arraySize;
            i++)
        {
            SerializedProperty element =
                requiredRooms.GetArrayElementAtIndex(
                    i);


            if (element.objectReferenceValue ==
                room)
            {
                return true;
            }
        }


        return false;
    }


    // =========================================================
    // ADD REQUIRED ROOM
    // =========================================================

    private void AddRequiredRoom(
        Room room)
    {
        if (room == null)
            return;


        if (ContainsRoom(
            room))
        {
            return;
        }


        serializedObject.Update();


        int index =
            requiredRooms.arraySize;


        requiredRooms.InsertArrayElementAtIndex(
            index);


        SerializedProperty element =
            requiredRooms.GetArrayElementAtIndex(
                index);


        element.objectReferenceValue =
            room;


        serializedObject.ApplyModifiedProperties();


        EditorUtility.SetDirty(
            target);


        Repaint();
    }
}

#endif