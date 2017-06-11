using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Team
{
    Player,
    Enemy
}

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
    public List<GameObject> hazard;

    [SerializeField]
    public GameObject enemyShip;

    [SerializeField]
    public GameObject spawningVolume;
    //private GameState gameState;

    [SerializeField]
    private float asteroidTimeBetweenSpawn = 1.0f;
    [SerializeField]
    private int asteroidNumberPerSpawn = 30;

    [SerializeField]
    private float enemyShipTimeBetweenSpawn = 3.0f;
    [SerializeField]
    private int enemyShipNumberPerSpawn = 10;

    private DifficultySettings difficultySettings;
    private GameState gameState;
    // Use this for initialization

    public DialogHelper dialogHelper;

    void Start ()
    {
        if(GameModel.gameDifficulty == Difficulty.Easy)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("EasyUnavailable"))
            {
                obj.SetActive(false);
            }
        }

        difficultySettings = DifficultySettings.getSettings(GameModel.gameDifficulty);
        gameScore = 0;
        StartSpawn();
    }

    /*
    void Update()
    {

    }
    */
    private void StartSpawn()
    {
        if (difficultySettings.asteroidShouldSpawn) StartCoroutine("AsteroidSpawnWaves");
        if (difficultySettings.enemyShipShouldSpawn) StartCoroutine("EnemyShipSpawnWaves");
    }

    public IEnumerator AsteroidSpawnWaves()
    {
        for (;;)
        {
            yield return new WaitForSeconds(3.0f);

            for (int i = 0; i < asteroidNumberPerSpawn * difficultySettings.asteroidSpawnMultiplier; i++)
            {
                Instantiate(hazard[Random.Range(0, hazard.Count)], GenerateSpawnPosition(), new Quaternion(0.0f, 90.0f, 0.0f, 1.0f));
                yield return new WaitForSeconds(asteroidTimeBetweenSpawn * difficultySettings.asteroidSpawnTimeMultiplier);
            }
        }
    }

    public IEnumerator EnemyShipSpawnWaves()
    {
        for (;;)
        {

            yield return new WaitForSeconds(3.0f);

            for (int i = 0; i < enemyShipNumberPerSpawn * difficultySettings.enemyShipSpawnMultiplier; i++)
            {
                Instantiate(enemyShip, GenerateSpawnPosition(), new Quaternion(0.0f, 90.0f, 0.0f, 1.0f));
                yield return new WaitForSeconds(enemyShipTimeBetweenSpawn * difficultySettings.asteroidSpawnTimeMultiplier);
            }
        }
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
        if (points <= 0) return;

        gameScore += points;
        textScore.text = gameScore.ToString();
    }

    public void OnGameOver()
    {
        StartCoroutine("GameOverProcess");

        StopCoroutine("AsteroidSpawnWaves");
        StopCoroutine("EnemyShipSpawnWaves");

        Invoke("DialogHelper.ShowGameOverDialog(OnGameOverCallback)", 2);
    }

    IEnumerator GameOverProcess()
    {
        StopCoroutine("AsteroidSpawnWaves");
        StopCoroutine("EnemyShipSpawnWaves");

        yield return new WaitForSeconds(2f);

        dialogHelper.ShowGameOverDialog(OnGameOverCallback);
    }

    private void OnGameOverCallback(bool restart)
    {
        if(restart)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Hazard"))
            {
                Destroy(obj);
            }

            gameScore = 0;
            textScore.text = gameScore.ToString();

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Respawn();

            StartSpawn();
        }
        else
        {
            SceneManager.LoadScene("MenuScreen");
        }
        
    }
}
