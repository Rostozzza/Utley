﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	JsonManager JsonManager = new JsonManager(true);
	RequestManager RequestManager = new RequestManager(true);
	[SerializeField] private string currentPLayerName;
	[SerializeField] private string currentPlayerPassword;
	[SerializeField] private TextMeshProUGUI currentPlayerField;
	[SerializeField] private GameObject loadingView;
	[SerializeField] private List<GameObject> tutor;
	[SerializeField] private bool isPauseMenuActive = false;
	private Dictionary<LineRenderer, bool> linesStates = new();
	[Header("Game message settings")]
	[SerializeField] private NotificationsManager notificationsManager;
	private Coroutine messageViewRoutine;
	[Header("Screens")]
	[SerializeField] private GameObject startingScreen;
	[SerializeField] private GameObject registrationScreen;
	[SerializeField] private GameObject loginScreen;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private Slider loadingBar;
	[SerializeField] public GameObject mainMenuScreen;
	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject loseScreen;
	[SerializeField] private GameObject winScreen;
	[SerializeField] private List<GameObject> buttonsToHide;
	[SerializeField] private List<GameObject> buttonsToShow;
	[SerializeField] private List<TextMeshProUGUI> scores;
	[SerializeField] public GameObject menuBG;
	[SerializeField] public GameObject problemSolverScreen;
	[SerializeField] public GameObject SetPipesScreen;
	[SerializeField] public GameObject connectFurnacesScreen;
	[SerializeField] public GameObject shopScreen;
	[SerializeField] public GameObject setResistorsScreen;
	//[SerializeField] private List<TextMeshProUGUI> scores;
	[Header("Inputs")]
	[SerializeField] private TMP_InputField registrationUsernameField;
	[SerializeField] private TMP_InputField registrationPasswordField;
	[SerializeField] private TMP_InputField loginUsernameField;
	[SerializeField] private TMP_InputField loginPasswordField;
	[SerializeField] private GameObject continueGameButton;
	[Header("Audio")]
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider SFXSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private AudioSource BGMusicSource;
	[Header("API")]
	public bool isAPIActive;
	[Header("Cursor")]
	[SerializeField] private Texture2D cursorDefault;
	[SerializeField] private Texture2D cursorClick;
	[SerializeField] private Texture2D cursorDrag;
	[Header("Cutscenes")]
	[SerializeField] VideoPlayer videoPlayer;
	[SerializeField] VideoClip firstCutscene;
	[SerializeField] VideoClip menuClip;
	[SerializeField] VideoClip secondCutscene;
	[Header("Cosmodrome Resistors Problem")]
	[SerializeField] private CosmodromeResistorsExercise cosmodromeResistors;
	[Header("Number Summation Problem")]
	[SerializeField] private NumberSummationExercise numberSummation;
	[Header("Number By Table Exercise")]
	[SerializeField] private NumbersByTableExercise numbersByTable;
	[SerializeField] public Animator tabletAnimator;
	[Header("Supply Room Graph Exercise")]
	[SerializeField] public SupplyRoomGraphExercise graphExercise;
	private bool canContinueAfter2Cutscene = false;
	private Coroutine skipChecker;
	public bool isPlayerLoadable = false;

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
		List<LineRenderer> lines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None).ToList(); // hides graph lines
		lines.ForEach(x => x.enabled = false);
		//SwitchHideLinesVFX(true);

		if (ShopManager.Instance.GetIsOpen()) ShopManager.Instance.OpenShop();
		EventManager.onGameEnd.Invoke(false);
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
			group.alpha += Time.deltaTime * 2f;
			if (group.alpha >= 1f)
			{
				break;
			}
			yield return null;
		}
	}

	public void ShowWinScreen()
	{
		List<LineRenderer> lines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None).ToList(); // hides graph lines
		lines.ForEach(x => x.enabled = false);
		//SwitchHideLinesVFX(true);

		if (ShopManager.Instance.GetIsOpen()) ShopManager.Instance.OpenShop();
		EventManager.onGameEnd.Invoke(true);
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
		int scoreHNY = (int)(await game.GetHNY() * 550f);
		int scoreScienceSample = (int)(await game.GetPrototype() * 480f);
		int scoreAstroluminite = (int)(await game.GetAstroluminite() * 96f);
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

		scores.ForEach(x => x.text = $"Итого очков:\n\nОстаток астерия: {scoreAsteriy,16}\nОстаток энергомеда: {scoreHoney,13}\nПостройка комплексов: {scoreRooms,11}\nОстаток М.Е.Д.: {scoreHNY,17}\nОстаток образцов: {scoreScienceSample,15}\nОстаток астролуминита: {scoreAstroluminite,10}\nОстаток урсовокс: {scoreUrsowax,15}\nДлина смены: {scoreTime,20}\nОстаток прочности: {scoreDurabilitySum,14}\nУровень команды: {scoreBearsLevelsSum,16}\n\nИтого: {scoreTotal,26}");
	}

	public void ActivateAPI()
	{
		isAPIActive = true;
		JsonManager = new JsonManager(isAPIActive);
		if (isPlayerLoadable)
		{
			continueGameButton.SetActive(true);
		}
	}
	public void DeactivateAPI()
	{
		isAPIActive = false;
		JsonManager = new JsonManager(isAPIActive);
		continueGameButton.SetActive(false);
	}

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			problemSolverScreen.SetActive(false);
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		videoPlayer.loopPointReached += OnVideoEnd;

		skipChecker = StartCoroutine(SkipChecker());
		if (PlayerPrefs.GetString("currentPlayer") != "")
		{
			currentPLayerName = PlayerPrefs.GetString("currentPlayer");
			currentPlayerField.text = currentPLayerName;

			startingScreen.SetActive(false);
			mainMenuScreen.SetActive(false);
		}
		tabletAnimator = ShopManager.Instance.animator;
		SceneManager.activeSceneChanged += (Scene oldScene, Scene newScene) =>
		{
			try { GetComponent<Canvas>().worldCamera = Camera.main.GetComponentsInChildren<Camera>()[1]; } catch {}
			if (numberSummation.isTaskActive)
			{
				Time.timeScale = 1f;
				numberSummation.ClearGraph();
				numberSummation.isTaskActive = false;
				tabletAnimator.SetTrigger("CloseShop");
			}
			if (SceneManager.GetActiveScene().buildIndex != 0)
			{
				BGMusicSource = GameObject.FindGameObjectWithTag("BG_music").GetComponent<AudioSource>();
			}
		};
	}

	private void SwitchHideLinesVFX(bool setPauseMenuAfter) // switch because it's assumes that it will be only changing.
	{
		if (isPauseMenuActive) // from menu to game
		{
			foreach (LineRenderer line in linesStates.Keys)
			{
				line.enabled = linesStates[line];
			}
		}
		else // from game to menu
		{
			List<LineRenderer> lines = FindObjectsByType<LineRenderer>(FindObjectsSortMode.None).ToList();
			Dictionary<LineRenderer, bool> newLinesStates = new();
			foreach (var line in lines) newLinesStates.Add(line, line.enabled);
			linesStates = newLinesStates;

			lines.ForEach(x => x.enabled = false);
		}
		isPauseMenuActive = setPauseMenuAfter;
	}

	public void Pause()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}

		SwitchHideLinesVFX(true);
		pauseScreen.SetActive(true);
		//SetPipesScreen.SetActive(!numberSummation.isTaskActive);
		Time.timeScale = 0f;
		mixer.SetFloat("Lowpass", 500f);
		if (SceneManager.GetActiveScene().buildIndex != 0)
		{
			BGMusicSource.Pause();
		}
		try
		{
			GameObject.FindGameObjectWithTag("tutorial").GetComponent<Canvas>().sortingOrder = 9999; // God left us;
		} catch {}
	}

	public void Resume()
	{
		if (SceneManager.GetActiveScene().buildIndex == 0)
		{
			return;
		}

		SwitchHideLinesVFX(false);
		//SetPipesScreen.SetActive(numberSummation.isTaskActive);
		pauseScreen.SetActive(false);
		Time.timeScale = 1f;
		mixer.SetFloat("Lowpass", 22000f);
		if (SceneManager.GetActiveScene().buildIndex != 0)
		{
			BGMusicSource.UnPause();
		}
		try
		{
			GameObject.FindGameObjectWithTag("tutorial").GetComponent<Canvas>().sortingOrder = 32767; // 😭;
		} catch {}
	}

	public void ToMenu()
	{
		EventManager.onToMenuButton.Invoke();

		if (ShopManager.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("TabletShow"))
		{
			Debug.Log("<color=red>ЗАКРЫЛИ</color>");
			ShopManager.Instance.transform.SetSiblingIndex(0);
			tabletAnimator.speed = 10000;
			tabletAnimator.SetTrigger("CloseShop");
			Invoke(nameof(SetNormalTabletAnimatorState), 0.1f);
		}
		videoPlayer.clip = menuClip;
		loseScreen.SetActive(false);
		loseScreen.GetComponent<CanvasGroup>().alpha = 0f;
		winScreen.SetActive(false);
		winScreen.GetComponent<CanvasGroup>().alpha = 0f;
		pauseScreen.SetActive(false);
		isPauseMenuActive = false;
		EventManager.onGameEnd.Invoke(false);
		StartCoroutine(ToMenuCoroutine());
	}

	private void SetNormalTabletAnimatorState() // I sorry for that;
	{
		ShopManager.Instance.transform.SetSiblingIndex(1);
		tabletAnimator.speed = 1;
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
		isPauseMenuActive = false;
		loadingView.SetActive(false);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Update()
	{
		if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2)
		{
			if (InputController.GetKeyDown(ActionKeys.Pause))
			{
				Pause();
			}
			if (InputController.GetKeyDown(ActionKeys.OpenShop) && !problemSolverScreen.activeSelf && !isPauseMenuActive)
			{
				shopScreen.SetActive(true);
				Debug.Log("OPEN SHOP");
				ShopManager.Instance.OpenShop();
			}
		}
		if (Input.GetMouseButton(2))
		{
			Cursor.SetCursor(cursorDrag, Vector2.zero, CursorMode.Auto);
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				Cursor.SetCursor(cursorClick, Vector2.zero, CursorMode.Auto);
			}
			else
			{
				Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
			}
			if (Input.GetMouseButton(1))
			{
				Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
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
		currentPlayerPassword = registrationPasswordField.text;
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
		if (JsonManager.IsPlayerLoadable(requestedPlayer))
		{
			continueGameButton.SetActive(true);
			isPlayerLoadable = true;
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
		StartCoroutine(LoadingScreenCoroutine(0));
	}

	public void NewGame()
	{
		buttonsToHide.ForEach(x => x.SetActive(false));
		buttonsToShow.ForEach(x => x.SetActive(true));
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		StartCoroutine(LoadingScreenCoroutine(1));
	}

	private IEnumerator LoadingScreenCoroutine(int state)
	{
		if (state == 1)
		{
			yield return Cutscene2();
		}
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
			try
			{
				GameManager.Instance.asteriy = ValuesHolder.StartAsterium; // 40
				GameManager.Instance.honey = ValuesHolder.StartEnergohoney; // 40
				GameManager.Instance.astroluminite = ValuesHolder.StartAstroluminite; // 6
				GameManager.Instance.playerBears = 4; // 4
				GameManager.Instance.uiResourceShower.UpdateIndicators();
			}
			catch
			{
				Debug.Log("L");
			}
		}
		else
		{
			if (state == 1)
			{
				yield return JsonManager.RefillExistingShop(currentPLayerName);
				yield return JsonManager.ResetExistingPlayer(currentPLayerName, currentPlayerPassword);
			}
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

	private IEnumerator Cutscene2()
	{
		videoPlayer.clip = secondCutscene;
		skipChecker = StartCoroutine(SkipChecker());
		while (!canContinueAfter2Cutscene)
		{
			yield return null;
		}
	}

	private IEnumerator SkipChecker()
	{
		while (true)
		{
			if (Input.anyKeyDown)
			{
				yield return null;
				float timer = 1f;
				while (timer > 0)
				{
					if (Input.anyKeyDown)
					{
						OnVideoEnd(videoPlayer);
						skipChecker = null;
					}
					timer -= Time.deltaTime;
					yield return null;
				}
			}
			yield return null;
		}
	}

	public void OpenTutorial()
	{
		buttonsToHide.ForEach(x => x.SetActive(false));
		buttonsToShow.ForEach(x => x.SetActive(true));
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		SceneManager.LoadSceneAsync(2);
		videoPlayer.gameObject.SetActive(false);
		loadingBar.value = 0;
		loadingScreen.SetActive(false);
		Time.timeScale = 1f;
		loadingView.SetActive(false);
		try
		{
			GameManager.Instance.asteriy = ValuesHolder.StartAsterium;
			GameManager.Instance.honey = ValuesHolder.StartEnergohoney;
			GameManager.Instance.astroluminite = ValuesHolder.StartAstroluminite;
			GameManager.Instance.playerBears = 4;
			GameManager.Instance.uiResourceShower.UpdateIndicators();
		}
		catch
		{
			Debug.Log("LL");
		}
	}

	private void OnVideoEnd(VideoPlayer vp)
	{
		if (videoPlayer.clip == firstCutscene)
		{
			videoPlayer.isLooping = true;
			videoPlayer.clip = menuClip;
			mainMenuScreen.SetActive(true);
		}
		else if (videoPlayer.clip == secondCutscene)
		{
			canContinueAfter2Cutscene = true;
			videoPlayer.gameObject.SetActive(false);
		}
	}

	private async Task ContinueGameAsync(int state)
	{
		buttonsToHide.ForEach(x => x.SetActive(false));
		buttonsToShow.ForEach(x => x.SetActive(true));
		loadingView.SetActive(true);
		mainMenuScreen.SetActive(false);
		StartCoroutine(LoadingScreenCoroutine(state));
		//SceneManager.LoadSceneAsync(1);
	}

	public void CallProblemSolver(ProblemType type, RoomScript room)
	{
		shopScreen.SetActive(false);
		switch (type)
		{
			case ProblemType.SetPipes:
				SetPipesScreen.SetActive(true);
				StartCoroutine(WaitForNumberSummationEnd(room));
				break;
			case ProblemType.SetFurnaces:
				numbersByTable.gameObject.SetActive(true);
				StartCoroutine(WaitForFurnacesEnd(room));
				break;
			case ProblemType.SetSupply:
				problemSolverScreen.SetActive(true);
				graphExercise.gameObject.SetActive(true);
				graphExercise.InitializeTask(room);
				break;
			case ProblemType.SetResistors:
				setResistorsScreen.SetActive(true);
				StartCoroutine(WaitForResistorsCountEnd(room));
				break;
		}
		GameManager.Instance.SetIsGraphUsing(true);
		tabletAnimator.SetTrigger("OpenShop");
		
	}

	private IEnumerator WaitForFurnacesEnd(RoomScript room)
	{
		problemSolverScreen.SetActive(true);
		numbersByTable.GenerateTask(room);
		yield return new WaitForSeconds(1.5f);
	}

	private IEnumerator WaitForNumberSummationEnd(RoomScript room)
	{
		numberSummation.isTaskActive = true;
		problemSolverScreen.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		yield return numberSummation.AnswerWaiter(room);
		
		SetPipesScreen.SetActive(false);
		problemSolverScreen.SetActive(false);
		tabletAnimator.SetTrigger("CloseShop");
	}

	private IEnumerator WaitForResistorsCountEnd(RoomScript room)
	{
		problemSolverScreen.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		yield return cosmodromeResistors.AnswerWaiter(room);
		
		cosmodromeResistors.HideSample();
		setResistorsScreen.SetActive(false);
		problemSolverScreen.SetActive(false);
		tabletAnimator.SetTrigger("CloseShop");
	}

	public enum ProblemType
	{
		SetPipes,
		SetFurnaces,
		SetSupply,
		SetResistors
	}
}