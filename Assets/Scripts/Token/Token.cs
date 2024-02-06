using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Token : MonoBehaviour
{
    [SerializeField] private List<GameObject> rightPoints;
    [SerializeField] private List<GameObject> upperPoints;
    [SerializeField] private List<GameObject> leftPoints;
    [SerializeField] private List<GameObject> lowerPoints;
    [SerializeField] private List<GameObject> rightDiagPoints;
    [SerializeField] private List<GameObject> leftDiagPoints;
    [SerializeField] private GameObject CentralPoint;
    [SerializeField] private GameObject DownRightPoint;
    [SerializeField] private GameObject UpRightPoint;
    [SerializeField] private GameObject UpLeftPoint;
    [SerializeField] private GameObject DownLeftPoint;
    public void InstantMoveTo(float x, float y)
    {
        transform.position = new Vector3(x, y, 0);
    }
    public void MoveTo(float x, float y)
    {
        Vector3 newPosition = new Vector3(x, y, 0);
        transform.DOMove(newPosition, 1f);
    }
    
    public bool IsTokenAt(float x, float y)
    {
        return transform.position.x == x && transform.position.y == y;
    }
}
