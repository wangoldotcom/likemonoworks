using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    public int totalCandies; // 총 Candy 재화
    [SerializeField] private TextMeshProUGUI coinText; // Coin UI 텍스트
    [SerializeField] private TextMeshProUGUI candyText; // Candy UI 텍스트

    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private TextMeshProUGUI finalScoreText;
    [SerializeField]
    private TextMeshProUGUI clearScoreText;
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private GameObject scorePanel;
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject gameClearPanel;
    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private GameObject marketPanel;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private AudioClip startBGM;
    [SerializeField]
    private AudioClip gameBGM;
    [SerializeField]
    private AudioClip gameOverBGM;
    [SerializeField]
    private AudioClip gameClearBGM;
    [SerializeField]
    private GameObject scoreBoardPanel;
    [SerializeField]
    private TextMeshProUGUI scoreBoardText;
    [SerializeField]
    private TMP_InputField nameInputField;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button playAgainButton;
    [SerializeField]
    private Button pausePlayAgainButton;
    [SerializeField]
    public GameObject bulletPrefab;
    [SerializeField]
    public GameObject coinPrefab;
    [SerializeField]
    public GameObject candyPrefab;

    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject networkErrorPanel;

    [SerializeField]
    private GameObject endingCreditsPanel;
    [SerializeField]
    private VideoPlayer endingCreditsVideoPlayer;

    [SerializeField]
    private Button showScoreBoardButton;
    [SerializeField]
    private Button closeScoreBoardButton;

    [SerializeField]
    private GameObject[] hardModeEnemyPrefabs;
    [SerializeField]
    private GameObject[] hardModeBossPrefabs;

    [SerializeField]
    public Button removeBannerAdsButton;  // 배너 광고 제거 버튼
    [SerializeField]
    public Button removeInterstitialAdsButton;  // 전면 광고 제거 버튼
    [SerializeField]
    public Button openMarketButton;  // 마켓 패널 열기 버튼
    [SerializeField]
    public Button closeMarketButton;  // 마켓 패널 닫기 버튼
    [SerializeField]
    public Button healthPlus1Button;  // 첫 번째 체력 추가 버튼
    [SerializeField]
    public Button healthPlus2Button;  // 두 번째 체력 추가 버튼

    [SerializeField]
    private Image[] healthImages; // 하트 이미지를 참조하기 위한 배열
    [SerializeField]
    private Sprite fullHeartSprite; // 체력이 있을 때의 하트 이미지
    [SerializeField]
    private Sprite emptyHeartSprite; // 체력이 깎였을 때의 하트 이미지

    [SerializeField]
    private GameObject healthPanel; // 체력 패널

    private int currentLanguage; // 경고를 없애기 위해 사용하도록 설정

    [SerializeField] private GameObject optionsPanel; // 옵션 패널을 할당
    [SerializeField] private Button openOptionsButton; // 옵션 버튼
    [SerializeField] private Button closeOptionsButton; // 옵션 패널 내에서 닫기 버튼

    private PlayerHealth playerHealth;

    [SerializeField]
    private TextMeshProUGUI messageText; // 메시지를 표시할 TextMeshProUGUI 필드

    [SerializeField]
    private Button exitButton; // 일시정지 패널의 EXIT 버튼

    private AudioSource audioSource;
    private GameObject playerInstance;

    // AdsManager 싱글톤 인스턴스 사용
    private AdsManager adsManager;

    private int coin = 0;
    private int bossKillCount = 0;
    private const int bossKillLimit = 4;

    public GameObject hardPanel; // 하드모드에서만 활성화할 패널

    public bool isHardMode = false; // Hard 모드 여부
    private bool isHardModeUnlocked = false; // 하드 모드 개방 여부

    [HideInInspector]
    public bool isGameOver = false;
    private bool isBossDefeated = false;
    private Background background;
    private string saveFilePath;

    public bool isNetworkErrorActive = false; // 네트워크 에러 패널 활성화 여부

    // 엔딩 크레딧 스킵 가능 여부
    private bool canSkipCredits = false;

    [SerializeField]
    private Button hardModeButton; // Hard 모드 버튼

    public enum GameState { Start, Playing, GameOver, GameClear, Paused }
    public GameState gameState = GameState.Start;

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
        public bool isHardMode;  // 추가: 하드 모드인지 여부를 나타내는 필드

        public ScoreEntry(string playerName, int score, bool isHardMode)
        {
            this.playerName = playerName;
            this.score = score;
            this.isHardMode = isHardMode;
        }
    }

    [System.Serializable]
    public class ScoreBoard
    {
        public List<ScoreEntry> scoreEntries = new List<ScoreEntry>();
    }

    private ScoreBoard scoreBoard;

    [SerializeField]
    private GameObject[] bossPrefabs;
    public Boss currentBoss;
    private int currentBossIndex = 0;
    private int playAgainCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        saveFilePath = Path.Combine(Application.persistentDataPath, "scores.json");
        LoadScores();

        isHardModeUnlocked = PlayerPrefs.GetInt("HardModeUnlocked", 0) == 1;

        // AdsManager 인스턴스 가져오기
        adsManager = AdsManager.Instance;

        CheckAndRemoveAdsButtons();

        // 시작 시 저장된 언어 불러오기
        currentLanguage = PlayerPrefs.GetInt("Language", 0); // 기본값 영어(0)
    }

    // 현재 선택된 언어를 반환하는 메서드
    public int GetCurrentLanguage()
    {
        return currentLanguage; // 변수 사용
    }

    // 언어를 업데이트하는 메서드
    public void UpdateLanguage(int languageIndex)
    {
        currentLanguage = languageIndex; // 변수 값 업데이트
        PlayerPrefs.SetInt("Language", languageIndex);
        PlayerPrefs.Save();
        Debug.Log($"Language set to: {languageIndex}");
    }

    void Start()
    {
        StartGame(); // 게임 시작 시 체력 UI 업데이트

        // IAPManager 인스턴스에 버튼을 전달하여 UI 업데이트
        IAPManager.Instance.UpdateUIBasedOnPurchases();

        nameInputField.characterLimit = 15;
        nameInputField.onValueChanged.AddListener(delegate { OnNameInputChange(); });
        background = FindObjectOfType<Background>();
        
        LoadGameData(); // 게임 데이터 로드
        UpdateCoinUI();
        UpdateCandyUI();

        // 버튼 이벤트 연결
        openOptionsButton.onClick.AddListener(OpenOptionsPanel);
        closeOptionsButton.onClick.AddListener(CloseOptionsPanel);

        ShowStartPanel();

        showScoreBoardButton.onClick.AddListener(() => ShowScoreBoard(false));
        closeScoreBoardButton.onClick.AddListener(CloseScoreBoard);

        hardModeButton.gameObject.SetActive(false);
        hardModeButton.onClick.AddListener(StartHardMode);

        Button startButton = startPanel.GetComponentInChildren<Button>(); // Start 버튼을 가져옵니다.
        if (startButton != null)
        {
            startButton.onClick.AddListener(() => StartGame(false)); // 일반 모드로 시작
        }

        if (PlayerPrefs.GetInt("HardModeUnlocked", 0) == 1)
        {
            hardModeButton.gameObject.SetActive(true);
        }

        // 옵션 패널을 비활성화된 상태로 시작
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }

        // 마켓 패널 버튼 이벤트 설정
        openMarketButton.onClick.AddListener(OpenMarketPanel);
        closeMarketButton.onClick.AddListener(CloseMarketPanel);

        // 일시정지 패널의 EXIT 버튼 설정
        exitButton.onClick.AddListener(ExitGame);

        // 첫 점수 제출 시 광고 표시
        playAgainCount = 4;

        // 일시정지 패널의 PlayAgain 버튼은 광고 없이 스타트 화면으로
        pausePlayAgainButton.onClick.AddListener(ReturnToStartFromPause);
    }

    // 옵션 패널 열기
    public void OpenOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true);
        }
    }

    // 옵션 패널 닫기
    public void CloseOptionsPanel()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void OnNoAdsButtonClicked()
    {
        IAPManager.Instance.BuyNoAds();
    }

    public void OnRemoveBannerButtonClicked()
    {
        IAPManager.Instance.BuyRemoveBanner();
    }

    private void OnNameInputChange()
    {
        string playerName = nameInputField.text;
        int charCount = GetCharacterCount(playerName);

        playerName = playerName.Replace("\u0008", "");

        if (charCount > 14)
        {
            while (GetCharacterCount(playerName) > 14)
            {
                playerName = playerName.Remove(playerName.Length - 1);
            }
            nameInputField.text = playerName;
        }
    }

    private string TrimToMaxLength(string input, int maxLength)
    {
        int count = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] >= 0xAC00 && input[i] <= 0xD7A3)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }

            if (count > maxLength)
            {
                return input.Substring(0, i);
            }
        }
        return input;
    }

    private int GetCharacterCount(string input)
    {
        int count = 0;
        foreach (char c in input)
        {
            if (c >= 0xAC00 && c <= 0xD7A3)
            {
                count += 2;
            }
            else
            {
                count += 1;
            }
        }
        return count;
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreBoard, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void LoadScores()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            scoreBoard = JsonUtility.FromJson<ScoreBoard>(json);

            if (scoreBoard.scoreEntries.Count > 10)
            {
                scoreBoard.scoreEntries = scoreBoard.scoreEntries.GetRange(0, 10);
            }
        }
        else
        {
            scoreBoard = new ScoreBoard();
        }
    }

    public void AddScore(string playerName, int score)
    {
        scoreBoard.scoreEntries.Add(new ScoreEntry(playerName, score, isHardMode));
        scoreBoard.scoreEntries.Sort((a, b) => b.score.CompareTo(a.score));

        if (scoreBoard.scoreEntries.Count > 10)
        {
            scoreBoard.scoreEntries = scoreBoard.scoreEntries.GetRange(0, 10);
        }

        SaveScores();
    }

    private void UpdateScoreBoard()
    {
        scoreBoardText.text = string.Empty;

        int count = 0;
        foreach (ScoreEntry entry in scoreBoard.scoreEntries)
        {
            string modeIndicator = entry.isHardMode ? "*" : "";
            scoreBoardText.text += $"{modeIndicator}{entry.playerName}: {entry.score}\n";

            count++;
            if (count >= 10)
            {
                break;
            }
        }
    }

    public void ShowStartPanel()
    {
        gameState = GameState.Start;

        startPanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        gameClearPanel?.SetActive(false);
        scorePanel?.SetActive(false);
        scoreBoardPanel?.SetActive(false);
        pauseButton?.SetActive(false);
        pausePanel?.SetActive(false);
        endingCreditsPanel?.SetActive(false);
        marketPanel?.SetActive(false); // 마켓 패널 비활성화
        healthPanel?.SetActive(false);  // 체력 패널 비활성화        

        // 하드모드 패널 비활성화
        if (hardPanel != null)
        {
            hardPanel.SetActive(false);
        }

        if (playerInstance != null)
        {
            playerInstance.SetActive(false);
        }

        ResetBackground();

        PlayBGM(startBGM, true);
    }

    private void ResetBackground()
    {
        if (background != null)
        {
            background.ResetSprite();
        }
    }

    private void ShowNetworkErrorPanel()
    {
        if (networkErrorPanel != null)
        {
            networkErrorPanel.SetActive(true);
            isNetworkErrorActive = true;
        }
    }

    private void HideNetworkErrorPanel()
    {
        if (networkErrorPanel != null)
        {
            networkErrorPanel.SetActive(false);
            isNetworkErrorActive = false;
        }
    }

    public void StartGame(bool hardMode)
    {
        isHardMode = hardMode; // 하드 모드 여부 설정

        // 시작 패널을 먼저 비활성화
        startPanel?.SetActive(false);

        // 현재 세션 코인 초기화
        coin = 0;
        text.SetText(coin.ToString());

        // 캔디는 누적된 값 유지
        UpdateCandyUI();

        // 게임 시작 로직 전에 튜토리얼 패널을 보여줍니다.
        StartCoroutine(ShowTutorialAndStartGame());
    }

    private IEnumerator ShowTutorialAndStartGame()
    {
        if (PlayerPrefs.GetInt("HasSeenTutorial", 0) == 0)
        {
            tutorialPanel?.SetActive(true);
            Time.timeScale = 0f; // 게임을 일시 중지
            yield return new WaitForSecondsRealtime(5f);
            tutorialPanel?.SetActive(false);
            PlayerPrefs.SetInt("HasSeenTutorial", 1);
            PlayerPrefs.Save();
            Time.timeScale = 1f; // 게임을 다시 정상 속도로 진행
        }

        // 튜토리얼이 끝난 후 게임 시작
        StartGameAfterTutorial();
    }

    private void InitializeGame()
    {
        if (scorePanel != null) scorePanel.SetActive(true);
        if (scoreBoardPanel != null) scoreBoardPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);

        isBossDefeated = false;

        // playerPrefab이 할당되어 있는지 먼저 확인
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab is not assigned in the inspector!");
            return;
        }

        // 플레이어가 이미 생성되었는지 확인
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, new Vector3(0, -3.5f, 0), Quaternion.identity);
        }
        else
        {
            playerInstance.SetActive(true);
        }

        // PlayerHealth 컴포넌트를 가져옴
        playerHealth = playerInstance.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;

            int healthPlus1Purchased = PlayerPrefs.GetInt("HealthPlus1Purchased", 0);
            int healthPlus2Purchased = PlayerPrefs.GetInt("HealthPlus2Purchased", 0);
            int totalHealthIncreases = 0;
            if (healthPlus1Purchased == 1) totalHealthIncreases += 1;
            if (healthPlus2Purchased == 1) totalHealthIncreases += 1;

            playerHealth.SetMaxHealth(2 + totalHealthIncreases);
            playerHealth.ResetHealth();
            UpdateHealthUI();
        }
        else
        {
            Debug.LogError("PlayerHealth component is missing in playerInstance.");
            return;
        }

        var player = playerInstance.GetComponent<Player>();
        if (player != null)
        {
            player.ResetWeapon();
        }
        else
        {
            Debug.LogError("Player component is missing in playerInstance.");
            return;
        }

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.ClearEnemies();
            enemySpawner.isHardMode = isHardMode; // 하드 모드 여부 전달
            enemySpawner.StartEnemyRoutine();
        }
        else
        {
            Debug.LogError("EnemySpawner not found in the scene.");
        }

        // 게임 시작 시 배너 광고 표시
        if (PlayerPrefs.GetInt("RemoveBanner", 0) == 0)
        {
            adsManager.ShowBannerAd();
        }
    }

    public void UpdateHealthUI()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component is missing.");
            return;
        }

        // 플레이어의 현재 체력과 최대 체력에 따라 UI 업데이트
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                healthImages[i].sprite = fullHeartSprite; // 체력이 있으면 채워진 하트
            }
            else
            {
                healthImages[i].sprite = emptyHeartSprite; // 체력이 없으면 빈 하트
            }

            // 현재 최대 체력보다 많은 하트는 비활성화
            healthImages[i].enabled = (i < playerHealth.maxHealth);
        }
    }

    public void StartGameAfterTutorial()
    {
        PlayBGM(startBGM, false);
        PlayBGM(gameBGM, true);

        startPanel?.SetActive(false);
        healthPanel?.SetActive(true);  // 게임 시작 시 체력 패널 활성화

        InitializeGame(); // 공통 초기화 호출
    }

    public void StartGame()
    {
        // 네트워크 연결 상태 확인
        if (!NetworkChecker.IsOnline() && PlayerPrefs.GetInt("NoAds", 0) == 0 && PlayerPrefs.GetInt("RemoveBanner", 0) == 0)
        {
            ShowNetworkErrorPanel(); // 예시: 네트워크 연결 오류 패널을 표시하는 메서드
            return;
        }

        PlayBGM(startBGM, false);
        PlayBGM(gameBGM, true);

        startPanel?.SetActive(false);
        healthPanel?.SetActive(true);  // 게임 시작 시 체력 패널 활성화       

        // 기타 초기화 로직
        StartCoroutine(ShowTutorialAndStartGame());

        InitializeGame(); // 공통 초기화 호출

        StartCoroutine(StartGameWithLoading());
    }

    private IEnumerator StartGameWithLoading()
    {
        gameState = GameState.Playing;
        startPanel?.SetActive(false);
        scorePanel?.SetActive(true);
        scoreBoardPanel?.SetActive(false);
        pauseButton?.SetActive(true);
        pausePanel?.SetActive(false);

        isBossDefeated = false;
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, new Vector3(0, -3.5f, 0), Quaternion.identity);
        }
        else
        {
            playerInstance.SetActive(true);
            playerInstance.GetComponent<Player>().ResetWeapon();
        }
        ResetGameManagerState();

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.ClearEnemies();
            enemySpawner.isHardMode = isHardMode; // Hard 모드 설정 전달
            enemySpawner.StartEnemyRoutine();
        }

        PlayBGM(gameBGM, true);

        yield return new WaitForSeconds(1f);
    }

    public void IncreaseCoin()
    {
        if (gameState != GameState.Playing) return;

        // 세션 코인 증가
        coin++;
        text.SetText(coin.ToString()); // 세션 코인 UI 업데이트

        // 업그레이드 조건 확인
        int coinsNeededForUpgrade = isHardMode ? 40 : 20; // 하드모드에서는 40개, 일반 모드에서는 20개
        if (coin % coinsNeededForUpgrade == 0)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.Upgrade();
            }
        }        

        // UI 갱신 (현재 세션 코인만)
        UpdateCoinUI();
    }    

    public void IncreaseCandy()
    {
        totalCandies++; // 총 캔디 수 증가
        PlayerPrefs.SetInt("TotalCandies", totalCandies); // 저장
        PlayerPrefs.Save(); // 강제 저장
        UpdateCandyUI(); // UI 업데이트
    }
        
    private void LoadGameData()
    {        
        totalCandies = PlayerPrefs.GetInt("TotalCandies", 0); // 총 캔디 로드

        UpdateCandyUI();
    }

    private void UpdateCandyUI()
    {
        if (candyText != null)
        {
            candyText.text = $"{totalCandies}";
        }
        else
        {
            Debug.LogWarning("CandyText is not assigned in the Inspector!");
        }
    }
    

    private void UpdateCoinUI()
    {
        coinText.text = coin.ToString();
    }

    public void BossDefeated()
    {
        if (gameState != GameState.Playing) return;

        isBossDefeated = true;
        if (currentBoss != null)
        {
            Destroy(currentBoss.gameObject);
            currentBoss = null;
        }

        bossKillCount += 1;

        if (background != null)
        {
            background.ChangeBackgroundSprite();
        }

        if (bossKillCount >= bossKillLimit)
        {
            SetGameClear();
        }
    }

    public void SetGameOver()
    {
        if (gameState != GameState.Playing) return;

        gameState = GameState.GameOver;
        isGameOver = true;
        scorePanel?.SetActive(false);
        finalScoreText?.SetText(" " + coin);
        pauseButton?.SetActive(false);
        healthPanel?.SetActive(false);

        if (playerInstance != null)
        {
            playerInstance.SetActive(false); // 플레이어 비활성화
        }

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.StopEnemyRoutine();
        }

        gameOverPanel?.SetActive(true);
        PlayBGM(gameOverBGM, false);

        DestroyAllBossBullets();
    }

    public void SetGameClear()
    {
        if (gameState != GameState.Playing) return;

        gameState = GameState.GameClear;
        isGameOver = true;
        scorePanel?.SetActive(false);
        clearScoreText?.SetText(" " + coin);
        pauseButton?.SetActive(false);
        healthPanel?.SetActive(false);

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.StopEnemyRoutine();
        }

        StartCoroutine(ShowGameClearPanel());
        PlayBGM(gameClearBGM, false);
        DestroyAllBossBullets();

        // 하드모드 개방
        UnlockHardMode();

        hardModeButton.gameObject.SetActive(true);

        // 하드모드 해제 상태를 PlayerPrefs에 저장
        PlayerPrefs.SetInt("HardModeUnlocked", 1);
        PlayerPrefs.Save(); // 저장 강제 실행
    }

    private void UnlockHardMode()
    {
        isHardModeUnlocked = true;
    }

    private void StartHardMode()
    {
        StartGame(true); // 하드 모드로 게임 시작

        if (hardPanel != null)
        {
            hardPanel.SetActive(true);
        }
        StartCoroutine(StartGameWithLoading());
    }

    private IEnumerator ShowGameClearPanel()
    {
        gameClearPanel?.SetActive(true);
        yield return new WaitForSeconds(3f);
        gameClearPanel?.SetActive(false);
        ShowScoreBoard(true);
    }

    public void ShowScoreBoard(bool isFromGameOver)
    {
        gameOverPanel?.SetActive(false);
        gameClearPanel?.SetActive(false);
        scoreBoardPanel?.SetActive(true);

        string modeText = isHardMode ? "Hard Mode" : "Normal Mode";
        scoreBoardText.text = $"{modeText} Scores:\n";

        if (isFromGameOver)
        {
            nameInputField?.gameObject.SetActive(true);
            submitButton?.gameObject.SetActive(true);
            playAgainButton?.gameObject.SetActive(false); // 게임 오버일 때 Play Again 버튼 표시
            closeScoreBoardButton?.gameObject.SetActive(false);
        }
        else
        {
            nameInputField?.gameObject.SetActive(false);
            submitButton?.gameObject.SetActive(false);
            playAgainButton?.gameObject.SetActive(false);
            closeScoreBoardButton?.gameObject.SetActive(true);
        }

        UpdateScoreBoard();
    }

    private void CloseScoreBoard()
    {
        scoreBoardPanel?.SetActive(false);
    }

    private bool isFirstScoreSubmitted = false; // 점수 첫 제출 여부

    public void SubmitScore()
    {
        string playerName = nameInputField?.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            int charCount = GetCharacterCount(playerName);
            if (charCount > 16)
            {
                Debug.Log("플레이어 이름이 너무 깁니다! 한글은 최대 8자, 영어는 최대 16자까지 가능합니다.");
                return;
            }

            AddScore(playerName, coin);

            // 전면 광고 표시
            if (PlayerPrefs.GetInt("NoAds", 0) == 0)
            {
                adsManager.ShowInterstitialAd();
            }

            nameInputField?.gameObject.SetActive(false);
            submitButton?.gameObject.SetActive(false);
            UpdateScoreBoard();

            if (gameState == GameState.GameClear)
            {
                StartCoroutine(ShowEndingCreditsWithDelay());
            }
            else
            {
                playAgainButton?.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("이름을 입력하세요.");
        }
    }

    private IEnumerator ShowAdAndContinue()
    {
        if (adsManager != null && PlayerPrefs.GetInt("NoAds", 0) == 0)
        {
            adsManager.ShowInterstitialAd();
            while (adsManager.IsInterstitialAdShowing)
            {
                yield return null;
            }
        }
        if (bossKillCount >= bossKillLimit && gameState == GameState.GameClear)
        {
            ShowEndingCredits();
        }
    }

    private IEnumerator ShowEndingCreditsWithDelay()
    {
        yield return new WaitForSeconds(5f);
        ShowEndingCredits();
    }

    public void ShowEndingCredits()
    {
        Debug.Log("ShowEndingCredits called");

        if (endingCreditsPanel != null)
        {
            endingCreditsPanel.SetActive(true);
        }

        if (scoreBoardPanel != null)
        {
            scoreBoardPanel.SetActive(false);
        }

        if (endingCreditsVideoPlayer != null)
        {
            endingCreditsVideoPlayer.Play();
            endingCreditsVideoPlayer.loopPointReached += OnEndingCreditsFinished;

            // 처음 엔딩 크레딧이 실행될 때는 스킵이 불가능하게 설정
            if (PlayerPrefs.GetInt("HasSeenCredits", 0) == 0)
            {
                canSkipCredits = false;
                PlayerPrefs.SetInt("HasSeenCredits", 1); // 처음 본 후에 플래그 설정
            }
            else
            {
                canSkipCredits = true; // 이후로는 스킵 가능
            }
        }
    }

    void Update()
    {
        // 엔딩 크레딧 스킵 처리
        if (canSkipCredits && gameState == GameState.GameClear && endingCreditsPanel.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            OnEndingCreditsFinished(endingCreditsVideoPlayer);
        }

        // 네트워크 오류 시 게임 조작 차단
        if (isNetworkErrorActive && Input.touchCount > 0)
        {
            Input.ResetInputAxes();
        }
    }

    private void OnEndingCreditsFinished(VideoPlayer vp)
    {
        Debug.Log("Ending video finished");
        if (endingCreditsPanel != null)
        {
            endingCreditsPanel.SetActive(false);
        }
        ShowStartPanel();
    }

    public void PlayAgain()
    {
        playAgainCount++;

        // 5번마다 전면 광고 표시
        if (playAgainCount >= 5)
        {
            if (PlayerPrefs.GetInt("NoAds", 0) == 0)
            {
                adsManager.ShowInterstitialAd();
            }
            playAgainCount = 0; // 카운트 초기화
        }

        if (playerInstance != null)
        {
            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth(); // 체력 초기화
            }

            playerInstance.SetActive(true); // 플레이어를 다시 활성화
        }

        // 게임 상태 초기화 및 리셋
        ShowStartPanel();
        ResetGameManagerState();
    }

    private void ResetGameManagerState()
    {
        gameState = GameState.Playing;
        isGameOver = false;
        isBossDefeated = false;
        coin = 0;
        bossKillCount = 0;
        currentBossIndex = 0;

        ResetBackground();
        ResetScore();

        gameOverPanel?.SetActive(false);
        gameClearPanel?.SetActive(false);

        if (currentBoss != null && currentBoss.gameObject != null && currentBoss.gameObject.scene.IsValid())
        {
            Destroy(currentBoss.gameObject);
            currentBoss = null;
        }

        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.ClearEnemies();
            enemySpawner.ResetEnemySpawnerState();
            enemySpawner.StopEnemyRoutine();
        }
    }

    private void ResetScore()
    {
        coin = 0;
        text.SetText(coin.ToString());
    }

    private void PlayBGM(AudioClip clip, bool loop)
    {
        audioSource.Stop();
        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SetBGMVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SpawnNextBoss()
    {
        if (currentBossIndex < bossPrefabs.Length)
        {
            GameObject bossObject = Instantiate(bossPrefabs[currentBossIndex], transform.position, Quaternion.identity);
            Boss boss = bossObject.GetComponent<Boss>();
            if (boss != null)
            {
                boss.player = playerInstance.transform;
                boss.bulletPrefab = bulletPrefab;

                Transform firePoint = bossObject.transform.Find("FirePoint");
                if (firePoint != null)
                {
                    boss.firePoint = firePoint;
                }
                else
                {
                    Debug.LogWarning("FirePoint not found in Boss prefab");
                }

                if (boss.bulletPrefab == null || boss.firePoint == null)
                {
                    Debug.LogError("BulletPrefab or FirePoint is not set correctly for the boss.");
                }
            }
            else
            {
                Debug.LogWarning("Boss component not found on bossObject");
            }
            currentBoss = boss;
            currentBossIndex++;
        }
        else
        {
            Debug.Log("All bosses have been defeated!");
        }
    }

    private void DestroyAllBossBullets()
    {
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            if (bullet.isBossBullet)
            {
                Destroy(bullet.gameObject);
            }
        }
    }

    public bool IsBossDefeated()
    {
        return isBossDefeated;
    }

    public void SetBossDefeated(bool value)
    {
        isBossDefeated = value;
    }

    public void PauseGame()
    {
        if (gameState == GameState.Playing)
        {
            gameState = GameState.Paused;
            pausePanel?.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (gameState == GameState.Paused)
        {
            gameState = GameState.Playing;
            pausePanel?.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ReturnToStartFromPause()
    {
        if (playerInstance != null)
        {
            PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth(); // 체력 초기화
            }

            playerInstance.SetActive(true); // 플레이어를 다시 활성화
            Time.timeScale = 1f;
        }

        // 게임 상태 초기화 및 리셋
        pausePanel?.SetActive(false);
        ShowStartPanel();
        ResetGameManagerState();
    }

    private void CheckAndRemoveAdsButtons()
    {
        // 광고 제거 여부 확인하여 버튼 비활성화 및 제거
        if (PlayerPrefs.GetInt("NoAds", 0) == 1)
        {
            removeBannerAdsButton?.gameObject.SetActive(false);
            removeInterstitialAdsButton?.gameObject.SetActive(false);
        }
        else
        {
            removeBannerAdsButton?.gameObject.SetActive(true);
            removeInterstitialAdsButton?.gameObject.SetActive(true);
        }
    }

    public void UpdateUIBasedOnPurchases()
    {
        if (PlayerPrefs.GetInt("NoAds", 0) == 1)
        {
            removeInterstitialAdsButton.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("RemoveBanner", 0) == 1)
        {
            removeBannerAdsButton.gameObject.SetActive(false);
            adsManager.HideBannerAd(); // 배너 광고 제거
        }

        if (PlayerPrefs.GetInt("HealthPlus1Purchased", 0) == 1)
        {
            healthPlus1Button.gameObject.SetActive(false); // 이미 구매된 경우 버튼 비활성화
        }

        if (PlayerPrefs.GetInt("HealthPlus2Purchased", 0) == 1)
        {
            healthPlus2Button.gameObject.SetActive(false);
        }
    }

    private void DisableNoAdsButton()
    {
        if (removeInterstitialAdsButton != null)
        {
            removeInterstitialAdsButton.gameObject.SetActive(false); // 전면 광고 제거 버튼 비활성화
        }
        Debug.Log("No Ads button disabled.");
    }

    private void DisableRemoveBannerButton()
    {
        if (removeBannerAdsButton != null)
        {
            removeBannerAdsButton.gameObject.SetActive(false); // 배너 광고 제거 버튼 비활성화
        }
        Debug.Log("Remove Banner button disabled.");
    }

    private void DisablehealthPlus1Button()
    {
        if (healthPlus1Button != null)
        {
            healthPlus1Button.gameObject.SetActive(false); // 배너 광고 제거 버튼 비활성화
        }
        Debug.Log("Health Plus1 Button disabled.");
    }

    private void DisablehealthPlus2Button()
    {
        if (healthPlus2Button != null)
        {
            healthPlus2Button.gameObject.SetActive(false); // 배너 광고 제거 버튼 비활성화
        }
        Debug.Log("Health Plus2 Button disabled.");
    }

    private void DisableButtonsAfterPurchase()
    {
        if (PlayerPrefs.GetInt("NoAds", 0) == 1)
        {
            removeInterstitialAdsButton.gameObject.SetActive(false);
        }
        else
        {
            removeInterstitialAdsButton.interactable = true;
        }

        if (PlayerPrefs.GetInt("RemoveBanner", 0) == 1)
        {
            removeBannerAdsButton.gameObject.SetActive(false);
        }
        else
        {
            removeBannerAdsButton.interactable = true;
        }

        if (PlayerPrefs.GetInt("HealthPlus1Purchased", 0) == 1)
        {
            healthPlus1Button.gameObject.SetActive(false);
        }
        else
        {
            healthPlus1Button.interactable = true;
        }

        if (PlayerPrefs.GetInt("HealthPlus2Purchased", 0) == 1)
        {
            healthPlus2Button.gameObject.SetActive(false);
        }
        else
        {
            healthPlus2Button.interactable = true;
        }
    }

    public void OpenMarketPanel()
    {
        marketPanel.SetActive(true);
    }

    public void CloseMarketPanel()
    {
        marketPanel.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);

            // 메시지를 3초 후에 자동으로 숨기기
            Invoke("HideMessage", 3f);
        }
        else
        {
            Debug.LogWarning("Message Text UI is not assigned.");
        }
    }

    private void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
