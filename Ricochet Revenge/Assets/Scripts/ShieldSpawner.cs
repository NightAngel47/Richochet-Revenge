using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShieldSpawner : MonoBehaviour
{
    public GameObject shieldPowerUp;

    public Camera cam;
    public Vector2 arenaSize;
    public GameObject player;
    public Tilemap tilemap;

    [Range(1f, 5f)]
    public float spawnRadiusMin = 1f;
    [Range(5f, 10f)]
    public float spawnRadiusMax = 5f;

    public void SpawnShield()
    {
        Vector2 spawnPos = spawnPoint();

        float camSize = cam.GetComponent<Camera>().orthographicSize;
        float aspect = cam.GetComponent<Camera>().aspect;
        spawnPos.x = Mathf.Clamp(spawnPos.x, -arenaSize.x + camSize * aspect, arenaSize.x - camSize * aspect);
        spawnPos.y = Mathf.Clamp(spawnPos.y, -arenaSize.y + camSize, arenaSize.y - camSize);

        Instantiate(shieldPowerUp, spawnPos, Quaternion.identity);
    }

    private Vector2 spawnPoint()
    {
        var pos = player.GetComponent<Rigidbody2D>().position + UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);

        // verify
        var hit = Physics2D.CircleCast(pos, 1, Vector2.zero, 1, 9);

        if (hit.collider)
        {
            if (hit.collider.GetComponent<Tilemap>())
            {
                if (tilemap.GetColliderType(tilemap.WorldToCell(hit.transform.position)) != Tile.ColliderType.None)
                {
                    spawnPoint();
                }
            }
        }

        return pos;
    }
}
