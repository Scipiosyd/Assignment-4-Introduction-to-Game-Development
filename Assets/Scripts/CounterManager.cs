using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Score : MonoBehaviour
{
    
 

    //Start Scene
    public Text highscoreText;
    public Text timerText;
    int highscore = 0;
    int milliseconds = 0;
    int seconds = 0;
    int minutes = 0;
    string timerFormat = null;

    //Level 1
    public Text scoreText;
    public Text timeText;
    int score = 0;
    string scoreFormat = null;

  
    
    



    float elapsedTime;
    

    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        //milliseconds = Mathf.FloorToInt(milliseconds);
        //seconds = Mathf.FloorToInt(seconds);
        //minutes = Mathf.FloorToInt(minutes);
        
        timerText.text = "TIMER: " + timerFormat;

       
        

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime % 1f) * 1000f);
        timerFormat = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        timeText.text = "TIME \n" + timerFormat;

        scoreFormat = string.Format("{000000}", score); //fix
        scoreText.text = "SCORE \n" + scoreFormat;



    }
}
