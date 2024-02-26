using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
	public AudioManager AudioManager { get; private set; }
	
	[SerializeField] private GameObject player1InputField;
	[SerializeField] private GameObject player2InputField;
	[SerializeField] private Button button;
	[SerializeField] private TMP_Text emptyNameText;
	[SerializeField] private TMP_Text sameNameText;

	[SerializeField] private float moveSpeed; //에러메세지가 움직이는 속도
	[SerializeField] private float alphaSpeed; //에러메세지가 사라지는 속도
	private Color alphaSameColor; //에러메세지의 색
	private Color alphaEmptyColor;
	
	
	private ToggleGroup player1ToggleGroup;
	private ToggleGroup player2ToggleGroup;
	
	private string player1Name;
	private string player1Hand;
	
	private string player2Name;
	private string player2Hand;
	
    void Start()
    {
	    AudioManager = GetComponentInChildren<AudioManager>();
	    AudioManager.PlayBGM();
	    player1ToggleGroup = player1InputField.transform.Find("HandToggleGroup").GetComponent<ToggleGroup>();
	    player2ToggleGroup = player2InputField.transform.Find("HandToggleGroup").GetComponent<ToggleGroup>();

	    alphaSameColor = sameNameText.color;
	    alphaEmptyColor = emptyNameText.color;
	    sameNameText.gameObject.SetActive(false);
	    emptyNameText.gameObject.SetActive(false);
    }

    
    void Update()
    {
	    if (sameNameText.gameObject.activeSelf)
	    {
		    sameNameText.transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
		    alphaSameColor.a = Mathf.Lerp(alphaSameColor.a, 0, Time.deltaTime * alphaSpeed);
		    sameNameText.color = alphaSameColor;
	    }
	    
	    if (emptyNameText.gameObject.activeSelf)
	    {
		    emptyNameText.transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
		    alphaEmptyColor.a = Mathf.Lerp(alphaEmptyColor.a, 0, Time.deltaTime * alphaSpeed);
		    emptyNameText.color = alphaEmptyColor;
	    }
    }

	public void ClickContinueButton()
	{
		List<string> playerHandList = new() { "LeftHand", "RightHand" };
		if (player1ToggleGroup.AnyTogglesOn() == false) player1Hand = playerHandList[Random.Range(0, 2)];
        else player1Hand = player1ToggleGroup.ActiveToggles().FirstOrDefault().name;

        if (player2ToggleGroup.AnyTogglesOn() == false) player2Hand = playerHandList[Random.Range(0, 2)];
        else player2Hand = player2ToggleGroup.ActiveToggles().FirstOrDefault().name;
		//플레이어가 선택한 손을 받아온다.


		player1Name = player1InputField.transform.Find("InputPlayerName").GetComponent<TMP_InputField>().text;
		player2Name = player2InputField.transform.Find("InputPlayerName").GetComponent<TMP_InputField>().text;
		//inputfield에 입력받은 플레이어의 이름을 받아온다.
		if (player1Name.Equals("") || player2Name.Equals(""))
		{
			emptyNameText.gameObject.SetActive(true);
			emptyNameText.transform.localPosition = new Vector3(0,500,0);
			alphaEmptyColor.a = 1f;
			return;
		}
			
		if (player1Name.Equals(player2Name))
		{
			sameNameText.gameObject.SetActive(true);
			sameNameText.transform.localPosition = new Vector3(0, 500, 0);
			alphaSameColor.a = 1f;
			return;
		}
		
		PlayerPrefs.SetString("player1Name", player1Name);
		PlayerPrefs.SetString("player2Name", player2Name);
		PlayerPrefs.SetString("player1Hand", player1Hand);
		PlayerPrefs.SetString("player2Hand", player2Hand);
		//playerprefs를 이용해 플레이어의 정보를 저장한다.
		
		SceneManager.LoadScene("Yutnori");
	}
	

}
