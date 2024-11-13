using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    #region FIELDS

    public int waveNumber = 0;
    public int enemiesLeft = 0;
    public int money = 0;
    public GameObject MenuPnl, MenuBtnPnl, TimeControlPanel, EnemyPnl, EndPnl, tutPnl, UpgradeBtn, SubUpgrade, SubSubUpgrade, SubUp1, SubUp2, SellBtn, BuyGolemBtn, BuyMusketBtn, BuyCannonBtn;
    public CanvasGroup canvasGroup;
    public float fadeTime = 3.0f;
    private bool isFading = false;
    public UnityEngine.UI.Image TimeImage;
    public Sprite PauseSpt, PlaySpt, FastSpt;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI waveNumberText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI EndText;
    public TextMeshProUGUI upgradeCost, SellCost;
    public Defender occupyingUnit;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        TimeImage = TimeControlPanel.GetComponent<UnityEngine.UI.Image>();
    }

    private void OnEnable()
    {
        EventManager.OnGamePaused += OnGamePaused;
        EventManager.OnGameResumed += OnGameResumed;
        EventManager.OnGameStarted += OnGameStarted;
        EventManager.OnGameEnded += OnGameEnded;
        EventManager.OnWaveCompleted += OnWaveCompleted;
        EventManager.SetMoneyUI += SetMoney;
        EventManager.SetEnemyUI += SetEnemyUI;
        //EventManager.PlayBtnClicked += PlayBtnClicked;
        EventManager.PauseBtnClicked += PauseBtnClicked;
        EventManager.FastBtnClicked += FastBtnClicked;
        EventManager.OnTileDeselect += OnTileDeselect;
        EventManager.OnTileSelected += OnTileClicked;
        EventManager.SetWaveUI += UpdateWaveNumber;
    }

    private void OnDisable()
    {
        EventManager.OnGamePaused -= OnGamePaused;
        EventManager.OnGameResumed -= OnGameResumed;
        EventManager.OnGameStarted -= OnGameStarted;
        EventManager.OnGameEnded -= OnGameEnded;
        EventManager.OnWaveCompleted -= OnWaveCompleted;
        EventManager.SetMoneyUI -= SetMoney;
        EventManager.SetEnemyUI -= SetEnemyUI;
        //EventManager.PlayBtnClicked -= PlayBtnClicked;
        EventManager.PauseBtnClicked -= PauseBtnClicked;
        EventManager.FastBtnClicked -= FastBtnClicked;
        EventManager.OnTileDeselect -= OnTileDeselect;
        EventManager.OnTileSelected -= OnTileClicked;
        EventManager.SetWaveUI -= UpdateWaveNumber;
    }

    private void Start()
    {
        TimeControlPanel.SetActive(true);
        MenuBtnPnl.SetActive(false);
        EndPnl.SetActive(false);
        tutPnl.SetActive(true);
        StartCoroutine(TutorialPnlWaitTimer());
    }

    private void Update()
    {
        if (isFading && canvasGroup.alpha >= 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;
            if (canvasGroup.alpha == 0)
            {
                tutPnl.SetActive(false);
                NarrativeManager.Instance.SartDialog();
                //Debug.Log("Tutorial Panel Faded");
                isFading = false;
            }
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void onClickForNextDialog()
    {
    }

    public void UpdateWaveNumber(int waveNumber)
    {
        waveNumberText.text = "Wave: " + waveNumber.ToString();
    }

    private void OnGamePaused()
    {
        MenuBtnPnl.SetActive(false);
    }

    private void SetEnemyUI(int enemiesLeft)
    {
        enemyCountText.text = "Enemies: " + enemiesLeft.ToString();
    }

    private void OnGameResumed()
    {
        MenuBtnPnl.SetActive(false);
    }

    private void OnGameStarted()
    {
        MenuBtnPnl.SetActive(false);
        SubUpgrade.SetActive(false);
        SubSubUpgrade.SetActive(false);
    }

    private void OnGameEnded()
    {
        MenuBtnPnl.SetActive(false);
        TimeControlPanel.SetActive(false);
        MenuPnl.SetActive(false);
        EnemyPnl.SetActive(false);
        GameManager.gameState = GameState.End;
        SetDeltaTimeSpeedPause();
        EndPnl.SetActive(true);
        string endTxt = "Game Over\n";
        endTxt += "You Survived Wave: " + EnemyManager.WaveCount + "\n";
        endTxt += "You Killed " + EnemyManager.EnemiesKilled + " Enemies\n";
        EndText.text = endTxt;
        StartCoroutine(WaitFor(5));
        SceneManager.LoadScene(0);
    }

    public void OnBuyDefenderBtnClicked(int DefenderType)
    {
        EventManager.BuyDefender?.Invoke(PlayerManager.selectedTile, DefenderType);
    }

    public void OnSellDefenderBtnClicked()
    {
        EventManager.SellDefender?.Invoke(PlayerManager.selectedTile);
    }

    public void OnTowerUpgrade()
    {
        SubSubUpgrade.SetActive(true);
        SubUpgrade.SetActive(false);
        UpgradeBtn.SetActive(false);
        SellBtn.SetActive(false);
    }

    public void OnUpgradeDefBtnClicked()
    {
        if (!occupyingUnit.Upgraded)
        {
            SubUpgrade.SetActive(true);
            UpgradeBtn.SetActive(false);
            SellBtn.SetActive(false);
            SubUp1.GetComponent<Image>().sprite = occupyingUnit.IconUpgrade1;
            //SubUp1.GetComponent<Button>().onClick.AddListener(() => OnUpgradeDefenderBtnClicked(1));
            SubUp2.GetComponent<Image>().sprite = occupyingUnit.IconUpgrade2;
            //SubUp2.GetComponent<Button>().onClick.AddListener(() => OnUpgradeDefenderBtnClicked(2));
        }
    }

    public void OnUpgradeTower(int num)
    {
        EventManager.UpgradeDefender?.Invoke(PlayerManager.selectedTile, num);
        SubUpgrade.SetActive(false);
        SubSubUpgrade.SetActive(false);
    }

    public void OnUpgradeDefenderBtnClicked(int num)
    {
        EventManager.UpgradeDefender?.Invoke(PlayerManager.selectedTile, num);
        SubUpgrade.SetActive(false);
    }

    private void OnWaveCompleted()
    {
        MenuBtnPnl.SetActive(false);
        SetDeltaTimeSpeedNormal();
        TimeImage.sprite = PauseSpt;
        GameManager.gameState = GameState.BuildingPhase;
    }

    private void SetMoney(int money)
    {
        moneyText.text = "$: " + money.ToString();
    }

    private void OnTileClicked(BuildingTile tile)
    {
        MenuBtnPnl.SetActive(true);
        MenuPnl.SetActive(true);
        if (tile.properties.OccupyingUnit != null)
        {
            UpgradeBtn.SetActive(true);
            occupyingUnit = tile.properties.OccupyingUnit;
            upgradeCost.text = occupyingUnit.UpgradeCost.ToString();
            SellBtn.SetActive(true);
            SellCost.text = (occupyingUnit.cost / 2).ToString();
            BuyGolemBtn.SetActive(false);
            BuyMusketBtn.SetActive(false);
            BuyCannonBtn.SetActive(false);
        }
        else
        {
            UpgradeBtn.SetActive(false);
            occupyingUnit = null;
            SellBtn.SetActive(false);
            BuyGolemBtn.SetActive(true);
            BuyMusketBtn.SetActive(true);
            BuyCannonBtn.SetActive(true);
        }
    }

    private void OnTileDeselect()
    {
        MenuBtnPnl.SetActive(false);
        MenuPnl.SetActive(false);
        SubUpgrade.SetActive(false);
    }

    public void PlayBtnClicked()
    {
        //Debug.Log("Play Button Clicked");
        if (GameManager.gameState == GameState.Pause)
        {
            TimeControlPanel.GetComponent<UnityEngine.UI.Image>().sprite = PlaySpt;
            EventManager.OnGameResumed?.Invoke();
            SetDeltaTimeSpeedNormal();
            GameManager.gameState = GameState.Play;
        }
        if (GameManager.gameState == GameState.BuildingPhase)
        {
            TimeControlPanel.GetComponent<UnityEngine.UI.Image>().sprite = PlaySpt;
            EventManager.OnWaveStart?.Invoke();
            SetDeltaTimeSpeedNormal();
            GameManager.gameState = GameState.Play;
        }
        if (GameManager.gameState == GameState.Fast)
        {
            TimeControlPanel.GetComponent<UnityEngine.UI.Image>().sprite = PlaySpt;
            SetDeltaTimeSpeedNormal();
            GameManager.gameState = GameState.Play;
        }
    }

    private void SetDeltaTimeSpeedFast()
    {
        Time.timeScale = 2;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void SetDeltaTimeSpeedNormal()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    private void SetDeltaTimeSpeedPause()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public void PauseBtnClicked()
    {
        //Debug.Log("Pause Button Clicked");
        if (GameManager.gameState == GameState.Play)
        {
            TimeImage.sprite = PauseSpt;
            SetDeltaTimeSpeedPause();
            GameManager.gameState = GameState.Pause;
        }

        if (GameManager.gameState == GameState.Fast)
        {
            TimeImage.sprite = PauseSpt;
            SetDeltaTimeSpeedPause();
            GameManager.gameState = GameState.Pause;
        }
    }

    public void FastBtnClicked()
    {
        //Debug.Log("Fast Button Clicked");
        if (GameManager.gameState == GameState.Pause)
        {
            TimeImage.sprite = FastSpt;
            EventManager.OnGameResumed?.Invoke();
            SetDeltaTimeSpeedFast();
            GameManager.gameState = GameState.Fast;
        }
        if (GameManager.gameState == GameState.BuildingPhase)
        {
            TimeImage.sprite = FastSpt;
            EventManager.OnWaveStart?.Invoke();
            SetDeltaTimeSpeedFast();
            GameManager.gameState = GameState.Fast;
        }
        if (GameManager.gameState == GameState.Play)
        {
            TimeImage.sprite = FastSpt;
            SetDeltaTimeSpeedFast();
            GameManager.gameState = GameState.Fast;
        }
    }

    public IEnumerator WaitFor(float Secs)
    {
        yield return new WaitForSeconds(Secs);
    }

    public IEnumerator TutorialPnlWaitTimer()
    {
        yield return new WaitForSeconds(1.5f);
        isFading = true;
    }

    #endregion METHODS
}