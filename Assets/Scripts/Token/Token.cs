using UnityEngine;
using DG.Tweening;

public class Token : MonoBehaviour
{
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
