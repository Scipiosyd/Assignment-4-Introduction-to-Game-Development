using UnityEngine;

public class LevelGeneratorInScene : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject Empty;
    public GameObject OuterCorner;
    public GameObject InnerCorner;
    public GameObject Tjunction;
    public GameObject Outerwall;
    public GameObject Innerwall;
    public GameObject GhostExit;
    public GameObject Floor;
    public GameObject Pellet;
    public GameObject PowerPellet;

    [Header("Grid Settings")]
    public float tileSize = 1f; // spacing between tiles

    // Base level map (1st quadrant)
    public int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };

    /// <summary>
    /// Call this to generate the full 4-quadrant map in the scene
    /// </summary>
    public void GenerateLevelInScene()
    {
        // Clear existing children
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // 1️⃣ First Quadrant
        GenerateQuadrant(levelMap, Vector2.zero, false, false);

        // 2️⃣ Second Quadrant (horizontal flip)
        GenerateQuadrant(levelMap, new Vector2(levelMap.GetLength(1) * tileSize, 0), true, false);

        // 3️⃣ Third Quadrant (horizontal + vertical flip)
        GenerateQuadrant(levelMap, new Vector2(levelMap.GetLength(1) * tileSize, -levelMap.GetLength(0) * tileSize), true, true);

        // 4️⃣ Fourth Quadrant (vertical flip)
        GenerateQuadrant(levelMap, new Vector2(0, -levelMap.GetLength(0) * tileSize), false, true);
    }

    /// <summary>
    /// Generates a single quadrant with optional flips
    /// </summary>
    void GenerateQuadrant(int[,] map, Vector2 offset, bool flipX, bool flipY)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Determine which tile in the map to use based on flips
                int r = flipY ? rows - 1 - row : row;
                int c = flipX ? cols - 1 - col : col;

                int tile = map[r, c];

                // Calculate world position
                Vector3 worldPos = new Vector3(col * tileSize + offset.x, -row * tileSize + offset.y, 0);

                // Pass row/col/map to properly check neighbors
                InstantiateTile(r, c, map, tile, worldPos);
            }
        }
    }

    /// <summary>
    /// Instantiate prefabs based on tile number
    /// </summary>
    void InstantiateTile(int row, int col, int[,] map, int tile, Vector3 worldPos)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);

        // Neighbor checks
        bool up = row > 0 && (map[row - 1, col] == 5 || map[row - 1, col] == 6);
        bool down = row < rows - 1 && (map[row + 1, col] == 5 || map[row + 1, col] == 6);
        bool left = col > 0 && (map[row, col - 1] == 5 || map[row, col - 1] == 6);
        bool right = col < cols - 1 && (map[row, col + 1] == 5 || map[row, col + 1] == 6);

        bool wallup = row > 0 && map[row - 1, col] == 2;
        bool walldown = row < rows - 1 && map[row + 1, col] == 2;
        bool wallleft = col > 0 && map[row, col - 1] == 2;
        bool wallright = col < cols - 1 && map[row, col + 1] == 2;

        bool iwallup = row > 0 && map[row - 1, col] == 4;
        bool iwalldown = row < rows - 1 && map[row + 1, col] == 4;
        bool iwallleft = col > 0 && map[row, col - 1] == 4;
        bool iwallright = col < cols - 1 && map[row, col + 1] == 4;
        switch (tile)
        {
            case 0: Instantiate(Empty, worldPos, Quaternion.identity, transform); break;
            case 1:
                Quaternion rotation1 = Quaternion.identity;
                if (wallup && wallleft && right && down) rotation1 = Quaternion.Euler(0, 0, 180);
                else if (wallup && wallright && down && left) rotation1 = Quaternion.Euler(0, 0, 90);
                else if (walldown && wallleft && up && right) rotation1 = Quaternion.Euler(0, 0, -90);
                else if (walldown && wallright && up && left) rotation1 = Quaternion.Euler(0, 0, 0);
                Instantiate(OuterCorner, worldPos, rotation1, transform);
                break;
            case 2:
                Quaternion rotation2 = Quaternion.identity;
                if (right) rotation2 = Quaternion.Euler(0, 0, 90);
                else if (left) rotation2 = Quaternion.Euler(0, 0, -90);
                else if (up) rotation2 = Quaternion.Euler(0, 0, 180);
                else if (down) rotation2 = Quaternion.Euler(0, 0, 0);
                Instantiate(Outerwall, worldPos, rotation2, transform);
                break;
            case 3:
                Quaternion rotation3 = Quaternion.identity;
                if (down && left) rotation3 = Quaternion.Euler(0, 0, 90);
                else if (down && right) rotation3 = Quaternion.Euler(0, 0, 180);
                else if (up && left) rotation3 = Quaternion.Euler(0, 0, 0);
                else if (up && right) rotation3 = Quaternion.Euler(0, 0, -90);
                Instantiate(InnerCorner, worldPos, rotation3, transform);
                break;
            case 4:
                Quaternion rotation4 = Quaternion.identity;
                if (up) rotation4 = Quaternion.Euler(0, 0, 90);
                else if (down) rotation4 = Quaternion.Euler(0, 0, -90);
                else if (right) rotation4 = Quaternion.Euler(0, 0, 0);
                else if (left) rotation4 = Quaternion.Euler(0, 0, 180);
                Instantiate(Innerwall, worldPos, rotation4, transform);
                break;
            case 5:
                Instantiate(Floor, worldPos, Quaternion.identity, transform);
                Instantiate(Pellet, worldPos, Quaternion.identity, transform);
                break;
            case 6:
                Instantiate(Floor, worldPos, Quaternion.identity, transform);
                Instantiate(PowerPellet, worldPos, Quaternion.identity, transform);
                break;
            case 7:
                Instantiate(Tjunction, worldPos, Quaternion.identity, transform); break;
            case 8:
                Instantiate(GhostExit, worldPos, Quaternion.identity, transform); break;
        }
    }
}
