using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class LevelEditorSelection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera editorCamera;
    [SerializeField] private Transform generatedRoomsRoot;

    [Header("Room Database")]
    [SerializeField] private RoomDatabase roomDatabase;

    [Header("Auto Build")]
    [SerializeField] private Room startingRoom;

    [Min(1)]
    [SerializeField] private int minimumRooms = 8;

    [Min(1)]
    [SerializeField] private int maximumRooms = 20;

    [SerializeField] private bool useDeadEnds = true;
    [SerializeField] private bool addCorridors = true;
    [SerializeField] private bool countLongCorridorChains = true;

    [Min(0)]
    [SerializeField] private int corridorChainLimit = 3;

    [Header("Required Rooms")]
    [SerializeField] private List<Room> requiredRooms = new List<Room>();

    [Header("Rebuild")]
    [SerializeField] private bool infiniteRebuild = false;

    [Min(1)]
    [SerializeField] private int maxRebuildAttempts = 3;

    public RoomDatabase RoomDatabase => roomDatabase;
    public Room StartingRoom => startingRoom;

    public bool IsAutoBuildRunning => autoBuildRunning;

    private Coroutine autoBuildCoroutine;
    private bool autoBuildRunning;
    private int currentBuildAttempt;

    // Only rooms created by Auto Build are stored here.
    private readonly HashSet<Room> autoBuiltRooms = new HashSet<Room>();

    private InputSystem_Actions input;

    private Room selectedPrefab;
    private Room selectedRoom;
    private RoomConnector selectedConnector;

    private bool selectRequested;
    private Vector2 selectMousePosition;

    private void OnValidate()
    {
        minimumRooms = Mathf.Max(1, minimumRooms);
        maximumRooms = Mathf.Max(minimumRooms, maximumRooms);
        corridorChainLimit = Mathf.Max(0, corridorChainLimit);
        maxRebuildAttempts = Mathf.Max(1, maxRebuildAttempts);
    }

    private void Awake()
    {
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        if (input == null)
            input = new InputSystem_Actions();

        input.Enable();
        input.Editor.Select.performed += OnSelect;
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Editor.Select.performed -= OnSelect;
            input.Disable();
        }

        ClearAllSelection();
    }

    private void Update()
    {
        if (!selectRequested)
            return;

        selectRequested = false;

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
            return;

        SelectWorldObject(selectMousePosition);
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        selectMousePosition =
            input.Editor.MousePosition.ReadValue<Vector2>();

        selectRequested = true;
    }

    private void SelectWorldObject(Vector2 mousePosition)
    {
        if (editorCamera == null)
            return;

        Ray ray = editorCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            ClearAllSelection();
            return;
        }

        RoomConnector connector =
            hit.collider.GetComponentInParent<RoomConnector>();

        if (connector != null)
        {
            SelectConnector(connector);
            return;
        }

        Room room =
            hit.collider.GetComponentInParent<Room>();

        if (room != null)
        {
            SelectRoom(room);
            return;
        }

        ClearAllSelection();
    }

    private void SelectRoom(Room room)
    {
        if (room == null)
            return;

        ClearAllSelection();

        selectedRoom = room;
        selectedRoom.SetSelected(true);

        SelectNextAvailableConnector(room);

        Debug.Log("Selected Room: " + room.RoomName);
    }

    private void SelectConnector(RoomConnector connector)
    {
        if (connector == null || connector.Occupied)
            return;

        if (selectedConnector != null &&
            selectedConnector != connector)
        {
            selectedConnector.SetSelected(false);
        }

        if (selectedRoom != null)
        {
            selectedRoom.SetSelected(false);
            selectedRoom = null;
        }

        selectedConnector = connector;
        selectedConnector.SetSelected(true);
    }

    private void SelectNextAvailableConnector(Room room)
    {
        if (room == null)
            return;

        selectedConnector = null;

        foreach (RoomConnector connector in room.Connectors)
        {
            if (connector == null || connector.Occupied)
                continue;

            selectedConnector = connector;
            selectedConnector.SetSelected(true);
            return;
        }
    }

    // =========================================================
    // INDICATORS
    // =========================================================

    private void SetConnectorIndicator(
        RoomConnector connector,
        bool enabled)
    {
        if (connector == null)
            return;

        connector.SetSelected(enabled);
    }

    private void RefreshConnectorIndicators()
    {
        foreach (Room room in autoBuiltRooms)
        {
            if (room == null)
                continue;

            foreach (RoomConnector connector in room.Connectors)
            {
                if (connector == null)
                    continue;

                // + is visible only on an available connector.
                // Occupied connectors are always hidden.
                connector.SetSelected(!connector.Occupied);
            }
        }

        // Selection has priority over the general indicator state.
        if (selectedConnector != null &&
            !selectedConnector.Occupied)
        {
            selectedConnector.SetSelected(true);
        }
    }

    // =========================================================
    // MANUAL BUILD
    // =========================================================

    public void BuildSelectedRoom(Room prefab)
    {
        if (prefab == null || selectedConnector == null)
            return;

        if (selectedConnector.Occupied)
            return;

        RoomConnector connector = selectedConnector;

        Room previousRoom =
            connector.GetComponentInParent<Room>();

        Room newRoom =
            RoomBuilder.Build(connector, prefab);

        if (newRoom == null)
            return;

        if (generatedRoomsRoot != null)
            newRoom.transform.SetParent(
                generatedRoomsRoot, true);

        Physics.SyncTransforms();

        if (IsRoomOverlappingExistingRooms(
            newRoom, previousRoom, false))
        {
            ResetFailedBuild(connector, newRoom);
            DestroyRoom(newRoom);

            Debug.LogWarning(
                "Manual room rejected because it overlaps another room.");

            return;
        }

        // The connector used by the new room is no longer a +.
        SetConnectorIndicator(connector, false);

        SelectRoom(newRoom);
    }

    // =========================================================
    // AUTO BUILD
    // =========================================================

    public void AutoBuild()
    {
        if (autoBuildRunning)
        {
            Debug.LogWarning("Auto Build is already running.");
            return;
        }

        if (roomDatabase == null)
        {
            Debug.LogError(
                "Auto Build: Room Database is not assigned.");
            return;
        }

        if (roomDatabase.rooms == null ||
            roomDatabase.rooms.Count == 0)
        {
            Debug.LogError(
                "Auto Build: Room Database contains no rooms.");
            return;
        }

        if (startingRoom == null)
        {
            Debug.LogError(
                "Auto Build: Starting Room is not assigned.");
            return;
        }

        if (generatedRoomsRoot == null)
        {
            Debug.LogError(
                "Auto Build: Generated Rooms Root is not assigned.");
            return;
        }

        StopAutoBuild();
        autoBuildCoroutine = StartCoroutine(AutoBuildRoutine());
    }

    public void StopAutoBuild()
    {
        if (autoBuildCoroutine != null)
        {
            StopCoroutine(autoBuildCoroutine);
            autoBuildCoroutine = null;
        }

        autoBuildRunning = false;
    }

    private IEnumerator AutoBuildRoutine()
    {
        autoBuildRunning = true;
        currentBuildAttempt = 0;

        while (true)
        {
            currentBuildAttempt++;

            ClearAutoBuild();
            yield return null;

            Room root = CreateStartingRoom();

            if (root == null)
                break;

            yield return null;
            Physics.SyncTransforms();

            if (GetAvailableConnectorsFromRoom(root).Count == 0)
            {
                Debug.LogWarning(
                    "Starting Room has no available connectors.");

                ClearAutoBuild();
                yield return null;

                if (!infiniteRebuild &&
                    currentBuildAttempt >= maxRebuildAttempts)
                    break;

                continue;
            }

            int result = GenerateMap();

            if (result < 0)
            {
                Debug.LogWarning("Auto Build attempt failed.");

                ClearAutoBuild();
                yield return null;

                if (!infiniteRebuild &&
                    currentBuildAttempt >= maxRebuildAttempts)
                    break;

                continue;
            }

            int countedRooms = GetCountedRoomCount();

            if (countedRooms < minimumRooms)
            {
                Debug.LogWarning(
                    "Minimum room count not reached: " +
                    countedRooms + " / " + minimumRooms);

                ClearAutoBuild();
                yield return null;

                if (!infiniteRebuild &&
                    currentBuildAttempt >= maxRebuildAttempts)
                    break;

                continue;
            }

            // Required rooms are checked before dead ends are created.
            if (!HasAllRequiredRooms())
            {
                Debug.LogWarning("Required room(s) missing.");

                ClearAutoBuild();
                yield return null;

                if (!infiniteRebuild &&
                    currentBuildAttempt >= maxRebuildAttempts)
                    break;

                continue;
            }

            if (useDeadEnds)
                CloseRemainingConnectors();

            RefreshConnectorIndicators();

            Debug.Log(
                "AUTO BUILD SUCCESS | Attempt: " +
                currentBuildAttempt +
                " | Counted Rooms: " +
                GetCountedRoomCount() +
                " | Generated Rooms: " +
                autoBuiltRooms.Count);

            break;
        }

        autoBuildRunning = false;
        autoBuildCoroutine = null;
    }

    // =========================================================
    // STARTING ROOM
    // =========================================================

    private Room CreateStartingRoom()
    {
        if (startingRoom == null)
            return null;

        Room room = Instantiate(startingRoom);

        if (room == null)
            return null;

        room.transform.position =
            startingRoom.transform.position;

        room.transform.rotation =
            startingRoom.transform.rotation;

        room.transform.localScale =
            startingRoom.transform.localScale;

        room.transform.SetParent(
            generatedRoomsRoot, true);

        room.SetBuildHistory(
            null,
            null,
            startingRoom);

        ResetRoomConnectors(room);

        autoBuiltRooms.Add(room);

        // Starting room is not selected, but all of its free
        // connectors show their + indicator.
        RefreshConnectorIndicators();

        Debug.Log(
            "AUTO BUILD STARTING ROOM CREATED: " +
            room.RoomName);

        return room;
    }

    // =========================================================
    // GENERATION
    // =========================================================

    private int GenerateMap()
    {
        const int safetyLimit = 1000;
        int safety = 0;

        while (safety < safetyLimit)
        {
            safety++;

            int currentCount = GetCountedRoomCount();

            if (currentCount >= maximumRooms)
                return currentCount;

            List<RoomConnector> connectors =
                GetAllAvailableConnectors();

            if (connectors.Count == 0)
            {
                return currentCount >= minimumRooms
                    ? currentCount
                    : -1;
            }

            ShuffleConnectors(connectors);

            bool placed = false;

            foreach (RoomConnector connector in connectors)
            {
                if (connector == null ||
                    connector.Occupied)
                    continue;

                if (TryBuildRoom(connector))
                {
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                currentCount = GetCountedRoomCount();

                return currentCount >= minimumRooms
                    ? currentCount
                    : -1;
            }

            Physics.SyncTransforms();
        }

        Debug.LogWarning(
            "Auto Build reached its safety limit.");

        return -1;
    }

    private bool TryBuildRoom(RoomConnector connector)
    {
        if (connector == null || connector.Occupied)
            return false;

        Room previousRoom =
            connector.GetComponentInParent<Room>();

        if (previousRoom == null)
            return false;

        List<Room> candidates =
            new List<Room>(roomDatabase.rooms);

        ShuffleRooms(candidates);

        foreach (Room prefab in candidates)
        {
            if (prefab == null)
                continue;

            if (prefab.Type == Room.RoomType.DeadEnd)
                continue;

            if (prefab.Size == Room.RoomSize.Corridor &&
                !addCorridors)
                continue;

            if (!CanAddRoom(prefab, previousRoom))
                continue;

            Room newRoom =
                RoomBuilder.Build(
                    connector,
                    prefab);

            if (newRoom == null)
                continue;

            if (generatedRoomsRoot != null)
            {
                newRoom.transform.SetParent(
                    generatedRoomsRoot, true);
            }

            Physics.SyncTransforms();

            if (IsRoomOverlappingExistingRooms(
                newRoom, previousRoom, false))
            {
                ResetFailedBuild(connector, newRoom);
                DestroyRoom(newRoom);
                continue;
            }

            autoBuiltRooms.Add(newRoom);

            // IMPORTANT:
            // Hide the + on the connector that was consumed.
            SetConnectorIndicator(connector, false);

            // Show + on the new room's unused connectors.
            RefreshConnectorIndicators();

            Debug.Log(
                "Auto Build placed: " +
                newRoom.RoomName);

            return true;
        }

        return false;
    }

    // =========================================================
    // ROOM COUNT
    // =========================================================

    private bool CanAddRoom(
        Room prefab,
        Room previousRoom)
    {
        if (prefab == null)
            return false;

        int currentCount =
            GetCountedRoomCount();

        if (prefab.Size != Room.RoomSize.Corridor)
            return currentCount < maximumRooms;

        if (!countLongCorridorChains)
            return true;

        int previousChain =
            GetPreviousCorridorChainLength(previousRoom);

        int newPosition =
            previousChain + 1;

        if (newPosition <= corridorChainLimit)
            return true;

        return currentCount < maximumRooms;
    }

    private int GetPreviousCorridorChainLength(Room room)
    {
        int count = 0;
        Room current = room;

        while (current != null)
        {
            if (current.Size != Room.RoomSize.Corridor)
                break;

            count++;
            current = current.PreviousRoom;
        }

        return count;
    }

    private int GetCountedRoomCount()
    {
        int count = 0;

        foreach (Room room in autoBuiltRooms)
        {
            if (room == null)
                continue;

            if (room.Type == Room.RoomType.DeadEnd)
                continue;

            if (room.Size != Room.RoomSize.Corridor)
            {
                count++;
                continue;
            }

            if (!countLongCorridorChains)
                continue;

            int chain =
                GetPreviousCorridorChainLength(room);

            if (chain > corridorChainLimit)
                count++;
        }

        return count;
    }

    // =========================================================
    // REQUIRED ROOMS
    // =========================================================

    private bool HasAllRequiredRooms()
    {
        if (requiredRooms == null ||
            requiredRooms.Count == 0)
            return true;

        foreach (Room required in requiredRooms)
        {
            if (required == null)
                continue;

            bool found = false;

            foreach (Room generated in autoBuiltRooms)
            {
                if (generated == null)
                    continue;

                if (generated.SourcePrefab == required)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.Log(
                    "Required room missing: " +
                    required.RoomName);

                return false;
            }
        }

        return true;
    }

    // =========================================================
    // DEAD ENDS
    // =========================================================

    private void CloseRemainingConnectors()
    {
        List<RoomConnector> connectors =
            GetAllAvailableConnectors();

        ShuffleConnectors(connectors);

        foreach (RoomConnector connector in connectors)
        {
            if (connector == null ||
                connector.Occupied)
                continue;

            TryBuildDeadEnd(connector);
        }

        RefreshConnectorIndicators();
    }

    private bool TryBuildDeadEnd(
        RoomConnector connector)
    {
        if (connector == null)
            return false;

        if (connector.Occupied)
            return false;

        List<Room> deadEnds =
            new List<Room>();

        foreach (Room prefab in roomDatabase.rooms)
        {
            if (prefab == null)
                continue;

            if (prefab.Type ==
                Room.RoomType.DeadEnd)
            {
                deadEnds.Add(prefab);
            }
        }

        ShuffleRooms(deadEnds);

        foreach (Room prefab in deadEnds)
        {
            Room deadEnd =
                RoomBuilder.Build(
                    connector,
                    prefab);

            if (deadEnd == null)
                continue;

            deadEnd.transform.SetParent(
                generatedRoomsRoot,
                true);

            // Dead Ends intentionally do NOT use collision checking.
            // They simply close the connector.
            autoBuiltRooms.Add(
                deadEnd);

            // The connector used by the dead end is occupied.
            connector.SetSelected(false);

            // Dead ends expose no usable + indicators.
            foreach (RoomConnector deadEndConnector in deadEnd.Connectors)
            {
                deadEndConnector.SetSelected(false);
            }

            Debug.Log(
                "Dead End placed: " +
                prefab.RoomName);

            return true;
        }

        return false;
    }


    private bool IsRoomOverlappingExistingRooms(
        Room newRoom,
        Room ignoredRoom,
        bool includeIgnoredRoom)
    {
        if (newRoom == null)
            return true;

        Collider newCollider =
            newRoom.RoomBounds;

        if (newCollider == null)
        {
            Debug.LogWarning(
                "Room '" +
                newRoom.RoomName +
                "' has no Room Bounds collider.");

            return false;
        }

        Physics.SyncTransforms();

        foreach (Room existingRoom in autoBuiltRooms)
        {
            if (existingRoom == null ||
                existingRoom == newRoom)
                continue;

            if (!includeIgnoredRoom &&
                existingRoom == ignoredRoom)
                continue;

            Collider existingCollider =
                existingRoom.RoomBounds;

            if (existingCollider == null)
                continue;

            if (!newCollider.bounds.Intersects(
                existingCollider.bounds))
                continue;

            if (Physics.ComputePenetration(
                newCollider,
                newCollider.transform.position,
                newCollider.transform.rotation,
                existingCollider,
                existingCollider.transform.position,
                existingCollider.transform.rotation,
                out _,
                out float distance))
            {
                if (distance > 0.001f)
                    return true;
            }
        }

        return false;
    }

    // =========================================================
    // CONNECTORS
    // =========================================================

    private List<RoomConnector>
        GetAllAvailableConnectors()
    {
        List<RoomConnector> result =
            new List<RoomConnector>();

        foreach (Room room in autoBuiltRooms)
        {
            if (room == null)
                continue;

            foreach (RoomConnector connector in room.Connectors)
            {
                if (connector == null ||
                    connector.Occupied)
                    continue;

                result.Add(connector);
            }
        }

        return result;
    }

    private List<RoomConnector>
        GetAvailableConnectorsFromRoom(Room room)
    {
        List<RoomConnector> result =
            new List<RoomConnector>();

        if (room == null)
            return result;

        foreach (RoomConnector connector in room.Connectors)
        {
            if (connector == null ||
                connector.Occupied)
                continue;

            result.Add(connector);
        }

        return result;
    }

    private void ResetRoomConnectors(Room room)
    {
        if (room == null)
            return;

        foreach (RoomConnector connector in room.Connectors)
        {
            if (connector == null)
                continue;

            connector.Occupied = false;
            connector.ConnectedRoom = null;

            // New starting room connector is available,
            // therefore its + is visible.
            connector.SetSelected(true);
        }
    }

    private void ResetFailedBuild(
        RoomConnector connector,
        Room room)
    {
        if (connector != null)
        {
            connector.Occupied = false;
            connector.ConnectedRoom = null;
            connector.SetSelected(true);
        }

        if (room == null)
            return;

        // Disable the bounds collider immediately.
        // This prevents a failed room, waiting for Destroy(),
        // from interfering with the next collision test.
        if (room.RoomBounds != null)
            room.RoomBounds.enabled = false;

        foreach (RoomConnector roomConnector in room.Connectors)
        {
            if (roomConnector == null)
                continue;

            roomConnector.Occupied = false;
            roomConnector.ConnectedRoom = null;
            roomConnector.SetSelected(false);
        }
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void ClearAutoBuild()
    {
        if (autoBuiltRooms.Count == 0)
            return;

        ClearAllSelection();

        List<Room> roomsToDelete =
            new List<Room>(autoBuiltRooms);

        autoBuiltRooms.Clear();

        foreach (Room room in roomsToDelete)
        {
            if (room == null)
                continue;

            ClearRoomConnectors(room);
            DestroyRoom(room);
        }

        selectedRoom = null;
        selectedConnector = null;
    }

    private void ClearRoomConnectors(Room room)
    {
        if (room == null)
            return;

        foreach (RoomConnector connector in room.Connectors)
        {
            if (connector == null)
                continue;

            connector.Occupied = false;
            connector.ConnectedRoom = null;
            connector.SetSelected(false);
        }
    }

    private void DestroyRoom(Room room)
    {
        if (room == null)
            return;

        if (Application.isPlaying)
            Destroy(room.gameObject);
        else
            DestroyImmediate(room.gameObject);
    }

    // =========================================================
    // SHUFFLE
    // =========================================================

    private void ShuffleRooms(List<Room> rooms)
    {
        for (int i = rooms.Count - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            Room temp = rooms[i];
            rooms[i] = rooms[randomIndex];
            rooms[randomIndex] = temp;
        }
    }

    private void ShuffleConnectors(
        List<RoomConnector> connectors)
    {
        for (int i = connectors.Count - 1; i > 0; i--)
        {
            int randomIndex =
                Random.Range(0, i + 1);

            RoomConnector temp = connectors[i];
            connectors[i] = connectors[randomIndex];
            connectors[randomIndex] = temp;
        }
    }

    // =========================================================
    // SELECTION
    // =========================================================

    private void ClearRoomHighlight()
    {
        if (selectedRoom != null)
            selectedRoom.SetSelected(false);
    }

    private void ClearConnectorSelection()
    {
        if (selectedConnector != null)
        {
            // Do not hide an available connector permanently.
            // Refreshing the indicators will restore its +.
            selectedConnector.SetSelected(false);
        }

        selectedConnector = null;
    }

    private void ClearAllSelection()
    {
        ClearRoomHighlight();
        ClearConnectorSelection();
        selectedRoom = null;
    }

    // =========================================================
    // DELETE SELECTED ROOM
    // =========================================================

    public void DeleteSelectedRoom()
    {
        if (selectedRoom == null)
            return;

        if (selectedRoom.PreviousRoom == null)
        {
            Debug.LogWarning(
                "Cannot delete the starting room.");
            return;
        }

        Room room = selectedRoom;
        Room previousRoom = room.PreviousRoom;
        RoomConnector previousConnector =
            room.PreviousConnector;

        DeleteRoomChain(room);

        ClearAllSelection();

        if (previousRoom != null)
        {
            selectedRoom = previousRoom;
            selectedRoom.SetSelected(true);

            if (previousConnector != null &&
                !previousConnector.Occupied)
            {
                selectedConnector =
                    previousConnector;

                selectedConnector.SetSelected(true);
            }
        }

        RefreshConnectorIndicators();
    }

    private void DeleteRoomChain(Room room)
    {
        if (room == null)
            return;

        List<Room> children =
            new List<Room>();

        foreach (RoomConnector connector in room.Connectors)
        {
            if (connector == null)
                continue;

            Room child = connector.ConnectedRoom;

            if (child == null ||
                child == room.PreviousRoom)
                continue;

            if (child.PreviousRoom != room)
                continue;

            children.Add(child);
        }

        foreach (Room child in children)
            DeleteRoomChain(child);

        if (room.PreviousConnector != null)
        {
            room.PreviousConnector.Occupied = false;
            room.PreviousConnector.ConnectedRoom = null;
            room.PreviousConnector.SetSelected(true);
        }

        ClearRoomConnectors(room);
        DestroyRoom(room);
    }

    // =========================================================
    // PUBLIC GETTERS
    // =========================================================

    public Room GetSelectedRoom()
    {
        return selectedRoom;
    }

    public RoomConnector GetSelectedConnector()
    {
        return selectedConnector;
    }

    public Room GetSelectedPrefab()
    {
        return selectedPrefab;
    }

    public bool HasRoomSelection()
    {
        return selectedRoom != null;
    }

    public bool HasConnectorSelection()
    {
        return selectedConnector != null;
    }

    public bool HasPrefabSelection()
    {
        return selectedPrefab != null;
    }
}