using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "WalkableTile", menuName = "Tiles/WalkableTile")]
public class WalkableTile : Tile
{
    [Header("Tile Properties")]
    public bool isWalkable = true;
    public bool isPellet = false;
    public bool isPortal = false;
}
