﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public static int stepsTaken = 0;

    [Tooltip("How long enemies spawn for")]
    [SerializeField]
    [Range(5.0f, 60.0f)]
    private float spawnDuration = 30.0f;

    [Tooltip("How many steps for the treasure turtle to spawn (repeatedly)")]
    [SerializeField]
    [Range(1, 10000)]
    private int treasureTurtleStep = 5000;

    [Tooltip("Treasure turtle to be spawned")]
    [SerializeField]
    private TreasureTurtle treasureTurtle;

    [Tooltip("Rate at which the enemies will spawn")]
    public float spawnRate;

    [Tooltip("Enemies which will be randomly spawned on the spawn line")]
    public Enemy[] enemiesToSpawn;

    [Tooltip("Starting position for the spawn line")]
    public Vector2 startingPosition;

    [Tooltip("Ending position for the spawn line")]
    public Vector2 endingPosition;

    private readonly Color spawnLineColor = Color.red;

    private int steps = int.Parse(FitbitRestClient.ActivitiesDaily.summary.steps);

    void Awake()
    {
        steps /= 1000;
        spawnRate = 1.0f / (steps + 1.0f);
        bagoodyba = spawnDuration;
    }

    void Start()
    {
        Physics2D.IgnoreLayerCollision(8, 8, true);
        InvokeRepeating("SpawnEnemy", 1, spawnRate);
    }

    public float bagoodyba;
    void Update()
    {
        spawnDuration -= Time.deltaTime;
        stepsTaken = (int)(steps - steps * spawnDuration / bagoodyba);
        if(stepsTaken % treasureTurtleStep == 0)
        {
            //Instantiate(treasureTurtle, new Vector3(startingPosition.x, Random.Range(endingPosition.y, startingPosition.y)), Quaternion.identity);
        }
        if (spawnDuration <= 0)
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }

    void OnDrawGizmosSelected()
    {
        DrawEnemySpawnOrigin();
    }

    private void DrawEnemySpawnOrigin()
    {
        Gizmos.color = spawnLineColor;
        Gizmos.DrawLine(startingPosition, endingPosition);
        Gizmos.DrawIcon(startingPosition, FileNames.EnemySpawnPositionGizmo);
        Gizmos.DrawIcon(endingPosition, FileNames.EnemySpawnPositionGizmo);
    }

    private void SpawnEnemy()
    {
        Instantiate(enemiesToSpawn[Random.Range(0, enemiesToSpawn.Length)], new Vector3(startingPosition.x, Random.Range(endingPosition.y, startingPosition.y)), Quaternion.identity);
    }
}