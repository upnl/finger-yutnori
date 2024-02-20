using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerToggle : MonoBehaviour
{
    [SerializeField]private int SerialNumber;
    [SerializeField]private FingerToggleGroup _FingerToggleGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// allocates its serialnumber to selectedfinger when something has been selected
    /// but allocates -1 when someting has been released
    /// </summary>
    /// <param name="isOn"></param>
    public void ToggleClick(bool isOn)
    {
        if(_FingerToggleGroup.SelectedFinger < 0)
        {
            _FingerToggleGroup.SelectedFinger = SerialNumber;
        }
        else
        {
            _FingerToggleGroup.SelectedFinger = -1;
        }
    }
}
