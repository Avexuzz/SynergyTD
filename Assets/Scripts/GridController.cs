// THIS SCRIPT MAKES BONUS SYSTEM WORK
// STILL NEED SOME IMPROVEMENTS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private TowerStats towerStats;

    public List<List<GridCell>> grid = new List<List<GridCell>>();
    private int totalGrids = 0;

   
    // Start is called before the first frame update
    void Start()
    {
        towerStats = GameObject.Find("TowerStats").gameObject.GetComponent<TowerStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RecalculateGridBonus()//recalculates bonus given to every tower, grid is not affected
    {
        for (int i = 0; i < totalGrids; i++)
        {
            float damageFactor = 1.0f;
            float ASFactor = 1.0f;
            foreach (List<GridCell> gs in grid)
            {
                foreach (GridCell gsc in gs)
                {
                    if (i == gsc.gridNumber)
                    {
                        if (gsc.bonusType == 1)
                        {
                            damageFactor += gsc.bonusFactor - 1;
                        }
                        if (gsc.bonusType == 2)
                        {
                            ASFactor += gsc.bonusFactor - 1;
                        }
                        if (gsc.bonusType == 3)
                        {
                            damageFactor += gsc.bonusFactor - 1;
                            ASFactor += gsc.bonusFactor - 1;
                        }
                    }

                }
            }
            foreach (List<GridCell> gs in grid)
            {
                foreach (GridCell gsc in gs)
                {
                    if (i == gsc.gridNumber)
                    {
                        gsc.tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().fullDamage = gsc.tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().damage * damageFactor;
                        gsc.tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().fullAS = gsc.tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().AS * ASFactor;
                    }

                }
            }

        }




    }

    public void OnGridNewObject(GameObject tile)//actions on adding new tower to grid, affects grids
    {
        GridCell cell = null;
        int cellX = 0;
        int cellY = 0;
        int gridNear;
        foreach (List<GridCell> gs in grid)
        {
            foreach (GridCell gsc in gs)
            {
                if (tile == gsc.tile)
                {
                    cell = gsc;
                    cellX = gsc.x;
                    cellY = gsc.y;
                }
            }
        }

        cell.bonusType = towerStats.towersBase[(int)tile.GetComponent<Tile>().type].bonusType;
        cell.bonusFactor = tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().bonus;
        cell.gridNumber = totalGrids;//new tower - new grid
        totalGrids++;
        if (cellX > 0)//check connecting grids, merge them
        {
            gridNear = grid[cellX - 1][cellY].gridNumber;
            if (gridNear != int.MaxValue)
            {
                int gridMin = System.Math.Min(cell.gridNumber, gridNear);
                cell.gridNumber = gridMin;
                RecalculateGrids(gridMin, gridNear);
            }
        }
        if (cellY > 0)
        {
            gridNear = grid[cellX][cellY - 1].gridNumber;
            if (gridNear != int.MaxValue)
            {
                int gridMin = System.Math.Min(cell.gridNumber, gridNear);
                cell.gridNumber = gridMin;
                RecalculateGrids(gridMin, gridNear);
            }
        }
        if (cellX < grid.Count - 1)
        {
            gridNear = grid[cellX + 1][cellY].gridNumber;
            if (gridNear != int.MaxValue)
            {
                int gridMin = System.Math.Min(cell.gridNumber, gridNear);
                cell.gridNumber = gridMin;
                RecalculateGrids(gridMin, gridNear);
            }
        }
        if (cellY < grid[cellX].Count - 1)
        {
            gridNear = grid[cellX][cellY + 1].gridNumber;
            if (gridNear != int.MaxValue)
            {
                int gridMin = System.Math.Min(cell.gridNumber, gridNear);
                cell.gridNumber = gridMin;
                RecalculateGrids(gridMin, gridNear);
            }

        }
        RecalculateGridBonus();//get new bonuses after merges

    }

    private void RecalculateGrids(int gridMin, int gridNear) //merge function actually
    {
        foreach (List<GridCell> gs in grid)
        {
            foreach (GridCell gsc in gs)
            {
                if (gsc.gridNumber == gridNear)
                {
                    gsc.gridNumber = gridMin;
                }
            }
        }
    }


    public void OnGridChanged(GameObject tile)//made public for correct stats UI, try to find better solution 
    {
        foreach (List<GridCell> gs in grid)
        {
            foreach (GridCell gsc in gs)
            {
                if (tile == gsc.tile)
                {
                    gsc.bonusFactor = tile.transform.GetChild(0).gameObject.GetComponent<TowerAI>().bonus;
                }
            }
        }
        RecalculateGridBonus();
    }

}

public class GridCell
{
    public int x = 0;
    public int y = 0;
    public GameObject tile = null;
    public int bonusType = 0;
    public float bonusFactor = 1.0f;
    public int gridNumber = int.MaxValue;
}

