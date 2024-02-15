using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set;}	

	
	public GameStateManager GameStateManager { get; private set; }

	public Player player1;
	public Player player2;


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
	    player1 = gameObject.AddComponent<Player>();
	    player1.playerName = PlayerPrefs.GetString("player1Name");
	    player1.playerHand = PlayerPrefs.GetString("player1Hand");
	    
	    player2 = gameObject.AddComponent<Player>();
	    player2.playerName = PlayerPrefs.GetString("player2Name");
	    player2.playerHand = PlayerPrefs.GetString("player2Hand");

	    GameStateManager = GetComponentInChildren<GameStateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
