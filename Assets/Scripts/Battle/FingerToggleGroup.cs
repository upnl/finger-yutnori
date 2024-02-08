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
    
    //Toggle이 선택되지 않았을 경우 False를 반환
    public bool IsToggleClicked()
    {
        if(SelectedFinger < 0)
        {
            return false;
        }
        return true;
    }
}
