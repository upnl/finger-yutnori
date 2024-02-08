using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerToggle : MonoBehaviour
{
    // Start is called before the first frame update
    private Toggle _Toggle;
    [SerializeField]private GameObject _ToggleGroup;
    [SerializeField] private int SerialNumber;
    private FingerToggleGroup _FingerToggleGroup;
    void Start()
    {
        _Toggle = GetComponent<Toggle>();
        _FingerToggleGroup = _ToggleGroup.GetComponent<FingerToggleGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleClick(bool isOn)
    {
        _FingerToggleGroup.SelectedFinger = SerialNumber;
    }
}
