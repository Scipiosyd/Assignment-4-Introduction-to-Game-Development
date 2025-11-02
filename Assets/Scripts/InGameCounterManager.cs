using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameCounterManager : MonoBehaviour
{

    public static InGameCounterManager instance;
   
    int milliseconds = 0;
    int seconds = 0;
    int minutes = 0;
    string timerFormat = null;

    //Level 1
    public Text scoreText;
    public Text timeText;
    public Text ScaredText;
    public Text ScaredtimerText;
    int score = 0;
    string scoreFormat = null;


    public int GetScore {  get { return score; } }

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
       instance = this;
    }



    float elapsedTime;

    private float knightscaredremaining = 0f;
    private bool knightisscared = false;

    private float knightdeadremaining = 0f;
    private bool knightdead = false;


    public float Knightscaredremainingtime
    {
        get {  return knightscaredremaining; } 

    }

    private void Start()
    {
        
        scoreText.text = "SCORE \n" + score.ToString();
        ScaredText.text = "SCARED TIME";

    }


    // Update is called once per frame
    void Update()
    {
        if (IsPaused) return;
        elapsedTime += Time.deltaTime;
         minutes = Mathf.FloorToInt(elapsedTime / 60);
         seconds = Mathf.FloorToInt(elapsedTime % 60);
         milliseconds = Mathf.FloorToInt((elapsedTime % 1f) * 1000f);
        timerFormat = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        timeText.text = "TIME \n" + timerFormat;

        if (knightisscared)
        {

            knightscaredremaining -= Time.deltaTime;

            if (knightscaredremaining <= 0f)
            {
                knightscaredremaining = 0f;
                knightisscared = false;
                ScaredtimerText.gameObject.SetActive(false);
            }




            else
            {
                ScaredtimerText.gameObject.SetActive(true);
                ScaredtimerText.text = Mathf.FloorToInt(knightscaredremaining % 60).ToString();
            }
        }
    }

    public void PauseTimer() => IsPaused = true;
    public void ResumeTimer() => IsPaused = false;


    public void GhostTimer(float duration)
    {
        knightscaredremaining = duration;
        knightisscared = true;
        ScaredtimerText.gameObject.SetActive(true);
    }


    public void AddPoint(int points)
    {
        
        //scoreFormat = string.Format("{000000}", points); //fix

        score += points;
        
        scoreText.text = "SCORE \n" + score.ToString("000000");
    }


    public void ResetCounters()
    {
        elapsedTime = 0f;
        timeText.text = "TIME \n00:00:00";

        knightscaredremaining = 0f;
        knightisscared = false;
        ScaredtimerText.gameObject.SetActive(false);
    }




}


