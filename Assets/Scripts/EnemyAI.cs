using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float nextX;
    private float nextY;
    private int index = 0;

    [SerializeField] private GameObject SceneController;
    [SerializeField] private GameObject WaveController;

    public float hp = 100;
    public float speed = 1.28f;
    public int bounty = 1;



    void Awake()
    {
        Messenger<GameObject, float>.AddListener(GameEvent.ENEMY_HIT, OnEnemyHit);
    }
    void OnDestroy()
    {
        Messenger<GameObject, float>.RemoveListener(GameEvent.ENEMY_HIT, OnEnemyHit);
    }


    // Start is called before the first frame update
    void Start()
    {
        SceneController = GameObject.Find("SceneController");
        WaveController = GameObject.Find("WaveController");
        nextX = SceneController.GetComponent<SceneController>().startX;
        nextY = SceneController.GetComponent<SceneController>().startY;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            OnDeath();
        }
        if(SceneController.GetComponent<SceneController>().road[index].index == int.MaxValue)//destination check
        {
            if(transform.position.x == nextX && transform.position.y == nextY)
            {
                Messenger.Broadcast(GameEvent.DEST_REACHED);
                WaveController.GetComponent<WaveController>().aliveEnemyList.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
        }
        else if(transform.position.x == nextX && transform.position.y == nextY)//normal moving on road
        {
            index++;
            nextX = SceneController.GetComponent<SceneController>().road[index].posX;
            nextY = SceneController.GetComponent<SceneController>().road[index].posY;
        }

        if ((transform.position.x != nextX) || (transform.position.y != nextY))//rotation to movepoint, fixed random spins in middle of tiles
        {
            Vector3 dir = new Vector3(nextX, nextY, 0) - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextX, nextY, 0), speed * Time.deltaTime);
        
    }

    private void OnDeath()
    {
        Messenger<GameObject, int>.Broadcast(GameEvent.ENEMY_KILLED, this.gameObject, this.bounty);
        WaveController.GetComponent<WaveController>().aliveEnemyList.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    private void OnEnemyHit(GameObject target, float damage)
    {
        if(target == this.gameObject)
        {
            hp -= damage;
        }
    }
}
