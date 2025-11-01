using UnityEngine;

#if UNITY_EDITOR
[ExecuteAlways]
public class WallGizmoDrawer : MonoBehaviour
{
    public LayerMask wallLayer;
    public LayerMask pelletLayer;
    public LayerMask walkableLayer;
    public Vector2 gizmoSize = new Vector2(0.3f, 0.3f); // match your tile size
    [SerializeField]
    public Transform gridParent;  
    public string wallLayerName = "Wall";
    public string pelletLayerName = "Pellet";
    public string walkableLayerName = "Walkable";

    // Prefab names to check
    private string[] wallNames = new string[] { "OuterWall", "InnerWall", "OuterCorner", "InnerCorner" };

    [ContextMenu("Set Wall Layers")]
    public void SetWallLayers()
    {
        if (gridParent == null)
        {
            
            return;
        }

        int wallLayer = LayerMask.NameToLayer(wallLayerName);
        if (wallLayer == -1)
        {
            
            return;
        }

        foreach (Transform child in gridParent)
        {
            foreach (string wallName in wallNames)
            {
                if (child.name.Contains(wallName))
                {
                    child.gameObject.layer = wallLayer;
                    
                    foreach (Transform grandChild in child.GetComponentsInChildren<Transform>(true))
                    {
                        grandChild.gameObject.layer = wallLayer;
                    }
                    break;
                }
            }
        }

       
    }

    [ContextMenu("Add Colliders and Layers to Pellets")]
    public void AddPelletColliders()
    {
        if (gridParent == null)
            return;

        int pelletLayerIndex = LayerMask.NameToLayer(pelletLayerName);
        if (pelletLayerIndex == -1)
        {
            Debug.LogError($"Layer '{pelletLayerName}' does not exist!");
            return;
        }

        int addedCount = 0;
        foreach (Transform child in gridParent)
        {
            if (child.name.Contains("Pellet"))
            {
                child.gameObject.layer = pelletLayerIndex;

                foreach (Transform grandChild in child.GetComponentsInChildren<Transform>(true))
                    grandChild.gameObject.layer = pelletLayerIndex;

                BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
                if (collider == null)
                {
                    collider = child.gameObject.AddComponent<BoxCollider2D>();
                    addedCount++;
                }
                collider.isTrigger = true;
            }
        }


    }


    [ContextMenu("Set Floor Tiles as Walkable")]
    public void SetWalkableTiles()
    {
        if (gridParent == null) return;

        int walkableLayerIndex = LayerMask.NameToLayer(walkableLayerName);
        if (walkableLayerIndex == -1)
        {
            Debug.LogError($"Layer '{walkableLayerName}' does not exist!");
            return;
        }

        foreach (Transform child in gridParent)
        {
            // Assume floor tiles are not walls or pellets
            if (child.name.Contains("Floor"))
            {
                child.gameObject.layer = walkableLayerIndex;
                foreach (Transform grandChild in child.GetComponentsInChildren<Transform>(true))
                    grandChild.gameObject.layer = walkableLayerIndex;
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw walls in red
        Gizmos.color = Color.red;
        Collider2D[] walls = Object.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (var wall in walls)
        {
            if (((1 << wall.gameObject.layer) & wallLayer) != 0)
                Gizmos.DrawWireCube(wall.transform.position, gizmoSize);
        }

        // Draw walkable tiles in semi-transparent green
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        int walkableLayerIndex = LayerMask.NameToLayer(walkableLayerName);
        foreach (Transform child in gridParent)
        {
            if (child.gameObject.layer == walkableLayerIndex)
                Gizmos.DrawCube(child.position, gizmoSize);
        }
    }
}
#endif
