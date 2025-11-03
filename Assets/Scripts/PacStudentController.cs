using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDuration = 0.2f;      
    public Tweener tweener;

    private Vector3 targetPos;
    private Vector3 startingPos;
    
    private bool isMoving = false;

    private Vector2Int startInput;
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


    [SerializeField]
    private ParticleSystem bloodEffect;

    //Collisions

    [SerializeField]
    private LayerMask collisionLayers;

    [SerializeField]
    private LayerMask ghostdoorLayer;

    [SerializeField]
    private LayerMask teleporterLayer;

    [SerializeField]
    private LayerMask pelletLayer;

   

    [SerializeField]
    private LayerMask cherryLayer;

    [SerializeField]
    private CherryController cherryController;

    //teleporter

    [SerializeField]
    public GameObject pacStudent;

    [SerializeField]
    public GameObject teleporter1;
    [SerializeField]
    public GameObject teleporter2;


    //Ghosts (knights)
    

    [SerializeField]
    private GhostController[] knights;

    [SerializeField]
    private LayerMask knightLayer;

    [SerializeField]
    private LevelController levelController;

    private bool isDead;

    //Counter
    int pelletcounter = 217;
    int knightlivescounter = 3;


    private Coroutine moveCoroutine;
    private Coroutine wallEffectCoroutine;
    private Coroutine endLevelCoroutine;

    private void Awake()
    {
        
        targetPos = transform.position;
        startingPos = transform.position;
        startInput = Vector2Int.zero;
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
        Collider2D ghostdoorhit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, ghostdoorLayer);

        Collider2D teleporter = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, teleporterLayer);

        int pelletLayerMask = LayerMask.GetMask("Pellet");

        Collider2D pellethit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, pelletLayerMask);

        int cherryLayerMask = LayerMask.GetMask("Cherry");
        Collider2D cherryhit = Physics2D.OverlapBox(newTarget, new Vector2(1f,1f), 0f, cherryLayerMask);


        int knightLayerMask = LayerMask.GetMask("Ghosts");
        Collider2D knighthit = Physics2D.OverlapBox(newTarget, new Vector2(0.1f, 0.1f), 0f, knightLayerMask);



        if (knighthit != null)
        {
            GhostController hitKnight = knighthit.GetComponent<GhostController>();
            if (hitKnight == null) return false; 

            if (hitKnight.IsScared())
            {
               
                InGameCounterManager.instance.AddPoint(300);
                hitKnight.KnightDead(); 
            }
            else if(!hitKnight.IsScared() && !hitKnight.IsRecovering() && !hitKnight.IsDead())
            {
                
                isMoving = true;
                tweener.CancelTween(transform);

                foreach (var k in knights)
                {
                    Animator knightAnimator = k.GetComponent<Animator>();
                    if (knightAnimator != null)
                        knightAnimator.speed = 0;
                }

                

                StartCoroutine(DeathScene(3f));
                
                    return false;
            }

            }



            if (pellethit != null && !pellethit.CompareTag("PowerPellet")) {

            audioSourceeatingpellet.Play();

            if (pelletcounter > 1)
            {

                InGameCounterManager.instance.AddPoint(10);
                pelletcounter -= 1;
                Destroy(pellethit.gameObject);
            }

            else
            {
                InGameCounterManager.instance.AddPoint(10);
                pelletcounter -= 1;
                Destroy(pellethit.gameObject);
                StartCoroutine(EndLevel(3f));


            }


            }

        if(pellethit != null && pellethit.CompareTag("PowerPellet"))
        {
            audioSourceeatingpellet.Play();
            InGameCounterManager.instance.AddPoint(50);
            Destroy(pellethit.gameObject);

            foreach (var knight in knights)
            {

                
                knight.KnightScared();
                
            }

           InGameCounterManager.instance.GhostTimer(10f);

            
        }
        

        if(cherryhit != null)
        {
           
            InGameCounterManager.instance.AddPoint(100);

            cherryController.OnCherryDestroyed();

            Destroy(cherryhit.gameObject);
        }
        
        
        if (hit != null)
        {

            int hitLayer = hit.gameObject.layer;

           

             if ((collisionLayers.value & (1 << hitLayer)) != 0 && hitLayer == LayerMask.NameToLayer("Wall"))
            {
                
                wallEffect.transform.position = newTarget;
                wallEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                wallEffect.Play();
                audioSourcemoving.Pause();

                // stop it again after a short delay
                StartCoroutine(StopWallEffectAfter(0.5f));
                return false;
            }
        }


        if (ghostdoorhit != null)
        {
            int ghosthitLayer = ghostdoorhit.gameObject.layer;

            if ((ghostdoorLayer.value & (1 << ghosthitLayer)) != 0 && ghosthitLayer == LayerMask.NameToLayer("GhostWall"))
            {
                audioSourcemoving.Pause();
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

    private IEnumerator DeathScene(float duration)
{

            if (isDead) yield break;
            isDead = true;

            bloodEffect.Play();

            animator.Play("bundead");

            isMoving = true;

            tweener.CancelTween(transform);
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);

            yield return new WaitForSeconds(duration);

            isMoving = false;

            foreach (var knight in knights)
            {
                Animator knightAnimator = knight.GetComponent<Animator>();
                if (knightAnimator != null)
                    knightAnimator.speed = 1;
            }

            bloodEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            lifecounter.instance.removeLife();
            knightlivescounter--;


        if (knightlivescounter > 0)
        {
            LevelController.Instance.ResetEverything();

            isDead = false;
        }
        else
        {
            StartCoroutine(EndLevel(2));
        }
}

    private IEnumerator EndLevel(float duration)
    {
        yield return new WaitForSeconds(duration);

        PrePostRound.instance.GameOver();


    }



    public void ResetState()
    {

        isMoving = false;
        tweener.CancelTween(transform);
        transform.position = startingPos;
        targetPos = startingPos;

        currentInput = startInput;
        lastInput = startInput;

        animator.Play("bunright");

        dustEffect.Stop();

        audioSourcemoving.Pause();


        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (wallEffectCoroutine != null)
        {
            StopCoroutine(wallEffectCoroutine);
            wallEffectCoroutine = null;
        }
    }
 }
  
