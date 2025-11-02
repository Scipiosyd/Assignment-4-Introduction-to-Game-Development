using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScore : MonoBehaviour
{
    public void LoadHighScore()
    {
        PlayerPrefs.GetInt("HighScore");
    }
}
