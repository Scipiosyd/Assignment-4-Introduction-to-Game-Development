using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class lifecounter : MonoBehaviour
{

    public static lifecounter instance;

    [SerializeField]
    private float lifeimagewidth = 54f;

    [SerializeField]
    private int TotalLives = 3;

    private int currentlives = 3;

    private RectTransform rect;

    public UnityEvent NomoreLives;

    public int Currentlives
    {
        get => currentlives;
        private set
        {
            if ((value < 0))
            {
                NomoreLives?.Invoke();
            }
            currentlives = Mathf.Clamp(value, 0, TotalLives);
            AdjustImageWidth();
        }

    }

    private void Awake()
    {
        rect = transform  as RectTransform;
        AdjustImageWidth ();

        instance = this;
    }

    public void removeLife(int num = 1)
    {
        Currentlives -= num;
    }


    private void AdjustImageWidth()
    {
        rect.sizeDelta = new Vector2(lifeimagewidth * currentlives, rect.sizeDelta.y);
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
