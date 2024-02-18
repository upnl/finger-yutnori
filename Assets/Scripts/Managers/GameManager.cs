using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set;}	

	public UIManager UIManager { get; private set; }
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
	    
	    GameStateManager = GetComponentInChildren<GameStateManager>();
	    UIManager = GetComponentInChildren<UIManager>();
    }
    
    void Start() //시작할 때 사용자의 정보를 받아온다.
    {
	    player1.playerName = PlayerPrefs.GetString("player1Name");
	    player1.playerHand = PlayerPrefs.GetString("player1Hand");
	    
	    player2.playerName = PlayerPrefs.GetString("player2Name");
	    player2.playerHand = PlayerPrefs.GetString("player2Hand");

	    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
