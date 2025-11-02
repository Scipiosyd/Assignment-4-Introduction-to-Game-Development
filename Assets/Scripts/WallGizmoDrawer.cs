using UnityEngine;

#if UNITY_EDITOR
//[ExecuteAlways]
public class WallGizmoDrawer : MonoBehaviour
{
    [Header("Layers and Prefabs")]
    public LayerMask wallLayer;
    public LayerMask pelletLayer;
    public LayerMask walkableLayer;
    public Transform gridParent;
    public GameObject pelletPrefab;

    public string wallLayerName = "Wall";
    public string pelletLayerName = "Pellet";
    public string walkableLayerName = "Walkable";

    [Header("Gizmo Settings")]
    public Vector2 gizmoSize = new Vector2(0.3f, 0.3f);

    [Header("Pellet Spawning")]
    public float checkRadius = 0.1f;

    // Cache transforms to avoid repeated allocations
    private Transform[] cachedChildren;

    // Prefab names for walls
    private readonly string[] wallNames = { "OuterWall", "InnerWall", "OuterCorner", "InnerCorner" };

    // ------------------- Editor Methods -------------------
    [ContextMenu("Cache Grid Children")]
    public void CacheChildren()
    {
        if (gridParent != null)
        {
            cachedChildren = gridParent.GetComponentsInChildren<Transform>(true);
        }
        else
        {
            cachedChildren = new Transform[0];
        }
    }

    [ContextMenu("Set Wall Layers")]
    public void SetWallLayers()
    {
        if (cachedChildren == null) CacheChildren();

        int wallLayerIndex = LayerMask.NameToLayer(wallLayerName);
        if (wallLayerIndex == -1) return;

        foreach (var child in cachedChildren)
        {
            foreach (var name in wallNames)
            {
                if (child.name.Contains(name))
                {
                    child.gameObject.layer = wallLayerIndex;
                    foreach (var grandChild in child.GetComponentsInChildren<Transform>(true))
                        grandChild.gameObject.layer = wallLayerIndex;
                    break;
                }
            }
        }
    }

    [ContextMenu("Set Floor Tiles as Walkable")]
    public void SetWalkableTiles()
    {
        if (cachedChildren == null) CacheChildren();

        int walkableLayerIndex = LayerMask.NameToLayer(walkableLayerName);
        if (walkableLayerIndex == -1) return;

        foreach (var child in cachedChildren)
        {
            if (child.name.Contains("Floor"))
            {
                child.gameObject.layer = walkableLayerIndex;
            }
        }
    }

    //[ContextMenu("Add Colliders and Layers to Pellets")]
    //public void AddPelletColliders()
    //{
    //    if (cachedChildren == null) CacheChildren();

    //    int pelletLayerIndex = LayerMask.NameToLayer(pelletLayerName);
    //    if (pelletLayerIndex == -1)
    //    {
    //        Debug.LogError($"Layer '{pelletLayerName}' does not exist!");
    //        return;
    //    }

    //    foreach (var child in cachedChildren)
    //    {
    //        if (child.name.Contains("Pellet"))
    //        {
    //            child.gameObject.layer = pelletLayerIndex;

    //            BoxCollider2D bc = child.GetComponent<BoxCollider2D>();
    //            if (bc == null)
    //            {
    //                bc = child.gameObject.AddComponent<BoxCollider2D>();
    //            }
    //            bc.isTrigger = true;
    //        }
    //    }
    //}

    [ContextMenu("Spawn Pellets On Walkable Tiles")]
    public void SpawnPelletsOnWalkableTiles()
    {
        if (cachedChildren == null) CacheChildren();
        if (pelletPrefab == null) return;

        int walkableLayerIndex = LayerMask.NameToLayer(walkableLayerName);
        int wallLayerIndex = LayerMask.NameToLayer(wallLayerName);
        int pelletLayerIndex = LayerMask.NameToLayer(pelletLayerName);

        int spawnCount = 0;

        foreach (var tile in cachedChildren)
        {
            if (tile.gameObject.layer != walkableLayerIndex) continue;

            Collider2D hit = Physics2D.OverlapCircle(tile.position, checkRadius, wallLayer | pelletLayer);
            if (hit != null) continue;

            GameObject pellet = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(pelletPrefab, gridParent);
            pellet.transform.position = tile.position;
            pellet.name = "Pellet_" + spawnCount;
            pellet.layer = pelletLayerIndex;
            spawnCount++;
        }

        Debug.Log($"Spawned {spawnCount} pellets on walkable tiles.");
    }



    [ContextMenu("Tag Tiles With Pellets")]
    public void TagTilesWithPellets()
    {
        if (cachedChildren == null) CacheChildren();

        int pelletLayerIndex = LayerMask.NameToLayer(pelletLayerName);
        if (pelletLayerIndex == -1)
        {
            Debug.LogError($"Layer '{pelletLayerName}' does not exist!");
            return;
        }

        foreach (var tile in cachedChildren)
        {
            // Only check tiles (assuming walkable tiles are the ones that can have pellets)
            if (tile.gameObject.layer != LayerMask.NameToLayer(walkableLayerName))
                continue;

            // Check if there's a normal pellet on top of this tile
            Collider2D pelletOnTile = Physics2D.OverlapCircle(tile.position, checkRadius, pelletLayer);
            if (pelletOnTile != null && !pelletOnTile.CompareTag("PowerPellet"))
            {
                tile.gameObject.tag = "PelletTile";
            }

        }

        Debug.Log("Tiles with normal pellets have been tagged as 'PelletTile'.");
    }


    // ------------------- Gizmos -------------------
    private void OnDrawGizmos()
    {
        if (cachedChildren == null) CacheChildren();

        // Draw walls in red
        Gizmos.color = Color.red;
        int wallLayerIndex = LayerMask.NameToLayer(wallLayerName);
        foreach (var child in cachedChildren)
        {
            if (child.gameObject.layer == wallLayerIndex)
                Gizmos.DrawWireCube(child.position, gizmoSize);
        }

        // Draw walkable tiles in green
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        int walkableLayerIndex = LayerMask.NameToLayer(walkableLayerName);
        foreach (var child in cachedChildren)
        {
            if (child.gameObject.layer == walkableLayerIndex)
                Gizmos.DrawCube(child.position, gizmoSize);
        }
    }
}
#endif
