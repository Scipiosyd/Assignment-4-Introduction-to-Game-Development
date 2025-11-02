using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;
    [SerializeField] private PacStudentController pacStudent;
    [SerializeField] private GhostController[] knights;
    [SerializeField] private CherryController[] cherry;
    [SerializeField] private MapController mapcontrol;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()

    {

    }

    void Update()
    {

    }


    public void ResetEverything() {
        pacStudent.ResetState();
        foreach (var knight in knights)
        {
            knight.knightRestartState();
        }


        foreach (var cherry in cherry)
        {
            cherry.ResetCherry();
        }


        InGameCounterManager.instance.ResetCounters();
        //MapController.Instance.restartMap();

        }
}



