using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lifecounter : MonoBehaviour
{

    [SerializeField]
    private float lifeimagewidth = 54f;

    [SerializeField]
    private int TotalLives = 3;

    private int currentlives = 3;

    private RectTransform rect;

    public int Currentlives
    {
        get => currentlives;
        private set
        {
            currentlives = Mathf.Clamp(value, 0, TotalLives);
            rect.sizeDelta = new Vector2(lifeimagewidth * currentlives, rect.sizeDelta.y);
        }

    }

    private void Awake()
    {
        rect = transform  as RectTransform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
