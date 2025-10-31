using System.Collections;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    [Header("Grid Map Settings")]
    [SerializeField] private Grid grid; // Reference to your Grid component
    [SerializeField] private bool useGridCoordinates = true; // Toggle to use grid or manual values
    [SerializeField] private bool useSimpleBounds = false; // Use direct min/max values instead

    // Simple bounds (easiest method)
    [Header("Simple Bounds (if useSimpleBounds = true)")]
    [SerializeField] private Vector2 levelMinBounds = new Vector2(-10, -10);
    [SerializeField] private Vector2 levelMaxBounds = new Vector2(10, 10);

    // Manual grid settings (used if grid is null or useGridCoordinates is false)
    [Header("Manual Grid Settings")]
    public Vector2 mapOrigin = new Vector2(-874.9996f, -498.7015f);
    public int mapWidth = 28;
    public int mapHeight = 29;
    public float tileSize = 0.3f;

    [Header("Cherry Settings")]
    [SerializeField] private GameObject cherryPrefab;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float moveDuration = 5f;
    [SerializeField] private int cherrySortingOrder = 100;
    [SerializeField] private float spawnOffset = 2f; // Distance outside bounds to spawn

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector3 centerPos;
    private GameObject currentCherry;
    private bool cherryDestroyed = false;

    void Start()
    {
        if (cherryPrefab == null)
        {
            Debug.LogError("CherryController: Cherry Prefab is not assigned!");
            return;
        }

        CalculateBounds();
        Debug.Log($"Cherry Controller Started. Bounds: {minBounds} to {maxBounds}, Center: {centerPos}");
        StartCoroutine(SpawnCherryRoutine());
    }

    private void CalculateBounds()
    {
        // Simplest method - just use direct min/max bounds
        if (useSimpleBounds)
        {
            minBounds = levelMinBounds;
            maxBounds = levelMaxBounds;
            Debug.Log("Using simple bounds");
        }
        // If using a Grid component, calculate bounds from it
        else if (useGridCoordinates && grid != null)
        {
            // Get grid's world position and cell size
            Vector3 gridOriginWorld = grid.transform.position;
            Vector3 cellSize = grid.cellSize;

            // Calculate world space bounds
            minBounds = new Vector2(gridOriginWorld.x, gridOriginWorld.y);
            maxBounds = new Vector2(
                gridOriginWorld.x + (mapWidth * cellSize.x),
                gridOriginWorld.y + (mapHeight * cellSize.y)
            );

            Debug.Log($"Using Grid coordinates. Cell size: {cellSize}, Grid position: {gridOriginWorld}");
        }
        else
        {
            // Use manual values
            minBounds = mapOrigin;
            maxBounds = new Vector2(
                mapOrigin.x + mapWidth * tileSize,
                mapOrigin.y + mapHeight * tileSize
            );

            Debug.Log("Using manual coordinates");
        }

        centerPos = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            0f
        );
    }

    private IEnumerator SpawnCherryRoutine()
    {
        Debug.Log($"Waiting {spawnDelay} seconds before first cherry spawn...");
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            SpawnCherry();

            // Wait until cherry is destroyed
            while (!cherryDestroyed)
            {
                yield return null;
            }

            Debug.Log($"Cherry destroyed. Waiting {spawnDelay} seconds before next spawn...");
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnCherry()
    {
        // Simple horizontal movement through center for easier debugging
        bool fromLeft = Random.value > 0.5f;

        Vector3 startPos = fromLeft
            ? new Vector3(minBounds.x - spawnOffset, centerPos.y, 0f)
            : new Vector3(maxBounds.x + spawnOffset, centerPos.y, 0f);

        Vector3 endPos = fromLeft
            ? new Vector3(maxBounds.x + spawnOffset, centerPos.y, 0f)
            : new Vector3(minBounds.x - spawnOffset, centerPos.y, 0f);

        Debug.Log($"Spawning cherry at {startPos}, moving to {endPos} over {moveDuration} seconds");

        currentCherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
        currentCherry.name = "Cherry_Active";

        Debug.Log($"Cherry instantiated: {currentCherry != null}, Position: {currentCherry.transform.position}");
        Debug.Log($"Cherry active in hierarchy: {currentCherry.activeInHierarchy}");

        // Set sorting order to render over other sprites
        SpriteRenderer sr = currentCherry.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = cherrySortingOrder;
            Debug.Log($"Cherry sprite renderer found. Sorting order set to {cherrySortingOrder}");
        }
        else
        {
            Debug.LogWarning("Cherry prefab missing SpriteRenderer!");
        }

        cherryDestroyed = false;
        StartCoroutine(MoveCherry(currentCherry, startPos, endPos, moveDuration));
    }

    private IEnumerator MoveCherry(GameObject cherry, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        Debug.Log($"Cherry movement coroutine started. Time.timeScale = {Time.timeScale}");
        Debug.Log($"Start: {start}, End: {end}, Duration: {duration}");

        int frameCount = 0;
        while (elapsed < duration && !cherryDestroyed)
        {
            if (cherry == null)
            {
                Debug.Log("Cherry was destroyed externally");
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cherry.transform.position = Vector3.Lerp(start, end, t);

            frameCount++;
            // Debug first 10 frames, then every second
            if (frameCount <= 10 || Mathf.FloorToInt(elapsed) != Mathf.FloorToInt(elapsed - Time.deltaTime))
            {
                Debug.Log($"Frame {frameCount}: Cherry position: {cherry.transform.position}, Time.deltaTime: {Time.deltaTime}, Progress: {(t * 100):F1}%");
            }

            yield return null;
        }

        // Destroy cherry if it reached the end and wasn't collected
        if (cherry != null && !cherryDestroyed)
        {
            Debug.Log("Cherry reached end position, destroying");
            Destroy(cherry);
            cherryDestroyed = true;
        }
    }

    public void OnCherryDestroyed()
    {
        Debug.Log("Cherry destroyed!");
        cherryDestroyed = true;
        if (currentCherry != null)
        {
            Destroy(currentCherry);
            currentCherry = null;
        }
    }
}