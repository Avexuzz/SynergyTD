using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject SceneController;
    private GameObject enemy;
    private int number = 10;
    private float health = 10.0f;
    private float speed = 1.28f;
    private int bounty = 1;
    [HideInInspector]public List<GameObject> aliveEnemyList = new List<GameObject>();//look at towerAI, maybe not needed but working now
    [SerializeField]private Enemy[] enemies;//list for inspector to add enemy types
    int choice = 0;


    void Awake()
    {
        Messenger.AddListener(GameEvent.START_WAVE, OnStartWave);
    }
    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.START_WAVE, OnStartWave);
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneController = GameObject.Find("SceneController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator WaveCoroutine()//just sending enemies with chosen stats   TODO: add interval tweaks
    {
        choice = Random.Range(0, enemies.Length);
        number = enemies[choice].number;
        for (int i = 0; i<number; i++)
        {
            yield return new WaitForSeconds(1);
            enemy = Instantiate(enemyPrefab);
            enemy.transform.position = new Vector3(SceneController.GetComponent<SceneController>().startX - 0.64f, SceneController.GetComponent<SceneController>().startY, 0);
            aliveEnemyList.Add(enemy.gameObject);
            enemy.GetComponent<SpriteRenderer>().sprite = enemies[choice].sprite;
            enemy.GetComponent<EnemyAI>().hp = health * enemies[choice].healthFactor;
            enemy.GetComponent<EnemyAI>().speed = speed * enemies[choice].speedFactor;
            enemy.GetComponent<EnemyAI>().bounty = System.Convert.ToInt32(System.Math.Round(bounty * enemies[choice].bountyFactor, System.MidpointRounding.AwayFromZero));
        }

        Messenger.Broadcast(GameEvent.FINISH_WAVE);

    }

    private void OnStartWave()
    {
        health *= 2.0f;//increase health and bounty for each wave
        bounty = System.Convert.ToInt32(System.Math.Round(bounty * 1.5f, System.MidpointRounding.AwayFromZero));
        StartCoroutine("WaveCoroutine");
    }
}



