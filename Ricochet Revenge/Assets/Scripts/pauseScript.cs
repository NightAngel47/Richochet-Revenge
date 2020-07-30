using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseScript : MonoBehaviour
{
    public bool paused;
    public GameObject pausepanel;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            paused = !paused;
            pausepanel.SetActive(paused);


        }
        if (paused)
        {
            Time.timeScale = 0;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else 
        {
            Time.timeScale = 1;
            Cursor.SetCursor(GameObject.FindWithTag("Player").GetComponent<PlayerController>().crosshairTexture, new Vector2(16, 16), CursorMode.Auto);
        }
    }
    public void pause()
    {
        paused = !paused;
        pausepanel.SetActive(paused);
    }
}