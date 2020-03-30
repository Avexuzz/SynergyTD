// SINGLETON-BASED UI CONTROLLER
// EVERY UI REQUEST IS PROCEDURED HERE TO CONTROL UI OVERLAPS AND STUFF
// USE MESSENGER TO REQUEST UI DRAWINGS


using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private GameObject towerBuildCanvas;
    private GameObject towerStatsCanvas;
    private GameObject gameStateCanvas;
    private GameObject loseCanvas;
    private GameObject pauseMenuCanvas;
    private GameObject pauseCollider;
    [SerializeField] private GameObject buyElementPrefab;
    private TowerStats towerStats;
    private GameObject contentField;
    private GameObject buyElement;
    private GameObject waveTimerText;
    private GameObject startNextWaveButton;
    private GameObject coinCountText;
    private GameObject lifeCountText;

    void Awake()
    {
        Messenger<GameObject>.AddListener(GameEvent.BUILD_UI_REQUEST, OnBuildRequest);
        Messenger<GameObject>.AddListener(GameEvent.STATS_UI_REQUEST, OnStatsRequest);
        Messenger.AddListener(GameEvent.BUILD_TOWER_AFTER, OnBuildTowerAfter);
        Messenger<int>.AddListener(GameEvent.UI_WAVE_TIMER_CHANGED, OnWaveTimerChanged);
        Messenger.AddListener(GameEvent.START_WAVE, OnStartWave);
        Messenger.AddListener(GameEvent.FINISH_WAVE, OnFinishWave);
        Messenger<int>.AddListener(GameEvent.COINS_CHANGED, OnCoinsChanged);
        Messenger<int>.AddListener(GameEvent.LIFES_CHANGED, OnLifesChanged);
        Messenger.AddListener(GameEvent.GAME_LOST, OnGameLost);
    }
    void OnDestroy()
    {
        Messenger<GameObject>.RemoveListener(GameEvent.BUILD_UI_REQUEST, OnBuildRequest);
        Messenger<GameObject>.RemoveListener(GameEvent.STATS_UI_REQUEST, OnStatsRequest);
        Messenger.RemoveListener(GameEvent.BUILD_TOWER_AFTER, OnBuildTowerAfter);
        Messenger<int>.RemoveListener(GameEvent.UI_WAVE_TIMER_CHANGED, OnWaveTimerChanged);
        Messenger.RemoveListener(GameEvent.START_WAVE, OnStartWave);
        Messenger.RemoveListener(GameEvent.FINISH_WAVE, OnFinishWave);
        Messenger<int>.RemoveListener(GameEvent.COINS_CHANGED, OnCoinsChanged);
        Messenger<int>.RemoveListener(GameEvent.LIFES_CHANGED, OnLifesChanged);
        Messenger.RemoveListener(GameEvent.GAME_LOST, OnGameLost);
    }

    // Start is called before the first frame update
    void Start()//caching that bunch of UI crap
    {
        gameStateCanvas = GameObject.Find("GameStateCanvas");
        coinCountText = gameStateCanvas.transform.Find("CoinCountText").gameObject;
        lifeCountText = gameStateCanvas.transform.Find("LifeCountText").gameObject;
        waveTimerText = gameStateCanvas.transform.Find("UntilNextWaveText").gameObject;
        towerStats = GameObject.Find("TowerStats").gameObject.GetComponent<TowerStats>();
        startNextWaveButton = gameStateCanvas.transform.Find("StartNextWaveButton").gameObject;
        towerBuildCanvas = GameObject.Find("TowerBuildCanvas");
        towerStatsCanvas = GameObject.Find("TowerStatsCanvas");
        loseCanvas = GameObject.Find("LoseCanvas");
        pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
        pauseCollider = GameObject.Find("PauseCollider").gameObject;
        contentField = towerBuildCanvas.transform.Find("TowerScrollView").Find("Viewport").Find("Content").gameObject;
        towerBuildCanvas.SetActive(false);
        towerStatsCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        pauseCollider.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        coinCountText.GetComponent<Text>().text = GameObject.Find("SceneController").GetComponent<SceneController>().coins.ToString();
        lifeCountText.GetComponent<Text>().text = GameObject.Find("SceneController").GetComponent<SceneController>().lifes.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBuildRequest(GameObject tile)
    {
        towerStatsCanvas.SetActive(false);//clear other excessive interface
        ClearBuildContent();
        towerBuildCanvas.SetActive(true);
        foreach (TowerStatsStorage tss in towerStats.towersBase){//get list of towers and show it to player
            if(tss.type == int.MaxValue)
            {
                continue;
            }

            buyElement = Instantiate(buyElementPrefab);
            buyElement.transform.SetParent(contentField.transform, false);

            buyElement.transform.Find("BuyButton").gameObject.GetComponent<BuyButton>().type = tss.type;
            buyElement.transform.Find("BuyButton").gameObject.GetComponent<BuyButton>().tile = tile;

            float bonus = tss.bonus * 100 - 100;
            float range = tss.range / 0.64f;
            buyElement.transform.Find("DisplayNameText").gameObject.GetComponent<Text>().text = tss.displayName;
            buyElement.transform.Find("Image").gameObject.GetComponent<Image>().sprite = tss.sprite;
            buyElement.transform.Find("DamageText").gameObject.GetComponent<Text>().text = "Урон: " + tss.damage;
            buyElement.transform.Find("RangeText").gameObject.GetComponent<Text>().text = "Дальность: " + range;
            buyElement.transform.Find("ASText").gameObject.GetComponent<Text>().text = "Скорость: " + tss.AS;
            buyElement.transform.Find("CostText").gameObject.GetComponent<Text>().text = "Стоимость: " + tss.cost;
            buyElement.transform.Find("BonusText").gameObject.GetComponent<Text>().text = "Бонус: " + bonus + "%";
            buyElement.transform.Find("AbilityText").gameObject.GetComponent<Text>().text = tss.ability;
        }
        
    }

    private void OnStatsRequest(GameObject tower)//show tower stats and upgrade button to player
    {
        towerBuildCanvas.SetActive(false);//clear other excessive interface
        towerStatsCanvas.SetActive(true);
        float bonus = tower.GetComponent<TowerAI>().bonus * 100 - 100;//recalculate to player-friendly stats
        float range = tower.GetComponent<TowerAI>().range / 0.64f;
        towerStatsCanvas.transform.Find("Panel").Find("UpgradeButton").gameObject.GetComponent<UpgradeButton>().tower = tower;
        towerStatsCanvas.transform.Find("Panel").Find("DisplayNameText").gameObject.GetComponent<Text>().text = tower.GetComponent<TowerAI>().displayName;
        towerStatsCanvas.transform.Find("Panel").Find("Image").gameObject.GetComponent<Image>().sprite = tower.GetComponent<SpriteRenderer>().sprite;
        towerStatsCanvas.transform.Find("Panel").Find("DamageText").gameObject.GetComponent<Text>().text = "Урон: " + System.Convert.ToInt32(System.Math.Round(tower.GetComponent<TowerAI>().damage, System.MidpointRounding.AwayFromZero)).ToString();
        towerStatsCanvas.transform.Find("Panel").Find("RangeText").gameObject.GetComponent<Text>().text = "Дальность: " + range;
        towerStatsCanvas.transform.Find("Panel").Find("ASText").gameObject.GetComponent<Text>().text = "Скорость: " + tower.GetComponent<TowerAI>().AS;
        towerStatsCanvas.transform.Find("Panel").Find("CostText").gameObject.GetComponent<Text>().text = "Цена: " + tower.GetComponent<TowerAI>().cost;
        towerStatsCanvas.transform.Find("Panel").Find("BonusText").gameObject.GetComponent<Text>().text = "Бонус: " + System.Convert.ToInt32(System.Math.Round(bonus, System.MidpointRounding.AwayFromZero)).ToString() + "%";
        towerStatsCanvas.transform.Find("Panel").Find("AbilityText").gameObject.GetComponent<Text>().text = tower.GetComponent<TowerAI>().ability;
        towerStatsCanvas.transform.Find("Panel").Find("FullDamageText").gameObject.GetComponent<Text>().text = "Полный урон: " + System.Convert.ToInt32(System.Math.Round(tower.GetComponent<TowerAI>().fullDamage, System.MidpointRounding.AwayFromZero)).ToString();
        towerStatsCanvas.transform.Find("Panel").Find("FullASText").gameObject.GetComponent<Text>().text = "Полная скорость: " + System.Convert.ToInt32(System.Math.Round(tower.GetComponent<TowerAI>().fullAS, System.MidpointRounding.AwayFromZero)).ToString(); 
    }

    private void ClearBuildContent()//not really needed now, but may be useful later 
    {
        int n = contentField.transform.childCount;
        for (int i = 0; i < n; i++)
        {
            Destroy(contentField.transform.GetChild(i).gameObject);
        }
    }

    private void OnBuildTowerAfter()//clear build interface after build
    {
        ClearBuildContent();
        towerBuildCanvas.SetActive(false);
    }

    public void OnBuildClose()
    {
        towerBuildCanvas.SetActive(false);
    }

    public void OnStatsClose()
    {
        towerStatsCanvas.SetActive(false);
        Messenger.Broadcast(GameEvent.TOWER_UNSELECTED);
    }

    private void OnWaveTimerChanged(int t)
    {
        waveTimerText.GetComponent<Text>().text = t.ToString();
    }

    private void OnStartWave()
    {
        startNextWaveButton.SetActive(false);
        waveTimerText.GetComponent<Text>().text = "Волна в пути";
    }
    private void OnFinishWave()
    {
        startNextWaveButton.SetActive(true);
    }

    private void OnCoinsChanged(int coinsNew)
    {
        coinCountText.GetComponent<Text>().text = coinsNew.ToString();
    }

    private void OnLifesChanged(int lifesNew)
    {
        lifeCountText.GetComponent<Text>().text = lifesNew.ToString();
    }

    private void OnGameLost()
    {
        towerBuildCanvas.SetActive(false);
        towerStatsCanvas.SetActive(false);
        gameStateCanvas.SetActive(false);
        loseCanvas.SetActive(true);
        pauseCollider.SetActive(true);
    }

    public void OnPause()
    {
        towerBuildCanvas.SetActive(false);
        towerStatsCanvas.SetActive(false);
        pauseCollider.SetActive(true);
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void OnContinue()
    {
        pauseCollider.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
