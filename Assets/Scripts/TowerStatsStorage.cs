using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Scriptable Object/New Tower")]
public class TowerStatsStorage : ScriptableObject
{
    [HideInInspector]
    public int type;//id, not intended to be changed manually
    public Sprite sprite;//TODO: add bullet sprites
    public string displayName;
    public float damage;
    public float range;
    public float AS;
    public int cost;
    public float bonus;
    public int bonusType;
    public string ability;//just a dummy now   //TODO: Implement abilities
}
