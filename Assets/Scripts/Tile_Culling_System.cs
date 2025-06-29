using UnityEngine;

public class TileCullingSystem : MonoBehaviour
{
    [SerializeField] float cullingDistance = 10f;
    [SerializeField] float checkInterval = 0.5f;

    private TileBehaviour[] allTiles;

    public void Initialise()
    {
        allTiles = FindObjectsOfType<TileBehaviour>();
        InvokeRepeating(nameof(UpdateTileVisibility), 0f, checkInterval);
    }

    void UpdateTileVisibility()
    {
        Vector3 playerPos = GameManager.playerData[0].playerTransform.position;

        foreach (var tile in allTiles)
        {
            float dist = Vector3.Distance(tile.transform.position, playerPos);
            tile.SetVisible(dist <= cullingDistance);
        }
    }
}
