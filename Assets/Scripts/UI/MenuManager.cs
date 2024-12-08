using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	JsonManager JsonManager = new JsonManager(true);
	RequestManager RequestManager = new RequestManager(true);
	[SerializeField] private string currentPLayerName;
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
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private Slider loadingBar;
	[SerializeField] private GameObject mainMenuScreen;
	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject loseScreen;
	[SerializeField] private GameObject winScreen;
	[SerializeField] private List<GameObject> buttonsToHide;
	[SerializeField] private List<GameObject> buttonsToShow;
	[SerializeField] private List<TextMeshProUGUI> scores;
	[SerializeField] private GameObject menuBG;
	//[SerializeField] private List<TextMeshProUGUI> scores;
	[Header("Inputs")]
	[SerializeField] private TMP_InputField registrationUsernameField;
	[SerializeField] private TMP_InputField registrationPasswordField;
	[SerializeField] private TMP_InputField loginUsernameField;
	[SerializeField] private TMP_InputField loginPasswordField;
	[Header("Audio")]
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider SFXSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private AudioMixerGroup musicGroup;
	[Header("API")]
	public bool isAPIActive;

	public void SetMasterVolume()
	{
		float volume = masterSlider.value;
		mixer.SetFloat("MasterVolume", volume < 0.01f ? -80 : Mathf.Log10(volume) * 20);
	}

	public void SetSFXVolume()
	{
		float volume = SFXSlider.value;
		mixer.SetFloat("SFXVolume", volume < 0.01f ? -80 : Mathf.Log10(volume) * 20);
	}

	public void SetMusicVolume()
	{
		float volume = musicSlider.value;
		mixer.SetFloat("MusicVolume", volume < 0.01f ? -80 : Mathf.Log10(volume) * 20);
	}

	public void ShowLoseScreen()
	{
		GameManager.Instance.SetIsGameRunning(false);
		CountScoreAndUpdate();
		loseScreen.SetActive(true);
		StartCoroutine(LoseScreenFadeIn());
	}

	private IEnumerator LoseScreenFadeIn()
	{
		var group = loseScreen.GetComponent<CanvasGroup>();
		while (true)
		{
			group.alpha += Time.deltaTime*2f;
			if (group.alpha >= 1f)
			{
				break;
			}
			yield return null;
		}
	}

	public void ShowWinScreen()
	{
		GameManager.Instance.SetIsGameRunning(false);
		CountScoreAndUpdate();
		winScreen.SetActive(true);
		StartCoroutine(WinScreenFadeIn());
	}

	private IEnumerator WinScreenFadeIn()
	{
		var group = winScreen.GetComponent<CanvasGroup>();
		while (true)
		{
			group.alpha += Time.deltaTime * 2f;
			if (group.alpha >= 1f)
			{
				break;
			}
			yield return null;
		}
		Time.timeScale = 0f;
	}

	private async void CountScoreAndUpdate()
	{
		GameManager game = GameManager.Instance;

		int scoreAsteriy = (int)(await game.GetAsteriy() * 10f);
		int scoreHoney = (int)(await game.GetHoney() * 15f);
		int scoreRooms = (int)((game.allRooms.Count - 7) * 500f);
		int scoreHNY = (int)(await game.GetHNY() * 80f);
		int scoreScienceSample = (int)(await game.GetPrototype() * 480f);
		int scoreAstroluminite = (int)(await game.GetAstroluminite() * 48f);
		int scoreUrsowax = (int)(await game.GetUrsowaks() * 400f);
		int scoreTime = (int)(game.GetTimePast() * 10f);
		int scoreDurabilitySum = (int)(game.allRooms.Where(x => !x.CompareTag("elevator")).ToList().ConvertAll(y => y.GetComponent<RoomScript>().durability).Sum() * 100f * 10f); // all rooms without elevators multiplyes by 100 to get % and by 10 to get score
		int scoreBearsLevelsSum = (int)(game.bears.ConvertAll(x => x.GetComponent<UnitScript>().level).Sum() * 500f);

		int scoreTotal = scoreAsteriy + scoreHoney + scoreRooms + scoreHNY + scoreScienceSample + scoreAstroluminite + scoreUrsowax + scoreTime + scoreDurabilitySum + scoreBearsLevelsSum;

		//\\//\\//\\//\\//\\//\\//\\//\\//\\
		
		//scores[0].text = Convert.ToString(scoreAsteriy);
		//scores[1].text = Convert.ToString(scoreHoney);
		//scores[2].text = Convert.ToString(scoreRooms);
		//scores[3].text = Convert.ToString(scoreHNY);
		//scores[4].text = Convert.ToString(scoreScienceSample);
		//scores[5].text = Convert.ToString(scoreAstroluminite);
		//scores[6].text = Convert.ToString(scoreUrsowax);
		//scores[7].text = Convert.ToString(scoreTime);
		//scores[8].text = Convert.ToString(scoreDurabilitySum);
		//scores[9].text = Convert.ToString(scoreBearsLevelsSum);
//
		//scores[10].text = Convert.ToString(total);

		scores.ForEach(x => x.text = $"Итого очков:\n\nОстаток астерия: {scoreAsteriy, 16}\nОстаток энергомеда: {scoreHoney, 13}\nПостройка комплексов: {scoreRooms, 11}\nОстаток М.Е.Д.: {scoreHNY, 17}\nОстаток образцов: {scoreScienceSample, 15}\nОстаток астролуминита: {scoreAstroluminite, 10}\nОстаток урсовокс: {scoreUrsowax, 15}\nДлина смены: {scoreTime, 20}\nОстаток прочности: {scoreDurabilitySum, 14}\nУровень команды: {scoreBearsLevelsSum, 16}\n\nИтого: {scoreTotal, 26}");
	}

	public void ActivateAPI()
	{
		isAPIActive = true;
	}
	public void DeactivateAPI()
	{
		isAPIActive = false;
	}

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
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

	public void Pause()
	{
		if (SceneManager.GetActiveScene().buildIndex != 1)
		{
			return;
		}

		pauseScreen.SetActive(true);
		Time.timeScale = 0f;
		mixer.SetFloat("Lowpass", 500f);
	}

	public void Resume()
	{
		if (SceneManager.GetActiveScene().buildIndex != 1)
		{
			return;
		}
		pauseScreen.SetActive(false);
		Time.timeScale = 1f;
		mixer.SetFloat("Lowpass", 22000f);
	}

	public void ToMenu()
	{
		loseScreen.SetActive(false);
		loseScreen.GetComponent<CanvasGroup>().alpha = 0f;
		winScreen.SetActive(false);
		winScreen.GetComponent<CanvasGroup>().alpha = 0f;
		pauseScreen.SetActive(false);
		StartCoroutine(ToMenuCoroutine());
	}

	private IEnumerator ToMenuCoroutine()
	{
		var operation = SceneManager.LoadSceneAsync(0);
		loadingScreen.SetActive(true);
		while (!operation.isDone)
		{
			loadingBar.value = (operation.progress / 0.9f);
			yield return null;
		}
		menuBG.SetActive(true);
		loadingBar.value = 0;
		loadingScreen.SetActive(false);
		mainMenuScreen.SetActive(true);
		mixer.SetFloat("Lowpass", 22000f);
		buttonsToHide.ForEach(x => x.SetActive(true));
		buttonsToShow.ForEach(x => x.SetActive(false));
		Time.timeScale = 1f;
		loseScreen.SetActive(false);
		loseScreen.GetComponent<CanvasGroup>().alpha = 0f;
		winScreen.SetActive(false);
		winScreen.GetComponent<CanvasGroup>().alpha = 0f;
		pauseScreen.SetActive(false);
		loadingView.SetActive(false);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 1)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Pause();
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				ShopManager.Instance.OpenShop();
			}
		}
	}

	public async void Registrate()
	{
		loadingView.SetActive(true);
		var requestedPlayer = await RequestManager.GetPlayer(registrationUsernameField.text);
		if (requestedPlayer == null)
		{
			Debug.Log("Initializing player creation..");
			await JsonManager.CreateNewPlayer(registrationUsernameField.text, registrationPasswordField.text);
			await JsonManager.InitializeShop(registrationUsernameField.text);
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
		PlayerPrefs.SetString("currentPlayer", currentPLayerName);
		currentPlayerField.text = currentPLayerName;
		loginScreen.SetActive(false);
		mainMenuScreen.SetActive(true);
	}

	public void LogOut()
	{
		if (currentPLayerName != null)
		{
			PlayerPrefs.SetString("currentPlayer", "");
			currentPlayerField.text = "";
			currentPLayerName = null;
			mainMenuScreen.SetActive(false);
			startingScreen.SetActive(true);
		}
	}

	//DO NOT USE!
	//public async void CreateGAme()
	//{
	//	Debug.Log(await RequestManager.GeenrateUUID());
	//}

	public void ContinueGame()
	{
		buttonsToHide.ForEach(x => x.SetActive(false));
		buttonsToShow.ForEach(x => x.SetActive(true));
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		StartCoroutine(LoadingScreenCoroutine());
	}

	private IEnumerator LoadingScreenCoroutine()
	{
		var operation = SceneManager.LoadSceneAsync(1);
		loadingScreen.SetActive(true);
		while (!operation.isDone)
		{
			loadingBar.value = (operation.progress / 0.9f);
			yield return null;
		}
		if (!isAPIActive)
		{
			loadingBar.value = 0;
			loadingScreen.SetActive(false);
			Time.timeScale = 1f;
			loadingView.SetActive(false);
			GameManager.Instance.asteriy = 600;
			GameManager.Instance.honey = 100;
			GameManager.Instance.astroluminite = 20;
			GameManager.Instance.uiResourceShower.UpdateIndicators();
		}
		else
		{
			yield return RequestManager.GetPlayerEnum(currentPLayerName);
			ShopManager.Instance.isAPIActive = true;
			//Debug.Log(GameManager.Instance.playerModel.resources["elevators"]);
			GameManager.Instance.isAPIActive = isAPIActive;
			GameManager.Instance.JsonManager = new JsonManager(isAPIActive);
			GameManager.Instance.RequestManager = new RequestManager(isAPIActive);
			loadingBar.value = 0;
			GameManager.Instance.JsonManager.LoadPlayerFromModel(GameManager.Instance.playerModel);
			loadingScreen.SetActive(false);
			Time.timeScale = 1f;
			loadingView.SetActive(false);
		}
	}

	private async Task ContinueGameAsync()
	{
		buttonsToHide.ForEach(x => x.SetActive(false));
		buttonsToShow.ForEach(x => x.SetActive(true));
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		StartCoroutine(LoadingScreenCoroutine());
		//SceneManager.LoadSceneAsync(1);
	}
}