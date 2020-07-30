using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableTrackerScript : MonoBehaviour
{
    public int wave;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
