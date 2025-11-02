using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveScore : MonoBehaviour
{

    public static SaveScore Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Text scoreText;
    // Start is called before the first frame update
    public void saveScore()
    {
        PlayerPrefs.SetInt("HighScore", InGameCounterManager.instance.GetScore);
    }
}
