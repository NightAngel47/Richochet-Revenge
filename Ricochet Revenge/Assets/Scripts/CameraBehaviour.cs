using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject player;
    public Vector2 arenaSize;

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set camera position to player position but keep camera bounds within arena bounds

        Vector3 position = transform.position;
        //position.z = transform.position.z;
        float camSize = GetComponent<Camera>().orthographicSize;
        float aspect = GetComponent<Camera>().aspect;
        position.x = Mathf.Clamp(position.x, -arenaSize.x + camSize*aspect, arenaSize.x - camSize*aspect);
        position.y = Mathf.Clamp(position.y, -arenaSize.y + camSize, arenaSize.y - camSize);
        transform.position = position;
    }
}
