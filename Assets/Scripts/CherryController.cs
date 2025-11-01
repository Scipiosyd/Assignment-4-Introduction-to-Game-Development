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
    public Vector2 mapOrigin = new Vector2(-4f, -4.15f);
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
            return;
        }

        CalculateBounds();
       
        StartCoroutine(SpawnCherryRoutine());
    }

    private void CalculateBounds()
    {
       
        
        if (useGridCoordinates && grid != null)
        {
            
            Vector3 gridOriginWorld = grid.transform.position;
            Vector3 cellSize = grid.cellSize;

           
            minBounds = new Vector2(gridOriginWorld.x, gridOriginWorld.y);
            maxBounds = new Vector2(
                gridOriginWorld.x + (mapWidth * cellSize.x),
                gridOriginWorld.y + (mapHeight * cellSize.y)
            );

        }
        else
        {
            
            minBounds = mapOrigin;
            maxBounds = new Vector2(
                mapOrigin.x + mapWidth * tileSize,
                mapOrigin.y + mapHeight * tileSize
            );

        
        }

        centerPos = new Vector3(0, 0, 0);
        
    }

    private IEnumerator SpawnCherryRoutine()
    {
        
        yield return new WaitForSeconds(spawnDelay);

        while (true)
        {
            if (currentCherry == null)
            {
                SpawnCherry();

            }

            while (!cherryDestroyed)
            {
                yield return null;
            }

        
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void SpawnCherry()
    {

        int direction = Random.Range(0, 4);

        Vector3 startPos = Vector3.zero;    
        Vector3 endPos = Vector3.zero;

       switch(direction)
        {
            case 0: // from left to right
                startPos = new Vector3(minBounds.x - spawnOffset, Random.Range(minBounds.y, maxBounds.y), 0f);
                endPos = centerPos + (centerPos - startPos).normalized * (maxBounds.x - minBounds.x + 2 * spawnOffset);
                break;

            case 1: // from right to left
                startPos = new Vector3(maxBounds.x + spawnOffset, Random.Range(minBounds.y, maxBounds.y), 0f);
                endPos = centerPos + (centerPos - startPos).normalized * (maxBounds.x - minBounds.x + 2 * spawnOffset);
                break;

            case 2: // from top to bottom
                startPos = new Vector3(Random.Range(minBounds.x, maxBounds.x), maxBounds.y + spawnOffset, 0f);
                endPos = centerPos + (centerPos - startPos).normalized * (maxBounds.y - minBounds.y + 2 * spawnOffset);
                break;

            case 3: // from bottom to top
                startPos = new Vector3(Random.Range(minBounds.x, maxBounds.x), minBounds.y - spawnOffset, 0f);
                endPos = centerPos + (centerPos - startPos).normalized * (maxBounds.y - minBounds.y + 2 * spawnOffset);
                break;

        }

        currentCherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
        

      

        
        SpriteRenderer sr = currentCherry.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = cherrySortingOrder;
           
        }
        else
        {
            
        }

        cherryDestroyed = false;
        StartCoroutine(MoveCherry(currentCherry, startPos, endPos, moveDuration));
    }

    private IEnumerator MoveCherry(GameObject cherry, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
      

        int frameCount = 0;
        while (elapsed < duration && !cherryDestroyed)
        {
            if (cherry == null)
            {
                
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            cherry.transform.position = Vector3.Lerp(start, end, t);

            frameCount++;
           
            if (frameCount <= 10 || Mathf.FloorToInt(elapsed) != Mathf.FloorToInt(elapsed - Time.deltaTime))
            {
               
            }

            yield return null;
        }

      
        if (cherry != null && !cherryDestroyed)
        {
          
            Destroy(cherry);
            cherryDestroyed = true;
        }
    }

    public void OnCherryDestroyed()
    {
    
        cherryDestroyed = true;
        if (currentCherry != null)
        {
            Destroy(currentCherry);
            currentCherry = null;
        }
    }
}