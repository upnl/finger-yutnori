using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public List<GameObject> gameObjectNotToSeeList;

    Color imageColor;
    private bool isClicked = false;

    private void Start()
    {
        imageColor = this.GetComponent<Image>().color;
    }

    public void OnClick()
    {
        if (isClicked == false)
        {
            imageColor.a = 0f;
            this.GetComponent<Image>().color = imageColor;

            foreach (GameObject gameObjectNotToSee in gameObjectNotToSeeList)
            {
                gameObjectNotToSee.SetActive(false);
            }

            isClicked = true;
        }
        else
        {
            imageColor.a = 0.8627451f;
            this.GetComponent<Image>().color = imageColor;

            foreach (GameObject gameObjectNotToSee in gameObjectNotToSeeList)
            {
                gameObjectNotToSee.SetActive(true);
            }

            isClicked = false;
        }
    }

}
