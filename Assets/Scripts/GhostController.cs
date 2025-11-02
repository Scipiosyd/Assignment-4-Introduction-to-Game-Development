using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{

    public static GhostController instance;

    private Animator animator;

    private enum KnightState { Normal, Scared, Recovering }
    private KnightState currentState = KnightState.Normal;

    private Vector2 knightstartPos;
    private Vector2 movementDirection = Vector2.down;

    private Coroutine scaredCoroutine;

    

    public void Awake()
    {
        animator = GetComponent<Animator>();
        knightstartPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        if (currentState == KnightState.Normal)
        {
            PlayNormalAnimation();
        }

        

    }




    public void KnightScared()
    {
        if(scaredCoroutine != null)
        {
            StopCoroutine(scaredCoroutine);
        }

        
        scaredCoroutine = StartCoroutine(ScaredTime());
    }

    public bool isScared()
    {
        return currentState == KnightState.Scared;
    }



    private IEnumerator ScaredTime()
    {
        currentState = KnightState.Scared;
        animator.Play("knightscared");
        yield return new WaitForSeconds(7f);

        currentState = KnightState.Recovering;
        animator.Play("knightrecovering");
        yield return new WaitForSeconds(3f);

        currentState = KnightState.Normal;
        scaredCoroutine = null;
    }


    private void PlayNormalAnimation()
    {
        // Example: choose animation based on movement
        if (movementDirection == Vector2.up)
            animator.Play("knightup");
        else if (movementDirection == Vector2.down)
            animator.Play("Knightdown");
        else if (movementDirection == Vector2.left)
            animator.Play("knightleft");
        else if (movementDirection == Vector2.right)
            animator.Play("knightright");
    }


    // Start is called before the first frame update
    void Start()
    {

    }


    public void knightRestartState()
    {
        StopAllCoroutines();
        transform.position = knightstartPos;
        currentState = KnightState.Normal;
        animator.Play("Knightdown");
    }


}