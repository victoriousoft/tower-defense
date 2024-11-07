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

    public bool SubtractGold(int value)
    {
        if (gold >= value)
        {
            gold -= value;
            return true;
        }
        else return false;
    }

    public void SubtractLives(int value)
    {
        lives -= value;
        if (lives <= 0) GameOver();
    }

    void GameOver()
    {
        Debug.Log("take the L");
    }
}
