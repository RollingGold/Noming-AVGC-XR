using UnityEngine;

public static class RoomBuilder
{
    // =========================================================
    // BUILD ROOM
    // =========================================================

    public static Room Build(
        RoomConnector connector,
        Room prefab,
        float extraRotationY = 0f)
    {
        if (connector == null)
        {
            Debug.LogError(
                "RoomBuilder: Connector is null.");

            return null;
        }

        if (prefab == null)
        {
            Debug.LogError(
                "RoomBuilder: Prefab is null.");

            return null;
        }

        if (connector.Occupied)
        {
            Debug.LogWarning(
                "RoomBuilder: Connector is already occupied.");

            return null;
        }


        // =====================================================
        // CREATE ROOM
        // =====================================================

        Room room =
            Object.Instantiate(prefab);

        if (room == null)
        {
            Debug.LogError(
                "RoomBuilder: Failed to instantiate room.");

            return null;
        }


        // =====================================================
        // FIND AVAILABLE CONNECTOR
        // =====================================================

        RoomConnector targetConnector =
            GetAvailableConnector(room);

        if (targetConnector == null)
        {
            Debug.LogWarning(
                "RoomBuilder: " +
                prefab.RoomName +
                " has no available connectors.");

            Object.Destroy(room.gameObject);

            return null;
        }


        // =====================================================
        // GET TARGET DIRECTION
        // =====================================================

        Vector3 targetForward =
            -connector.DirectionVector;

        targetForward.y = 0f;

        if (targetForward.sqrMagnitude < 0.001f)
        {
            Debug.LogError(
                "RoomBuilder: Invalid target connector direction.");

            Object.Destroy(room.gameObject);

            return null;
        }

        targetForward.Normalize();


        // =====================================================
        // GET SOURCE DIRECTION
        // =====================================================

        Vector3 sourceForward =
            room.transform.InverseTransformDirection(
                targetConnector.DirectionVector);

        sourceForward.y = 0f;

        if (sourceForward.sqrMagnitude < 0.001f)
        {
            Debug.LogError(
                "RoomBuilder: Invalid source connector direction.");

            Object.Destroy(room.gameObject);

            return null;
        }

        sourceForward.Normalize();


        // =====================================================
        // GET UP DIRECTION
        // =====================================================

        Vector3 sourceUp =
            GetUpDirection(room);


        // =====================================================
        // CALCULATE ROTATION
        // =====================================================

        Quaternion sourceRotation =
            Quaternion.LookRotation(
                sourceForward,
                sourceUp);

        Quaternion targetRotation =
            Quaternion.LookRotation(
                targetForward,
                Vector3.up);

        Quaternion rotation =
            targetRotation *
            Quaternion.Inverse(
                sourceRotation);

        room.transform.rotation =
            rotation *
            room.transform.rotation;


        // =====================================================
        // EXTRA ROTATION
        // =====================================================

        if (Mathf.Abs(extraRotationY) > 0.01f)
        {
            room.transform.Rotate(
                Vector3.up,
                extraRotationY,
                Space.World);
        }


        // =====================================================
        // SNAP ROOM TO CONNECTOR
        // =====================================================

        Vector3 positionOffset =
            targetConnector.AnchorPoint.position -
            room.transform.position;

        room.transform.position =
            connector.AnchorPoint.position -
            positionOffset;


        // =====================================================
        // PREVIOUS ROOM
        // =====================================================

        Room previousRoom =
            connector.GetComponentInParent<Room>();


        // =====================================================
        // CONNECT
        // =====================================================

        connector.Occupied = true;

        targetConnector.Occupied = true;

        connector.ConnectedRoom =
            room;

        targetConnector.ConnectedRoom =
            previousRoom;


        // =====================================================
        // BUILD HISTORY
        // =====================================================

        room.SetBuildHistory(
            previousRoom,
            connector,
            prefab);


        Debug.Log(
            "Room built: " +
            prefab.RoomName +
            " using connector " +
            targetConnector.name);


        return room;
    }


    // =========================================================
    // CHECK ROOM OVERLAP
    // =========================================================

    public static bool IsOverlapping(
        Room room,
        Room ignoreRoom)
    {
        if (room == null)
            return true;


        // -----------------------------------------------------
        // ROOM BOUNDS
        // -----------------------------------------------------

        Collider roomBounds =
            room.RoomBounds;

        if (roomBounds == null)
        {
            Debug.LogWarning(
                "RoomBuilder: Room Bounds is missing on " +
                room.RoomName);

            return true;
        }


        // -----------------------------------------------------
        // BOX COLLIDER
        // -----------------------------------------------------

        BoxCollider box =
            roomBounds as BoxCollider;

        if (box == null)
        {
            Debug.LogWarning(
                "RoomBuilder: Room Bounds must be a BoxCollider on " +
                room.RoomName);

            return true;
        }


        // -----------------------------------------------------
        // WORLD SPACE BOX
        // -----------------------------------------------------

        Vector3 center =
            box.transform.TransformPoint(
                box.center);

        Vector3 halfExtents =
            Vector3.Scale(
                box.size * 0.5f,
                box.transform.lossyScale);

        Quaternion rotation =
            box.transform.rotation;


        // -----------------------------------------------------
        // FIND COLLIDERS
        // -----------------------------------------------------

        Collider[] hits =
            Physics.OverlapBox(
                center,
                halfExtents,
                rotation,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);


        // -----------------------------------------------------
        // CHECK COLLIDERS
        // -----------------------------------------------------

        foreach (Collider hit in hits)
        {
            if (hit == null)
                continue;


            // Ignore our own room.
            if (hit.transform.IsChildOf(
                room.transform))
            {
                continue;
            }


            Room otherRoom =
                hit.GetComponentInParent<Room>();

            if (otherRoom == null)
                continue;


            // IMPORTANT:
            // Ignore the room we are connecting to.
            //
            // The two rooms are intentionally touching
            // at their connectors.
            if (otherRoom == ignoreRoom)
            {
                continue;
            }


            // Another room was detected.
            return true;
        }


        return false;
    }


    // =========================================================
    // GET UP DIRECTION
    // =========================================================

    private static Vector3 GetUpDirection(
        Room room)
    {
        return room.Up switch
        {
            Room.UpDirection.YUp =>
                Vector3.up,

            Room.UpDirection.YDown =>
                Vector3.down,

            Room.UpDirection.XUp =>
                Vector3.right,

            Room.UpDirection.XDown =>
                Vector3.left,

            Room.UpDirection.ZUp =>
                Vector3.forward,

            Room.UpDirection.ZDown =>
                Vector3.back,

            _ =>
                Vector3.up
        };
    }


    // =========================================================
    // FIND AVAILABLE CONNECTOR
    // =========================================================

    private static RoomConnector GetAvailableConnector(
        Room room)
    {
        if (room == null)
            return null;

        foreach (
            RoomConnector connector
            in room.Connectors)
        {
            if (connector == null)
                continue;

            if (connector.Occupied)
                continue;

            return connector;
        }

        return null;
    }
}