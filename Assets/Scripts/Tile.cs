using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TowerStats towerStats;
    [HideInInspector]public int type = int.MaxValue;
    private GameObject tower = null;
    private GameObject gridController;
    [SerializeField] private Sprite commonSprite;
    [SerializeField] private Sprite selectedSprite;

    void Awake()
    {
        Messenger<int, GameObject>.AddListener(GameEvent.BUILD_TOWER, OnBuild);
        Messenger.AddListener(GameEvent.TOWER_UNSELECTED, OnUnselect);
    }
    void OnDestroy()
    {
        Messenger<int, GameObject>.RemoveListener(GameEvent.BUILD_TOWER, OnBuild);
        Messenger.RemoveListener(GameEvent.TOWER_UNSELECTED, OnUnselect);
    }

    // Start is called before the first frame update
    void Start()
    {
        gridController = GameObject.Find("GridController").gameObject;
        towerStats = GameObject.Find("TowerStats").gameObject.GetComponent<TowerStats>();
        this.gameObject.GetComponent<SpriteRenderer>().sprite = commonSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        Messenger.Broadcast(GameEvent.TOWER_UNSELECTED);
        this.gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        if (type == int.MaxValue)
        {
            Messenger<GameObject>.Broadcast(GameEvent.BUILD_UI_REQUEST, this.gameObject);
        }
        else
        {
            Messenger<GameObject>.Broadcast(GameEvent.STATS_UI_REQUEST, tower);
        }
    }

    private void OnBuild(int buildType, GameObject tile)
    {
        if (tile == this.gameObject) {
            type = buildType;
            tower = Instantiate(towerPrefab);
            tower.transform.SetParent(this.gameObject.transform);
            tower.transform.position = this.transform.position;
            tower.transform.Translate(0, 0, -1);

            tower.GetComponent<TowerAI>().type = buildType;
            tower.GetComponent<TowerAI>().displayName = towerStats.towersBase[buildType].displayName;
            tower.GetComponent<SpriteRenderer>().sprite = towerStats.towersBase[buildType].sprite;
            tower.GetComponent<TowerAI>().damage = towerStats.towersBase[buildType].damage;
            tower.GetComponent<TowerAI>().range = towerStats.towersBase[buildType].range;
            tower.GetComponent<TowerAI>().AS = towerStats.towersBase[buildType].AS;
            tower.GetComponent<TowerAI>().cost = towerStats.towersBase[buildType].cost;
            tower.GetComponent<TowerAI>().bonus = towerStats.towersBase[buildType].bonus;
            tower.GetComponent<TowerAI>().ability = towerStats.towersBase[buildType].ability;
            tower.GetComponent<TowerAI>().fullAS = towerStats.towersBase[buildType].AS;
            tower.GetComponent<TowerAI>().fullDamage = towerStats.towersBase[buildType].damage;
            gridController.GetComponent<GridController>().OnGridNewObject(this.gameObject);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = commonSprite;
        }
        
    }

    private void OnUnselect()
    {
        if(this.gameObject.GetComponent<SpriteRenderer>().sprite == selectedSprite)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = commonSprite;
        }
    }
}
