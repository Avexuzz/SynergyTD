using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scriptable Object/New Enemy")]
public class Enemy : ScriptableObject
{
    public string name;
    public Sprite sprite;
    public float healthFactor;
    public float speedFactor;
    public int number;
    public float bountyFactor;
}