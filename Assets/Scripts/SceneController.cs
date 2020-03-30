using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    [SerializeField] private GameObject baseTile;
    [SerializeField] private GameObject roadTile;
    [SerializeField] private GameObject destTile;

    private GameObject gridController;

    public int fieldHeight = 9;
    public int fieldWidth = 16;
    [HideInInspector] public List<List<int>> fieldArray;
    [HideInInspector] public int startHPoint;

    [HideInInspector] public Vector3 leftUpCorner;
    [HideInInspector] public float offsetX = 0.64f;
    [HideInInspector] public float offsetY = 0.64f;

    [HideInInspector] public float startX = 0.0f;
    [HideInInspector] public float startY = 0.0f;

    [HideInInspector] public List<roadCoords> road = new List<roadCoords>();

    [HideInInspector] public int timeUntilNextWave {get; private set;}
    [SerializeField] public int coins = 50;
    [SerializeField] public int lifes = 30;


    void Awake()
    {
        Messenger.AddListener(GameEvent.FINISH_WAVE, OnFinishWave);
        Messenger.AddListener(GameEvent.FORCE_START_WAVE, OnForceStartWave);
        Messenger<GameObject, int>.AddListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger<int>.AddListener(GameEvent.COINS_SPENT, OnCoinsSpent);
        Messenger.AddListener(GameEvent.DEST_REACHED, OnDestReached);
    }
    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.FINISH_WAVE, OnFinishWave);
        Messenger.RemoveListener(GameEvent.FORCE_START_WAVE, OnForceStartWave);
        Messenger<GameObject, int>.RemoveListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
        Messenger<int>.RemoveListener(GameEvent.COINS_SPENT, OnCoinsSpent);
        Messenger.RemoveListener(GameEvent.DEST_REACHED, OnDestReached);
    }

    // Start is called before the first frame update
    void Start()
    {
        gridController = GameObject.Find("GridController").gameObject;
        fieldHeight = PlayerPrefs.GetInt("height");
        fieldWidth = PlayerPrefs.GetInt("width");
        FieldGenerate();
        TimerSetUp();
        StartCoroutine("TimerCoroutine");
    }

    // Update is called once per frame
    void Update()
    {
        if(timeUntilNextWave == 0)//check wave timer
        {
            Messenger.Broadcast(GameEvent.START_WAVE);
            TimerSetUp();
        }
        if(lifes == 0)//check lose condition
        {
            Messenger.Broadcast(GameEvent.GAME_LOST);
            Time.timeScale = 0;
            lifes = int.MaxValue;
        }
    }

    private void FieldGenerate()//random gamefield generation with snaky road   ??maybe look for pathfinding algorhytms and make backwards roads possible? try later
    {
        leftUpCorner = new Vector3(-fieldWidth * offsetX / 2, fieldHeight * offsetY / 2, 4);
        fieldArray = new List<List<int>>(fieldHeight);
        List<int> row;
        for(int i = 0; i < fieldHeight; i++)
        {
            row = new List<int>();
            for (int j = 0; j < fieldWidth; j++)
            {
                row.Add(0);
            }
            fieldArray.Add(row);
        }
        bool reachDest = false;
        startHPoint = Random.Range(0, fieldHeight);
        int headWidth = 0;
        int headHeight = startHPoint;
        fieldArray[headHeight][headWidth] = 1;
        int direction;
        bool possible = false;
        bool up = false, down = false, left = false, right = false;
        int index = 2;
        while(reachDest == false)//randoming road
        {
            direction = Random.Range(0, 3);
            switch (direction)
            {
                case 0:
                    if(headHeight > 0)
                    {
                        if (fieldArray[headHeight - 1][headWidth] == 0)
                        {
                            possible = true;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (headWidth == 0)
                    {
                        left = true;
                    }
                    else if(fieldArray[headHeight - 1][headWidth - 1] == 0)
                    {
                        left = true;
                    }
                   
                    if(headWidth == fieldWidth-1)
                    {
                        right = true;
                    }
                    else if (fieldArray[headHeight - 1][headWidth + 1] == 0)
                    {
                        right = true;
                    }

                    if (possible && left && right)
                    {
                        fieldArray[headHeight - 1][headWidth] = index;
                        index++;
                        headHeight--;
                    }

                    break;
                case 1:
                    if (headWidth == (fieldWidth-1))
                    {
                         possible = true;
                         reachDest = true;
                         break;
                    }
                    else if (fieldArray[headHeight][headWidth + 1] == 0)
                    {
                        possible = true;
                        right = true;
                    }

                    if (headHeight == 0)
                    {
                        up = true;
                    }
                    else if (fieldArray[headHeight - 1][headWidth + 1] == 0)
                    {
                        up = true;
                    }

                    if (headHeight == fieldHeight-1)
                    {
                        down = true;
                    }
                    else if (fieldArray[headHeight + 1][headWidth + 1] == 0)
                    {
                        down = true;
                    }

                    if (possible && up && down && !reachDest)
                    {
                        fieldArray[headHeight][headWidth + 1] = index;
                        index++;
                        headWidth++;
                    }
                    break;
                case 2:
                    if (headHeight < fieldHeight - 1)
                    {
                        if (fieldArray[headHeight + 1][headWidth] == 0)
                        {
                            possible = true;
                            down = true;
                        }
                    }
                    else
                    {
                        break;
                    }

                    if (headWidth == 0)
                    {
                        left = true;
                    }
                    else if (fieldArray[headHeight + 1][headWidth - 1] == 0)
                    {
                        left = true;
                    }

                    if (headWidth == fieldWidth-1)
                    {
                        right = true;
                    }
                    else if (fieldArray[headHeight + 1][headWidth + 1] == 0)
                    {
                        right = true;
                    }



                    if (possible && left && right)
                    {
                        fieldArray[headHeight + 1][headWidth] = index;
                        index++;
                        headHeight++;
                    }

                    break;
                default:
                    break;
            }
            if (reachDest)
            {
                    fieldArray[headHeight][headWidth] = int.MaxValue;
            }
            else
            {
                possible = up = down = left = right = false;
            }
        }
        for(int i = 0; i < fieldHeight; i++)//final field build with grid
        {
            gridController.GetComponent<GridController>().grid.Add(new List<GridCell>());
            for ( int j = 0; j < fieldWidth; j++)
            {
                gridController.GetComponent<GridController>().grid[i].Add(new GridCell());
                GameObject tile;
                switch (fieldArray[i][j])
                {
                    case 0:
                        tile = Instantiate(baseTile);
                        gridController.GetComponent<GridController>().grid[i][j].tile = tile;
                        break;
                    case int.MaxValue:
                        tile = Instantiate(destTile);
                        break;
                    default:
                        tile = Instantiate(roadTile);
                        break;
                }

                float posX = (offsetX * j) + leftUpCorner.x;
                float posY = -(offsetY * i) + leftUpCorner.y;
                tile.transform.position = new Vector3(posX, posY, leftUpCorner.z);
                if (fieldArray[i][j] > 0)
                {
                    road.Add(new roadCoords(fieldArray[i][j], posX, posY));
                }

                gridController.GetComponent<GridController>().grid[i][j].x = i;
                gridController.GetComponent<GridController>().grid[i][j].y = j;

                if (j == 0 && i == startHPoint)
                {
                    startX = posX;
                    startY = posY;
                }
            }
        }

        roadCoordsComparer r = new roadCoordsComparer();
        road.Sort(r);
    }

    private void TimerSetUp()
    {
        timeUntilNextWave = 30;
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeUntilNextWave > 0)
        {
            yield return new WaitForSeconds(1);
            timeUntilNextWave--;
            Messenger<int>.Broadcast(GameEvent.UI_WAVE_TIMER_CHANGED, timeUntilNextWave);
        }

    }

    private void OnFinishWave()
    {
        TimerSetUp();
        StartCoroutine("TimerCoroutine");
    }

    private void OnForceStartWave()
    {
        StopCoroutine("TimerCoroutine");
        timeUntilNextWave = 0;
    }

    private void OnEnemyKilled(GameObject o, int bounty)
    {
        coins += bounty;
        Messenger<int>.Broadcast(GameEvent.COINS_CHANGED, coins);
    }

    private void OnCoinsSpent(int coinsSpent)
    {
        coins -= coinsSpent;
        Messenger<int>.Broadcast(GameEvent.COINS_CHANGED, coins);
    }

    private void OnDestReached()
    {
        lifes--;
        Messenger<int>.Broadcast(GameEvent.LIFES_CHANGED, lifes);
    }
}


public class roadCoordsComparer : IComparer<roadCoords>
{
    public int Compare(roadCoords r1, roadCoords r2)
    {
        if (r1.index > r2.index){
            return 1;
        }
        else if (r1.index < r2.index)
        {
            return -1;
        }

        return 0;
    }
}

public struct roadCoords
{
    public int index;
    public float posX;
    public float posY;
    public roadCoords(int i, float x, float y)
    {
        index = i;
        posX = x;
        posY = y;
    }
}