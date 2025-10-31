using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDuration = 0.2f;
    public float tileSize = 0.3f;

    [Header("Collision")]
    public LayerMask obstacleLayer; // walls/inner walls prefabs should be in this layer

    private Tweener tweener;

    private void Awake()
    {
        tweener = GetComponent<Tweener>();
        if (tweener == null)
        {
            tweener = gameObject.AddComponent<Tweener>();
        }
    }

    private void Update()
    {
        // Only accept new input if not currently tweening
        if (tweener.TweenExists(transform)) return;

        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) dir = Vector2Int.right;

        if (dir != Vector2Int.zero)
        {
            TryMove(dir);
        }
    }

    private void TryMove(Vector2Int dir)
    {
        Vector3 targetPos = transform.position + new Vector3(dir.x * tileSize, dir.y * tileSize, 0);

        // Check for wall collision
        if (Physics2D.OverlapBox(targetPos, Vector2.one * tileSize * 0.8f, 0f, obstacleLayer) != null)
        {
            return; // Blocked
        }

        MoveTo(targetPos);
    }

    private void MoveTo(Vector3 targetPos)
    {
        // Use existing Tweener
        tweener.AddTween(transform, transform.position, targetPos, moveDuration);
    }
}
