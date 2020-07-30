using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveNumberBehaviour : MonoBehaviour
{
    public Text myText;

    void Start()
    {
        int wave = GameObject.FindWithTag("VarTrack").GetComponent<VariableTrackerScript>().wave;
        myText.text = "Wave: " + wave;
    }
}
