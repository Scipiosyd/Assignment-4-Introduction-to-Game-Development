using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public AudioSource buttonpress;

    [SerializeField]
    public AudioSource startscenemusic;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadFirstLevel()
    {
            startscenemusic.Stop();
            buttonpress.Play();
            SceneManager.LoadScene(1);
        
    }


    // Start is called before the first frame update
    void Start()
    {
        startscenemusic.loop = true;
        startscenemusic.Play();
        
        
    }

  
    // Update is called once per frame
    void Update()
    {
        
    }
}


