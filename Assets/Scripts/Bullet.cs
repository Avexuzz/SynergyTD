using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target = null;
    public float damage = 0;
    private float speed = 5.12f;
    public Vector2 lastPos = new Vector2(0,0);

    void Awake()
    {
        Messenger<GameObject, int>.AddListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
    }
    void OnDestroy()
    {
        Messenger<GameObject, int>.RemoveListener(GameEvent.ENEMY_KILLED, OnEnemyKilled);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)//get new position, follows last seen position if enemy dies
        {
            lastPos = target.transform.position;
        }
        transform.position = Vector2.MoveTowards(transform.position, lastPos, speed * Time.deltaTime);
        if (target != null)
        {
            if (transform.position == target.transform.position)
            {
                Messenger<GameObject, float>.Broadcast(GameEvent.ENEMY_HIT, target, damage);
                Destroy(this.gameObject);
            }
        }
        else
        {
            if(new Vector2(transform.position.x, transform.position.y) == lastPos)//destroy if reached last position of dead enemy
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    private void OnEnemyKilled(GameObject enemy, int b)
    {
        if(enemy == target)
        {
            target = null;
        }
    }
}
