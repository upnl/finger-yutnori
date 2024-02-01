using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class LoginManager : MonoBehaviour
{
	
	[SerializeField] private GameObject player1InputField;
	[SerializeField] private GameObject player2InputField;
	
	private ToggleGroup player1ToggleGroup;
	private ToggleGroup player2ToggleGroup;
	
	private string player1Name;
	private string player1Hand;
	
	private string player2Name;
	private string player2Hand;
	
    void Start()
    {
	    player1ToggleGroup = player1InputField.transform.Find("HandToggleGroup").GetComponent<ToggleGroup>();
	    player2ToggleGroup = player2InputField.transform.Find("HandToggleGroup").GetComponent<ToggleGroup>();
    }

    
    void Update()
    {
        
    }

	public void ClickContinueButton()
	{
		if (player1ToggleGroup.ActiveToggles().FirstOrDefault() == null) player1Hand = "LeftHand";
		else player1Hand = player1ToggleGroup.ActiveToggles().FirstOrDefault().name;
		
		if (player2ToggleGroup.ActiveToggles().FirstOrDefault() == null) player2Hand = "LeftHand";
		else player2Hand = player1ToggleGroup.ActiveToggles().FirstOrDefault().name;
		
		//플레이어가 선택한 손을 받아온다.


		string player1Name = player1InputField.transform.Find("InputPlayer").GetComponent<TMP_InputField>().text;
		if (player1Name.Equals("")) player1Name = "현명한 청년";
		string player2Name = player2InputField.transform.Find("InputPlayer").GetComponent<TMP_InputField>().text;
		if (player1Name.Equals("")) player2Name = "멍청한 감자칩";
		//inputfield에 입력받은 플레이어의 이름을 받아온다.
		
		PlayerPrefs.SetString("player1Name", player1Name);
		PlayerPrefs.SetString("player2Name", player2Name);
		PlayerPrefs.SetString("player1Hand", player1Hand);
		PlayerPrefs.SetString("player2Hand", player2Hand);
		//playerprefs를 이용해 플레이어의 정보를 저장한다.
		
		SceneManager.LoadScene("Battle");
	}
	

}
