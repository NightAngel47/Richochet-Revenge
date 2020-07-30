using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenSwitch : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject mainMenuPanel;

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && titleScreen.activeSelf)
        {
            titleScreen.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}
