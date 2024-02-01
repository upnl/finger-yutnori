using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LoginManager : MonoBehaviour
{
	private const int numOfPlayer = 2;
	int numOfCurPlayer = 1;
	[SerializeField] private TMP_InputField inputPlayerName;
	private string playerName;
	private string playerHand;
	
	[SerializeField] private ToggleGroup handToggleGroup;
	private Toggle playerHandToggle;
	
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

	public void ClickContinueButton()
	{
		if(numOfCurPlayer<numOfPlayer)
		{
			numOfCurPlayer++;
			playerName = inputPlayerName.text;
			playerHand = handToggleGroup.ActiveToggles().FirstOrDefault().name;
			Debug.Log(playerName);
			Debug.Log(playerHand);
			return;
		}
		// playerprefs 저장

		SceneManager.LoadScene("Yutnori");
	}


	public void asd()
	{
		
	}

}
