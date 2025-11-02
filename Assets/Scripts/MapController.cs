using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [Header("Pellet Settings")]
    public Transform gridParent;        // Parent holding all pellet objects
    public string normalPelletTag = "Pellet";
    public string powerPelletTag = "PowerPellet";

    private int totalNormalPellets;
    private int remainingNormalPellets;

    private int totalPowerPellets;
    private int remainingPowerPellets;

    [Header("Map Reset Settings")]
    public GameObject mapPrefab; // Assign your original map prefab in inspector
    private Transform currentMapInstance; // Track the current instantiated map


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else 
            Instance = this;
    }


    void Start()
    {
        CountAllPelletsAtStart();
    }

    /// <summary>
    /// Counts all pellets in the scene under gridParent.
    /// Separates normal pellets and power pellets.
    /// </summary>
    private void CountAllPelletsAtStart()
    {
        totalNormalPellets = 0;
        totalPowerPellets = 0;

        int pelletLayer = LayerMask.NameToLayer("Pellet");

        Transform[] allChildren = gridParent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child == gridParent) continue;
            if (!child.gameObject.activeInHierarchy) continue;

            // Check if it's on the Pellet layer
            if (child.gameObject.layer == pelletLayer)
            {
                // If it has the PowerPellet tag, it's a power pellet
                if (child.gameObject.CompareTag(powerPelletTag))
                    totalPowerPellets++;
                else
                    totalNormalPellets++;
            }
        }

        remainingNormalPellets = totalNormalPellets;
        remainingPowerPellets = totalPowerPellets;

        Debug.Log($"Total normal pellets: {totalNormalPellets}");
        Debug.Log($"Total power pellets: {totalPowerPellets}");
    }


    

    /// <summary>
    /// Call this when a normal pellet is eaten
    /// </summary>
    public void OnNormalPelletEaten()
    {
        remainingNormalPellets = Mathf.Max(0, remainingNormalPellets - 1);
        Debug.Log($"Normal pellet eaten! Remaining: {remainingNormalPellets}");
        CheckAllPelletsEaten();
    }

    /// <summary>
    /// Call this when a power pellet is eaten
    /// </summary>
    public void OnPowerPelletEaten()
    {
        remainingPowerPellets = Mathf.Max(0, remainingPowerPellets - 1);
        Debug.Log($"Power pellet eaten! Remaining: {remainingPowerPellets}");
        CheckAllPelletsEaten();
    }

    /// <summary>
    /// Checks if all normal and power pellets are eaten
    /// </summary>
    private void CheckAllPelletsEaten()
    {
        if (remainingNormalPellets <= 0 && remainingPowerPellets <= 0)
        {
            OnAllPelletsEaten();
        }
    }

    private void OnAllPelletsEaten()
    {
        Debug.Log("All pellets eaten! Level complete!");
        // Trigger win condition, animation, etc.
    }

    // Getter methods
    public int GetRemainingNormalPellets() => remainingNormalPellets;
    public int GetRemainingPowerPellets() => remainingPowerPellets;
    public int GetTotalNormalPellets() => totalNormalPellets;
    public int GetTotalPowerPellets() => totalPowerPellets;


    public void restartMap()
    {
        if (mapPrefab == null)
        {
            Debug.LogError("Map prefab is not assigned!");
            return;
        }

        // Destroy the old map
        if (currentMapInstance != null)
            Destroy(currentMapInstance.gameObject);


        // Instantiate a new map
        GameObject newMap = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
        currentMapInstance = newMap.transform;

        // Find the gridParent inside the new map (if your prefab has a child called "GridParent")
        Transform newGridParent = currentMapInstance.Find("GridParent");
        if (newGridParent != null)
            gridParent = newGridParent;
        else
            gridParent = currentMapInstance; // fallback

        // Reset pellets counts
        CountAllPelletsAtStart();

        Debug.Log("Map restarted.");
    }
}
