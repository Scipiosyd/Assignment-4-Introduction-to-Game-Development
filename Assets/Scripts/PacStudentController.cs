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

    [SerializeField]
    private ParticleSystem dustEffect;

    private float dusteffectYPos = -0.15f;


    [SerializeField]
    private ParticleSystem wallEffect;

    //Collisions

    [SerializeField]
    private LayerMask collisionLayers;

    [SerializeField]
    private LayerMask teleporterLayer;

    [SerializeField]
    private LayerMask pelletLayer;

    //teleporter

    [SerializeField]
    public GameObject pacStudent;

    [SerializeField]
    public GameObject teleporter1;
    [SerializeField]
    public GameObject teleporter2;


    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        targetPos = transform.position;
    }

    private void Start()
    {
        animator.Play("bunright");
        dustEffect.transform.localPosition = new Vector3(0, dusteffectYPos, 0);

       
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

        Collider2D hit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, collisionLayers);

        Collider2D teleporter = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, teleporterLayer);

        int pelletLayerMask = LayerMask.GetMask("Pellet");

        Collider2D pellethit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, pelletLayerMask);


        if (pellethit != null && !pellethit.CompareTag("PowerPellet")) {

            
                Destroy(pellethit.gameObject);
            }
        
        
        
        if (hit != null)
        {

            int hitLayer = hit.gameObject.layer;

            if (hit.CompareTag("Ghostdoor"))
            {
                audioSourcemoving.Pause();
                return false;

            }

            else if ((collisionLayers.value & (1 << hitLayer)) != 0 && hitLayer == LayerMask.NameToLayer("Wall"))
            {
                Debug.Log("HIT WALL at " + newTarget);
                wallEffect.transform.position = newTarget;
                wallEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                wallEffect.Play();
                audioSourcemoving.Pause();

                // stop it again after a short delay
                StartCoroutine(StopWallEffectAfter(0.5f));
                return false;
            }
        }

        if (dir == Vector2Int.up)
        {
            animator.Play("bunup");
           
        }
        else if (dir == Vector2Int.down)
        {
            animator.Play("bundown");
            
        }
        else if (dir == Vector2Int.left)
        {
            animator.Play("bunleft");
            
        }
        else if (dir == Vector2Int.right)
        {
            animator.Play("bunright");
            
        }


        //Teleporter Logic

        if (teleporter != null)
        {
            int teleporterhitlayer = teleporter.gameObject.layer;

            if ((teleporterLayer.value & (1 << teleporterhitlayer)) != 0 && teleporterhitlayer == LayerMask.NameToLayer("Teleporter"))
            {

                tweener.CancelTween(transform);

                if (dir == Vector2Int.left)
                {
                    float exitPosXRight = teleporter2.transform.position.x - 0.3f;

                    pacStudent.transform.position = new Vector3(exitPosXRight, pacStudent.transform.position.y, 0);
                }

                else if (dir == Vector2Int.right)
                {
                    float exitPosXLeft = teleporter1.transform.position.x + 0.3f;
                    pacStudent.transform.position = new Vector3(exitPosXLeft, pacStudent.transform.position.y, 0);


                }

                isMoving = false;
                targetPos = pacStudent.transform.position;

                
                return true;

            }


        }
            if (dir == Vector2Int.up)
                dustEffect.transform.localRotation = Quaternion.Euler(0, 0, -90); 
            else if (dir == Vector2Int.down)
                dustEffect.transform.localRotation = Quaternion.Euler(0, 0, 90); 
            else if (dir == Vector2Int.left)
                dustEffect.transform.localRotation = Quaternion.Euler(0, 0, 0); 
            else if (dir == Vector2Int.right)
                dustEffect.transform.localRotation = Quaternion.Euler(0, 0, 180);




        Vector3 offset = Vector3.zero;
        float distance = 0.15f;

        if (dir == Vector2Int.up) offset = new Vector3(0, dusteffectYPos - distance, 0);
        else if (dir == Vector2Int.down) offset = new Vector3(0, dusteffectYPos + distance, 0);
        else if (dir == Vector2Int.left) offset = new Vector3(0, dusteffectYPos, 0);
        else if (dir == Vector2Int.right) offset = new Vector3(0, dusteffectYPos, 0);

        dustEffect.transform.localPosition = offset;





        if (tweener.AddTween(transform, transform.position, newTarget, moveDuration))
        {
            audioSourcemoving.Play();
            targetPos = newTarget;
            isMoving = true;

            if (!dustEffect.isPlaying) dustEffect.Play();

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

        if (dustEffect.isPlaying) dustEffect.Pause();
        

       
    }

    private IEnumerator StopWallEffectAfter(float time)
    {
        yield return new WaitForSeconds(time);
        wallEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }



}
  
