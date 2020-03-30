using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyButton : MonoBehaviour
{
    [SerializeField] private TowerStats towerStats;
    private int coinsAvailable = 0;
    public int type = 0;
    public GameObject tile;

    void Awake()
    {
        Messenger<int>.AddListener(GameEvent.COINS_CHANGED, OnCoinsChanged);
    }
    void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.COINS_CHANGED, OnCoinsChanged);
    }


    // Start is called before the first frame update
    void Start()
    {
        coinsAvailable = GameObject.Find("SceneController").GetComponent<SceneController>().coins;
        towerStats = GameObject.Find("TowerStats").GetComponent<TowerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnClick()
    {
        if (coinsAvailable >= towerStats.towersBase[type].cost)
        {
            Messenger<int, GameObject>.Broadcast(GameEvent.BUILD_TOWER, type, tile);
            Messenger.Broadcast(GameEvent.BUILD_TOWER_AFTER);//??maybe some better ways to control UI?
            Messenger<int>.Broadcast(GameEvent.COINS_SPENT, towerStats.towersBase[type].cost);
        }

    }

    private void OnCoinsChanged(int coinsNew)//update coins after enemy killed
    {
        coinsAvailable = coinsNew;
    }
}
