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

    //Toggle의 IsOn이 변화할 때 호출되는 메서드. 선택할 경우 자신의 SerialNumber을, 해제할 경우 -1을 SelectedFinger에 할당
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
        Debug.Log(_FingerToggleGroup.SelectedFinger.ToString());
    }
}
