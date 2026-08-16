using UnityEngine;

public class RoomOutline : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float lineWidth = 0.08f;

    [Header("Offset")]
    [SerializeField] private float heightOffset = 0.1f;

    private LineRenderer lineRenderer;
    private GameObject outlineObject;

    private Room currentRoom;

    private void Awake()
    {
        CreateOutline();
    }

    private void CreateOutline()
    {
        // Prevent creating multiple outlines
        outlineObject =
            new GameObject("Runtime Room Outline");

        outlineObject.transform.SetParent(
            transform,
            false);

        lineRenderer =
            outlineObject.AddComponent<LineRenderer>();

        lineRenderer.material =
            outlineMaterial;

        lineRenderer.startWidth =
            lineWidth;

        lineRenderer.endWidth =
            lineWidth;

        lineRenderer.useWorldSpace =
            true;

        lineRenderer.loop =
            true;

        lineRenderer.positionCount =
            4;

        lineRenderer.enabled =
            false;
    }

    public void Show(Room room)
    {
        if (room == null)
        {
            Hide();
            return;
        }

        currentRoom = room;

        Bounds bounds =
            GetRoomBounds(room);

        Vector3 min =
            bounds.min;

        Vector3 max =
            bounds.max;

        float y =
            max.y + heightOffset;

        Vector3[] points =
        {
            new Vector3(min.x, y, min.z),
            new Vector3(max.x, y, min.z),
            new Vector3(max.x, y, max.z),
            new Vector3(min.x, y, max.z)
        };

        // Always reset the exact number of points
        lineRenderer.positionCount = 4;

        lineRenderer.SetPositions(points);

        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        currentRoom = null;

        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }

    private Bounds GetRoomBounds(Room room)
    {
        Renderer[] renderers =
            room.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            return new Bounds(
                room.transform.position,
                Vector3.one);
        }

        Bounds bounds =
            renderers[0].bounds;

        for (int i = 1;
             i < renderers.Length;
             i++)
        {
            if (renderers[i] == null)
                continue;

            bounds.Encapsulate(
                renderers[i].bounds);
        }

        return bounds;
    }
}