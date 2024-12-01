using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	JsonManager JsonManager = new JsonManager();
	RequestManager RequestManager = new RequestManager();
	private string currentPLayerName;
	[SerializeField] private TextMeshProUGUI currentPlayerField;
	[SerializeField] private GameObject loadingView;
	[Header("Game message settings")]
	[SerializeField] private CanvasGroup messageView;
	[SerializeField] private TextMeshProUGUI messageField;
	private Coroutine messageViewRoutine;
	[Header("Screens")]
	[SerializeField] private GameObject startingScreen;
	[SerializeField] private GameObject registrationScreen;
	[SerializeField] private GameObject loginScreen;
	[SerializeField] private GameObject mainMenuScreen;
	[Header("Inputs")]
	[SerializeField] private TMP_InputField registrationUsernameField;
	[SerializeField] private TMP_InputField registrationPasswordField;
	[SerializeField] private TMP_InputField loginUsernameField;
	[SerializeField] private TMP_InputField loginPasswordField;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void Start()
	{
		if (PlayerPrefs.GetString("currentPlayer") != "")
		{
			currentPLayerName = PlayerPrefs.GetString("currentPlayer");
			currentPlayerField.text = currentPLayerName;
			startingScreen.SetActive(false);
			mainMenuScreen.SetActive(true);
		}
	}

	public void Quit()
	{
		Application.Quit();
	}

	public async void Registrate()
	{
		loadingView.SetActive(true);
		var requestedPlayer = await RequestManager.GetPlayer(registrationUsernameField.text);
		if (requestedPlayer == null)
		{
			Player newPLayer = await JsonManager.CreateNewPlayer(registrationUsernameField.text, registrationPasswordField.text);
		}
		else
		{
			Debug.Log("PLayer exist! (Log from MenuManager.cs)");
			return;
		}
		currentPLayerName = registrationUsernameField.text;
		PlayerPrefs.SetString("currentPlayer", currentPLayerName);
		loadingView.SetActive(false);
	}

	public async void Login()
	{
		loadingView.SetActive(true);
		var requestedPlayer = await RequestManager.GetPlayer(loginUsernameField.text);
		loadingView.SetActive(false);
		if (requestedPlayer == null)
		{
			return;
		}
		if (requestedPlayer.resources["password"] != loginPasswordField.text)
		{
			Debug.Log("Incorrect password!");
			return;
		}
		currentPLayerName = loginUsernameField.text;
		PlayerPrefs.SetString("currentPlayer",currentPLayerName);
		currentPlayerField.text = currentPLayerName;
		loginScreen.SetActive(false);
		mainMenuScreen.SetActive(true);
	}

	public void LogOut()
	{
		if (currentPLayerName != null)
		{
			PlayerPrefs.SetString("currentPlayer","");
			currentPlayerField.text = "";
			currentPLayerName = null;
			mainMenuScreen.SetActive(false);
			startingScreen.SetActive(true);
		}
	}

	//DO NOT USE!
	public async void CreateGAme()
	{
		Debug.Log(await RequestManager.GeenrateUUID());
	}

	public async void ContinueGame()
	{
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		await SceneManager.LoadSceneAsync(1);
		/*Time.timeScale = 0f;
		GameManager.Instance.playerName = currentPLayerName;
		var requestedPlayer = await RequestManager.GetPlayer(currentPLayerName);
		if (requestedPlayer == null)
		{
			Debug.Log("No player present!");
			loadingView.SetActive(false);
			return;
		}
		JsonManager.LoadPlayerFromModel(requestedPlayer);*/
		loadingView.SetActive(false);
		Time.timeScale = 1f;
	}
}