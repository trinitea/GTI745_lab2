using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Starting,
    Running,
    BossTime,
    GameOver,
    PlayerIsDead
}

public class GameController : MonoBehaviour {

    // Game Score
    [SerializeField]
    private Text textScore;

    private int gameScore = 0;

    // Enemies and Hazards
    [SerializeField]
    public GameObject hazard;

    [SerializeField]
    public GameObject spawningVolume;
    //private GameState gameState;

    [SerializeField]
    private float asteroidTimeBetweenSpawn = 1.0f;
    [SerializeField]
    private int asteroidNumberPerSpawn = 15;

    [SerializeField]
    private float ennemyShipTimeBetweenSpawn = 1.0f;
    [SerializeField]
    private float ennemyShipNumberPerSpawn = 1.0f;

    private DifficultySettings difficultySettings;
    private GameState gameState;
    // Use this for initialization
    void Start ()
    {
        gameScore = 0;
        StartCoroutine(AsteroidSpawnWaves());
	}

    /*
    void Update()
    {

    }
    */

    public IEnumerator AsteroidSpawnWaves()
    {
        for(;;)
        {
            for (int i = 0; i < asteroidNumberPerSpawn; i++)
            {
                Instantiate(hazard, GenerateSpawnPosition(), Quaternion.identity);
                yield return new WaitForSeconds(asteroidTimeBetweenSpawn);
            }
        }

        // NextWave();
    }

    public IEnumerator EnnemyShipSpawnWaves()
    {
        for (;;)
        {
            for (int i = 0; i < ennemyShipNumberPerSpawn; i++)
            {
                Instantiate(hazard, GenerateSpawnPosition(), Quaternion.identity);
                yield return new WaitForSeconds(ennemyShipTimeBetweenSpawn);
            }
        }

        // NextWave();
    }

    private Vector3 GenerateSpawnPosition()
    {
        Vector3 spawnVolumePosition = spawningVolume.transform.position;

        return new Vector3(
                spawnVolumePosition.x + Random.Range(-spawningVolume.transform.localScale.x / 2, spawningVolume.transform.localScale.x / 2),
                spawnVolumePosition.y, //+ Random.Range(-spawningVolume.transform.localScale.y / 2, spawningVolume.transform.localScale.y / 2),
                spawnVolumePosition.z //+ Random.Range(-spawningVolume.transform.localScale.z / 2, spawningVolume.transform.localScale.z / 2)
            );
    }

    public void UpdateScore(int points)
    {
        gameScore += points;
        textScore.text = gameScore.ToString();
    }
}
