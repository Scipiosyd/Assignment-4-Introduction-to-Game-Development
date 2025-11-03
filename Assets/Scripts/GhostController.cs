using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{

    public static GhostController instance;

    private Animator animator;

    private enum KnightState { Normal, Scared, Recovering, Dead}
    private KnightState currentState = KnightState.Normal;

    private Vector2 knightstartPos;
    private Vector2 movementDirection = Vector2.down;

    private Coroutine scaredCoroutine;
    private Coroutine deadCoroutine;

    public Transform target;
    public float speed = 1.0f;
    Vector3[] path;
    int targetIndex;


    [SerializeField]
    public AudioSource audioSourcescared;
    [SerializeField]
    public AudioSource audioSourcedead;


    void Start()
    {
        GhostPathfinding.RequestPath(transform.position,target.position, OnPathFound);
    }


    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
            yield return null;
        }
    }
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

    public bool IsScared()
    {
        return currentState == KnightState.Scared;
    }


    public void KnightDead()
    {
        if(deadCoroutine  != null)
        {
            StopCoroutine(deadCoroutine);
        }

        deadCoroutine = StartCoroutine(DeadTime());
    }

    public bool IsDead()
    {
        return currentState == KnightState.Dead;
    }

    public bool IsRecovering()
    {
        return currentState == KnightState.Recovering;
    }




    private IEnumerator ScaredTime()
    {
        audioSourcescared.Play();
        currentState = KnightState.Scared;
        animator.Play("knightscared");
        yield return new WaitForSeconds(7f);

        currentState = KnightState.Recovering;
        animator.Play("knightrecovering");
        yield return new WaitForSeconds(3f);
        audioSourcescared.Stop();
        currentState = KnightState.Normal;
        scaredCoroutine = null;
    }

    private IEnumerator DeadTime()
    {
        audioSourcedead.Play();
        currentState = KnightState.Dead;
        animator.Play("knightdead");
        yield return new WaitForSeconds(3f);

        if(InGameCounterManager.instance.Knightscaredremainingtime <= 0)
        {
            audioSourcedead.Stop();
            currentState = KnightState.Normal;
        }

        else
        {
            currentState = KnightState.Recovering;
            yield return new WaitForSeconds(3f);
            audioSourcedead.Stop();
            currentState = KnightState.Normal;
        }

        deadCoroutine = null;
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
    


    public void knightRestartState()
    {
        StopAllCoroutines();
        transform.position = knightstartPos;
        currentState = KnightState.Normal;
        animator.Play("Knightdown");
    }


}