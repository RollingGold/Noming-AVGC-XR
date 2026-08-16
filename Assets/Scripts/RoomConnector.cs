using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    [Header("Connector")]
    [SerializeField] private Direction direction;
    [SerializeField] private bool occupied;

    [Header("Snap Point")]
    [SerializeField] private Transform anchorPoint;

    [Header("Selection")]
    [SerializeField] private Material highlightMaterial;

    [Header("Selection")]
    [SerializeField] private GameObject selectionIndicator;

    
    private Renderer[] plusRenderers;
    private Material[][] originalMaterials;

    private Room connectedRoom;

    public Direction Facing => direction;

    public bool Occupied
    {
        get => occupied;
        set => occupied = value;
    }

    public Transform AnchorPoint => anchorPoint;

    public Room ConnectedRoom
    {
        get => connectedRoom;
        set => connectedRoom = value;
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (plusRenderers == null ||
            plusRenderers.Length == 0)
        {
            CacheRenderers();
        }

        bool visible = !occupied;

        foreach (Renderer renderer in plusRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }
    public Vector3 DirectionVector
    {
        get
        {
            return direction switch
            {
                Direction.North =>
                    transform.forward,

                Direction.South =>
                    -transform.forward,

                Direction.East =>
                    transform.right,

                Direction.West =>
                    -transform.right,

                _ =>
                    transform.forward
            };
        }
    }

    private void Awake()
    {
        CacheRenderers();
    }

    private void CacheRenderers()
    {
        plusRenderers =
            GetComponentsInChildren<Renderer>(true);

        originalMaterials =
            new Material[plusRenderers.Length][];

        for (int i = 0;
             i < plusRenderers.Length;
             i++)
        {
            originalMaterials[i] =
                plusRenderers[i].materials;
        }
    }

    private void Reset()
    {
        if (anchorPoint == null)
            anchorPoint = transform;
    }

    // ========================================
    // SELECTION
    // ========================================

    public void SetSelected(bool selected)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }
    }



#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color =
            occupied
                ? Color.red
                : Color.green;

        Vector3 start =
            transform.position;

        Vector3 end =
            start +
            DirectionVector * 2f;

        Gizmos.DrawSphere(
            start,
            0.15f);

        Gizmos.DrawLine(
            start,
            end);
    }

#endif
}