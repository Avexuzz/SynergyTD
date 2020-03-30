using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private int coinsAvailable = 0;
    public GameObject tower = null;
    private GameObject gridController;

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
        gridController = GameObject.Find("GridController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (coinsAvailable >= tower.GetComponent<TowerAI>().cost)
        {
            Messenger<int>.Broadcast(GameEvent.COINS_SPENT, tower.GetComponent<TowerAI>().cost);
            tower.GetComponent<TowerAI>().damage *= 1.5f;
            tower.GetComponent<TowerAI>().totalcost += tower.GetComponent<TowerAI>().cost;
            tower.GetComponent<TowerAI>().cost = System.Convert.ToInt32(System.Math.Round(tower.GetComponent<TowerAI>().cost * 1.5f, System.MidpointRounding.AwayFromZero));
            tower.GetComponent<TowerAI>().bonus *= 1.1f;
            gridController.GetComponent<GridController>().OnGridChanged(tower.transform.parent.gameObject);
            Messenger<GameObject>.Broadcast(GameEvent.STATS_UI_REQUEST, tower);

        }

    }

    private void OnCoinsChanged(int coinsNew)//update coins
    {
        coinsAvailable = coinsNew;
    }
}
