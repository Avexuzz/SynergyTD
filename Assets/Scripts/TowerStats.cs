using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//just an inspector and code storage for basic tower stats   ??think about something better
public class TowerStats : MonoBehaviour
{
    
    public TowerStatsStorage[] towersBase;

    void Start()
    {
        int i = 0;
        foreach(TowerStatsStorage tss in towersBase)
        {
            tss.type = i;//setting indexes
            i++;
        }
    }
    void Update()
    {

    }

}

