using UnityEngine;

public class TilemapGridCamera : MonoBehaviour
{
    public Camera cam;
    public GameObject tilemapGrid;      // parent of all tilemaps

    public float referenceHeight = 1080f;
    public float baseOrthoSize = 5f;

    void LateUpdate()
    {
        // --- 1) SCALE LIKE UI PANEL ---
        float scale = Screen.height / referenceHeight;
        cam.orthographicSize = baseOrthoSize / scale;

        // --- 2) CENTER ON GRID ---
        Bounds bounds = GetBounds(tilemapGrid);
        Vector3 center = bounds.center;
        center.z = cam.transform.position.z;
        cam.transform.position = center;
    }

    Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.zero);

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers)
            b.Encapsulate(r.bounds);
        return b;
    }
}
