using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        DeadEnd,
        Boss,
        Treasure,
        Shop,
        Event,
        Stair,
        Secret
    }

    public enum RoomSize
    {
        Small,
        Medium,
        Large,
        Corridor
    }

    public enum UpDirection
    {
        YUp,
        YDown,
        XUp,
        XDown,
        ZUp,
        ZDown
    }

    [Header("Orientation")]
    [SerializeField]
    private UpDirection upDirection = UpDirection.YUp;

    [Header("Room Info")]
    [SerializeField]
    private string roomName = "New Room";

    [SerializeField]
    private RoomType roomType = RoomType.Normal;

    [SerializeField]
    private RoomSize roomSize = RoomSize.Small;

    [Header("Connectors")]
    [SerializeField]
    private List<RoomConnector> connectors = new();

    [Header("Spawn Points")]
    [SerializeField]
    private Transform[] enemySpawnPoints;

    [SerializeField]
    private Transform[] chestSpawnPoints;

    [SerializeField]
    private Transform[] playerSpawnPoints;

    [Header("Room Bounds")]
    [SerializeField]
    private BoxCollider roomBounds;

    [Header("Editor")]
    [SerializeField]
    private Transform visualRoot;

    [Header("UI")]
    [SerializeField]
    private Sprite thumbnail;

    [Header("Selection")]
    [SerializeField] private GameObject selectionIndicator;

   

    private Room previousRoom;
    private RoomConnector previousConnector;
    private Room sourcePrefab;


    public BoxCollider RoomBounds => roomBounds;

    public Room PreviousRoom => previousRoom;

    public RoomConnector PreviousConnector => previousConnector;

    public Room SourcePrefab => sourcePrefab;

    public void SetBuildHistory(
        Room previousRoom,
        RoomConnector previousConnector,
        Room sourcePrefab)
    {
        this.previousRoom = previousRoom;
        this.previousConnector = previousConnector;
        this.sourcePrefab = sourcePrefab;
    }


    // ========================================
    // PUBLIC ACCESS
    // ========================================

    public bool IsDeadEnd =>
    roomType == RoomType.DeadEnd;

    public bool IsCorridor =>
        roomSize == RoomSize.Corridor;

    public Sprite Thumbnail =>
        thumbnail;

    public Transform VisualRoot =>
        visualRoot;

    public UpDirection Up =>
        upDirection;

    public string RoomName =>
        roomName;

    public RoomType Type =>
        roomType;

    public RoomSize Size =>
        roomSize;

    public IReadOnlyList<RoomConnector> Connectors =>
        connectors;

    public Transform[] EnemySpawnPoints =>
        enemySpawnPoints;

    public Transform[] ChestSpawnPoints =>
        chestSpawnPoints;

    public Transform[] PlayerSpawnPoints =>
        playerSpawnPoints;

    public Bounds Bounds
    {
        get
        {
            if (roomBounds != null)
                return roomBounds.bounds;

            return new Bounds(
                transform.position,
                Vector3.one);
        }
    }


#if UNITY_EDITOR

    // ========================================
    // VALIDATE
    // ========================================

    private void OnValidate()
    {
        connectors.Clear();

        connectors.AddRange(
            GetComponentsInChildren<RoomConnector>());
    }


    // ========================================
    // UP DIRECTION
    // ========================================

    private Vector3 GetUpVector()
    {
        return upDirection switch
        {
            UpDirection.YUp =>
                transform.up,

            UpDirection.YDown =>
                -transform.up,

            UpDirection.XUp =>
                transform.right,

            UpDirection.XDown =>
                -transform.right,

            UpDirection.ZUp =>
                transform.forward,

            UpDirection.ZDown =>
                -transform.forward,

            _ =>
                transform.up
        };
    }


    // ========================================
    // GIZMOS
    // ========================================

    private void OnDrawGizmos()
    {
        Vector3 origin =
            transform.position;

        Vector3 up =
            GetUpVector();

        float length = 3f;

        // -------------------------------
        // UP DIRECTION
        // -------------------------------

        Gizmos.color = Color.green;

        Vector3 end =
            origin +
            up * length;

        Gizmos.DrawLine(
            origin,
            end);

        Gizmos.DrawSphere(
            end,
            0.15f);

        // -------------------------------
        // ARROW HEAD
        // -------------------------------

        Vector3 side =
            Vector3.Cross(
                up,
                Vector3.forward);

        // If up is parallel to forward,
        // use right instead.
        if (side.sqrMagnitude < 0.01f)
        {
            side =
                Vector3.Cross(
                    up,
                    Vector3.right);
        }

        side.Normalize();

        Vector3 arrowLeft =
            end -
            up * 0.45f +
            side * 0.25f;

        Vector3 arrowRight =
            end -
            up * 0.45f -
            side * 0.25f;

        Gizmos.DrawLine(
            end,
            arrowLeft);

        Gizmos.DrawLine(
            end,
            arrowRight);

        // -------------------------------
        // ROOM CENTER
        // -------------------------------

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawWireSphere(
            origin,
            0.2f);
    }

#endif
    public void SetSelected(bool selected)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }
    }
}