using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Transform player;
    public Vector2 arenaSize;

    // How long the object should shake for
    public static float shakeDuration = 0f;

    // How hard should the shaking be
    [Range(0f, 2f)]
    public static float shakeAmount = 0.15f;

    private Vector3 originalPos;

	// Use this for initialization
	void Start ()
    {
        shakeDuration = 0;
        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // update original pos
        originalPos = player.position;
        originalPos.z = -10;

        // clamp
        float camSize = GetComponent<Camera>().orthographicSize;
        float aspect = GetComponent<Camera>().aspect;
        originalPos.x = Mathf.Clamp(originalPos.x, -arenaSize.x + camSize * aspect, arenaSize.x - camSize * aspect);
        originalPos.y = Mathf.Clamp(originalPos.y, -arenaSize.y + camSize, arenaSize.y - camSize);

        // screenshake
        if (shakeDuration > 0)
        {
            transform.position = originalPos + (Random.insideUnitSphere * shakeAmount);
            shakeDuration -= Time.deltaTime;
        }
        else
        {
            shakeDuration = 0;
            transform.position = originalPos;
        }
	}
}
