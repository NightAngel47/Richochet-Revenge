using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCanvasController : MonoBehaviour
{
    public GameObject player;

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        Vector3 position = player.transform.position;
        position.z = transform.position.z;

        transform.position = position;
    }
}
