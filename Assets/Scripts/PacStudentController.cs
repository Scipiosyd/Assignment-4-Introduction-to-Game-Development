using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDuration = 0.2f;      // Lerp duration for moving one tile
    public Tweener tweener;

    private Vector3 targetPos;
    private bool isMoving = false;
    private Rigidbody2D rb2D;

    private Vector2Int currentInput = Vector2Int.zero;
    private Vector2Int lastInput = Vector2Int.zero;
    [SerializeField]
    public Animator animator;


    [SerializeField]
    public AudioSource audioSourcemoving;
    [SerializeField]
    public AudioSource audioSourceeatingpellet;



    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        targetPos = transform.position;
    }

    private void Start()
    {
        animator.Play("bunright");

       
    }

    private void Update()
    {
        if (isMoving) return;
       
        
        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKey(KeyCode.W))
        {
            
            lastInput = Vector2Int.up;

        }
        else if (Input.GetKey(KeyCode.S))
        {
            
            lastInput = Vector2Int.down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
          
            lastInput = Vector2Int.left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            
            lastInput = Vector2Int.right;
        }
        if (!isMoving)
        {
            if (TryMove(lastInput))
            {
                currentInput = lastInput;
            }

            else if (TryMove(currentInput))
            {

            }
        }
    }

    private bool TryMove(Vector2Int dir)
    {


        if (dir == Vector2Int.zero) return false;

        Vector3 newTarget = targetPos + new Vector3(dir.x * 0.3f, dir.y * 0.3f, 0);

        Collider2D hit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f);
    
        if (hit != null)
        {
            if (hit.CompareTag("Ghostdoor"))
            {
                audioSourcemoving.Pause();
                return false;

            }

            else if (hit.CompareTag("Wall"))
            {
                audioSourcemoving.Pause();
                return false;
            }
        }

        if (dir == Vector2Int.up) animator.Play("bunup");
        else if (dir == Vector2Int.down) animator.Play("bundown");
        else if (dir == Vector2Int.left) animator.Play("bunleft");
        else if (dir == Vector2Int.right) animator.Play("bunright");

        if (tweener.AddTween(transform, transform.position, newTarget, moveDuration))
        {
            audioSourcemoving.Play();
            targetPos = newTarget;
            isMoving = true;
            StartCoroutine(WaitForTween(moveDuration));
            return true;
        }

        return false;
    }

    private IEnumerator WaitForTween(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSourcemoving.Pause();
        isMoving = false;

       
    }

}
  
