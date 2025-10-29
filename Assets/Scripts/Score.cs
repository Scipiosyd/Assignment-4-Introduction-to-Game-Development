using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Score : MonoBehaviour
{
    public Text highscoreText;
    public Text timerText;

    int highscore = 0;
    int milliseconds = 0;
    int seconds = 0;
    int minutes = 0;
    string timerFormat = null;
    // Start is called before the first frame update
    void Start()
    {
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        milliseconds = Mathf.FloorToInt(milliseconds);
        seconds = Mathf.FloorToInt(seconds);
        minutes = Mathf.FloorToInt(minutes);
        timerFormat = string.Format("{0:00}:{0:00}:{0:00)", minutes, seconds, milliseconds);
        timerText.text = "TIMER: " + timerFormat;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
