using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSheet : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public float initialDelay; // in seconds, delay before the first spawn
        public WaveEnemy[] enemies;

        public IEnumerator SpawnWave()
        {
            foreach (WaveEnemy enemy in enemies)
            {
                yield return enemy.SpawnWaveEnemy();
            }
        }
    }

    [System.Serializable]
    public class WaveEnemy
    {
        public GameObject enemyPrefab;
        public Transform pathParent;
        public int count;
        public float spawnDelay; // in seconds, delay between each spawn
        public float initialDelay; // in seconds, delay before the first spawn

        public IEnumerator SpawnWaveEnemy()
        {
            for (int i = 0; i < count; i++)
            {
                GameObject enemy = Instantiate(enemyPrefab);
                enemy.GetComponent<BaseEnemy>().SetPathParent(pathParent);
                enemy.transform.SetParent(GameObject.Find("Enemies").transform);
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    [SerializeField]
    public Wave[] waves;

    public void Awake()
    {
        StartCoroutine(SpawnWaves());
    }

    public IEnumerator SpawnWaves()
    {
        foreach (Wave wave in waves)
        {
            yield return new WaitForSeconds(wave.initialDelay);
            yield return wave.SpawnWave();
        }
    }
}
