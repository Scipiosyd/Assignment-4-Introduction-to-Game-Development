using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrePostRound : MonoBehaviour
{
    
    public Text countdownText;
    public GameObject screenOverlay; 

   
    public PacStudentController player;
    public GhostController[] knights;

    public InGameCounterManager counter;


    public float countdownInterval = 1f; // 1 second per number

    private void Start()
    {
        
        SetGameplayActive(false);

        
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        screenOverlay.SetActive(true);

        string[] countdownValues = { "3", "2", "1", "GO!" };

        foreach (string val in countdownValues)
        {
            countdownText.text = val;
            countdownText.gameObject.SetActive(true);
            yield return new WaitForSeconds(countdownInterval);
        }

        
        countdownText.gameObject.SetActive(false);
        screenOverlay.SetActive(false);

        
        SetGameplayActive(true);
    }

    private void SetGameplayActive(bool active)
    {
        
        player.enabled = active;

        if(active )
        {
            counter.ResumeTimer();
        }
        else
        {
            counter.PauseTimer();
        }


        foreach (var knight in knights)
        {
            knight.enabled = active;
         }
    }
}