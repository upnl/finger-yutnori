using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set;}	

    


    void Awake()
    {
	    if (Instance != null && Instance != this)
	    {
		    Destroy(this);
		    return;
	    }

	    Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
