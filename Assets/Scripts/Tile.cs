using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    public TileData _tileData;

    [SerializeField]
    public WFCTile _tile;
    
    public void Initialize(WFCTile tile)
    {
        _tile = tile;
        _tileData = tile.options[0];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);
    }
}