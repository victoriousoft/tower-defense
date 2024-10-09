using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    public int lives;
    public int gold;
    private GameObject[] exits;
    void Awake()
    {
        exits = GameObject.FindGameObjectsWithTag("Exit");
    }
    public void AddGold(int value)
    {
        gold += value;
    }
    public bool SubGold(int value)
    {
        if (gold >= value)
        {
            gold -= value;
            return true;
        }
        else return false;
    }
    public void SubLives(int value)
    {
        lives -= value;
        Debug.Log("hp= " + lives);
        if (lives <= 0) GameOver();
    }
    void GameOver()
    {
        Debug.Log("take the L");
    }
}
