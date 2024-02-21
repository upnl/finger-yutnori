using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerToggleGroup : MonoBehaviour
{
    public int SelectedFinger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// false when any toggle isn't selected
    /// </summary>
    /// <returns></returns>
    public bool IsToggleClicked()
    {
        if(SelectedFinger < 0)
        {
            return false;
        }
        return true;
    }
}
