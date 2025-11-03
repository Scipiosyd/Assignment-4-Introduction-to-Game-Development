using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [Header("Pellet Settings")]
    public Transform gridParent;        
    public string normalPelletTag = "Pellet";
    public string powerPelletTag = "PowerPellet";

    private int totalNormalPellets;
    private int remainingNormalPellets;

    private int totalPowerPellets;
    private int remainingPowerPellets;

    [Header("Map Reset Settings")]
    public GameObject mapPrefab; 
    private Transform currentMapInstance; 


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

            
            if (child.gameObject.layer == pelletLayer)
            {
                
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


    

  
    public void OnNormalPelletEaten()
    {
        remainingNormalPellets = Mathf.Max(0, remainingNormalPellets - 1);
        Debug.Log($"Normal pellet eaten! Remaining: {remainingNormalPellets}");
        CheckAllPelletsEaten();
    }

   
    public void OnPowerPelletEaten()
    {
        remainingPowerPellets = Mathf.Max(0, remainingPowerPellets - 1);
        Debug.Log($"Power pellet eaten! Remaining: {remainingPowerPellets}");
        CheckAllPelletsEaten();
    }

    
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
       
    }

   
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

      
        if (currentMapInstance != null)
            Destroy(currentMapInstance.gameObject);


      
        GameObject newMap = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
        currentMapInstance = newMap.transform;

        
        Transform newGridParent = currentMapInstance.Find("GridParent");
        if (newGridParent != null)
            gridParent = newGridParent;
        else
            gridParent = currentMapInstance; 

       
        CountAllPelletsAtStart();

        Debug.Log("Map restarted.");
    }
}
