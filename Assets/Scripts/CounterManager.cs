using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class CounterManager : MonoBehaviour
{
    
 

    //Start Scene
    public Text highscoreText;
    public Text timerText;
    int highscore = 0;
    int milliseconds = 0;
    int seconds = 0;
    int minutes = 0;
    string timerFormat = null;

    
  
    
    



    float elapsedTime;
    

    // Start is called before the first frame update
    void Start()
    {
        highscore = PlayerPrefs.GetInt("HighScore");
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        //milliseconds = Mathf.FloorToInt(milliseconds);
        //seconds = Mathf.FloorToInt(seconds);
        //minutes = Mathf.FloorToInt(minutes);
        timerFormat = PlayerPrefs.GetString("timer");
        timerText.text = "TIMER: " + timerFormat;

       
        

    }

    // Update is called once per frame
    void Update()
    {
      


    }
}
