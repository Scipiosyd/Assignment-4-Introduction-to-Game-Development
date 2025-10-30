using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class InputManager : MonoBehaviour
{
    [SerializeField]
    private GameObject item;

    private Tweener tweener;

    private List<GameObject> itemList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        tweener = GetComponent<Tweener>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddMoveTween(new Vector3(-2.0f, 0.5f, 0.0f), 1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            AddMoveTween(new Vector3(2.0f, 0.5f, 0.0f), 1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            AddMoveTween(new Vector3(0.0f, 0.5f, -2.0f), 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            AddMoveTween(new Vector3(0.0f, 0.5f, 2.0f), 0.5f);
        }
    }

    private void AddMoveTween(Vector3 endPos, float duration)
    {
        Transform targetTransform = item.transform;
        Vector3 startPos = targetTransform.position;

        // Create a new Tween
        Tween newTween = new Tween(targetTransform, startPos, endPos, Time.time, duration);

        // Add it to the Tweener
        tweener.AddTween(item.transform, startPos, endPos, duration);
    }
}
