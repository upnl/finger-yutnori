using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed : MonoBehaviour
{
    void Start()
    {
        SetResolution();
    }

    public void SetResolution()
    {
        Screen.SetResolution((Screen.height * 9 * 4) / 16 / 5, (Screen.height * 4) / 5, true);  
    }
}
