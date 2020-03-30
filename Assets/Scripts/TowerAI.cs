using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public int type = int.MaxValue;

    public string displayName = "";
    public float damage = 0.0f;
    public float range = 0.0f;
    public float AS = 0.0f;
    public int cost = 0;
    public float bonus = 1.0f;
    public string ability = "";

    public float fullDamage = 0.0f;
    public float FullAS = 0.0f;
    public float fullAS {//set max possible attack speed if overwhelms
        get {
            return FullAS;
        }
        set
        {
            if (value > 600.0f)
            {
                FullAS = 600.0f;
            }
            else
            {
                FullAS = value;
            }
        }
    }

    public int totalcost = 0;

    [SerializeField] private GameObject WaveController;
    [SerializeField] private GameObject bulletPrefab;
    private GameObject target;
    private GameObject bullet;
    

    // Start is called before the first frame update
    void Start()
    {
        WaveController = GameObject.Find("WaveController");
        StartCoroutine("FireCoroutine");
        totalcost += cost;
        cost = System.Convert.ToInt32(System.Math.Round(cost * 1.5f, System.MidpointRounding.AwayFromZero));
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)//rotation to target with mirroing (so no strange top-downs)
        {
            Vector3 dir = new Vector3(target.transform.position.x, target.transform.position.y, 0) - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
            if (angle > 90 && angle <= 270)
            {
                transform.localScale = new Vector2(0.5f, -0.5f);
            }
            if (angle <= 90 || angle > 270)
            {
                transform.localScale = new Vector2(0.5f, 0.5f);
            }
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (range < Vector2.Distance(transform.position, target.transform.position))
            {
                target = null;
            }
        }

        if (target == null)//getting new possible target from list    ??try with collders maybe? needs testing   || current way is still working btw
        {
            float minDistance = float.MaxValue;
            GameObject closestEnemy = null;
            foreach (GameObject enemy in WaveController.GetComponent<WaveController>().aliveEnemyList)
            {
                float distance = 0.0f;
                distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
            if (minDistance <= range)
            {
                target = closestEnemy;
            }
        }

        

    }

    private IEnumerator FireCoroutine()
    {
        while (true)
        {
            if(target != null)//firing with set attack speed
            {
                bullet = Instantiate(bulletPrefab);
                bullet.transform.position = transform.position;
                bullet.GetComponent<Bullet>().target = target;
                bullet.GetComponent<Bullet>().damage = fullDamage;
                bullet.GetComponent<Bullet>().lastPos = target.transform.position;
                yield return new WaitForSeconds(60 / fullAS);
            }
            else
            {
                while (target == null)//wait mode to start firing immediately
                {
                    yield return null;
                }
            }
        }
    }
}
