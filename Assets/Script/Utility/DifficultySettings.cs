using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Impossibru
}

public class DifficultySettings : MonoBehaviour {

    public bool asteroidShouldSpawn { get; set; }
    public float asteroidSpawnMultiplier { get; set; }
    public float asteroidSpawnTimeMultiplier { get; set; }

    public bool enemyShipShouldSpawn { get; set; }
    public float enemyShipSpawnMultiplier { get; set; }
    public float enemyShipSpawnTimeMultiplier { get; set; }

    public bool bossShouldSpawn { get; set; }
    public float bossTimeBeforSpawn { get; set; }

    // public float healthMultiplier = 1.0f

    public DifficultySettings(bool hasAsteroid, float asteroidMultiplier, float asteroidTimeMultiplier,
        bool hasEnemyShip, float enemyShipMultiplier, float enemyShipTimeMultiplier,
        bool hasBoss, float bossTimer)
    {
        asteroidShouldSpawn = hasAsteroid;
        asteroidSpawnMultiplier = asteroidMultiplier;
        asteroidSpawnTimeMultiplier = asteroidTimeMultiplier;

        enemyShipShouldSpawn = hasEnemyShip;
        enemyShipSpawnMultiplier = enemyShipMultiplier;
        enemyShipSpawnTimeMultiplier = enemyShipTimeMultiplier;

        bossShouldSpawn = hasBoss;
        bossTimeBeforSpawn = bossTimer;
    }

    static DifficultySettings getSettings(Difficulty difficulty)
    {
        DifficultySettings settings = null;

        switch(difficulty)
        {
            case Difficulty.Easy:
                settings = new DifficultySettings(true, 0.8f, 1.2f, false, 1.0f, 1.0f, false, 90.0f);
                break;

            case Difficulty.Normal:
                settings = new DifficultySettings(true, 1.0f, 1.0f, true, 1.0f, 1.0f, false, 90.0f);
                break;

            case Difficulty.Hard:
                settings = new DifficultySettings(true, 2.0f, 0.75f, true, 1.5f, 0.75f, false, 90.0f);
                break;

            case Difficulty.Impossibru:
                settings = new DifficultySettings(false, 1.0f, 1.0f, false, 0.0f, 1.0f, true, 2.5f); // boss not supported yet
                break;
        }

        return settings;
    }
}
