using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour
{
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField] private float _spawnRate;
     [SerializeField] private GameObject _pickupPrefab;
     [SerializeField] private float _spawnRatePickup;

    private float rightMax = 9f;
    private float leftMax = -9f;
    private float upMax = 12f;
    private float downMax = 4f;
    private List<Obstacle> obstacles;

    private Coroutine currentSpawner;
    private bool isCurrentCoroutineFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        obstacles = new List<Obstacle>();



        /*for (int i = 0; i < 100; i++) {
            Obstacle obstacle = Instantiate(_obstaclePrefab, Vector3.zero +  new Vector3(0, 50, 0), Quaternion.identity).GetComponent<Obstacle>();
            obstacles.Add(obstacle);
        }*/

        StartCoroutine(Level());
        StartCoroutine(SpawnPickup());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Level() {
        currentSpawner = SpawnBigAsteroid();

        while (!isCurrentCoroutineFinished) {
            yield return null;
        }

        currentSpawner = SpawnAsteroidsCloud();
        while (!isCurrentCoroutineFinished) {
            yield return null;
        }

        currentSpawner = SpawnZigZag();
        while (!isCurrentCoroutineFinished) {
            yield return null;
        }

        currentSpawner = SpawnBigAsteroid();
        while (!isCurrentCoroutineFinished) {
            yield return null;
        }

    }


    Coroutine SpawnSpiral() {
        float spawnRate = 0.6f;
        ObstacleType[] obstacleCourse = {ObstacleType.BigDown,
                                            ObstacleType.BigLeft,
                                            ObstacleType.BigUp,
                                            ObstacleType.BigRight,
                                            ObstacleType.BigDown,
                                            ObstacleType.BigLeft,
                                            ObstacleType.BigUp,
                                            ObstacleType.BigRight};
        return StartCoroutine(SpawnObstacles(obstacleCourse, spawnRate));
    }

    Coroutine SpawnAsteroidsCloud() {
        float spawnRate = 0.3f;
        int numberAsteroids = 20;
        ObstacleType[] obstacleCourse = new ObstacleType[numberAsteroids];
        for (int i = 0; i < numberAsteroids; i++) {
            obstacleCourse[i] = ObstacleType.SmallRandom;
        }
        
        return StartCoroutine(SpawnObstacles(obstacleCourse, spawnRate));
    }

    Coroutine SpawnZigZag() {

        float spawnRate = 0.6f;
        ObstacleType[] obstacleCourse = {ObstacleType.BigLeft,
                                            ObstacleType.BigRight,
                                            ObstacleType.BigLeft,
                                            ObstacleType.BigRight,
                                            ObstacleType.BigLeft,
                                            ObstacleType.BigRight,
                                            ObstacleType.BigLeft,
                                            ObstacleType.BigRight};
        return StartCoroutine(SpawnObstacles(obstacleCourse, spawnRate));
    }

    Coroutine SpawnBigAsteroid() {

        int asteroidSize = 20;
        float spawnRate = 0.4f;

        ObstacleType[] obstacleCourse = new ObstacleType[asteroidSize];
        for (int i = 0; i < asteroidSize; i++) {
            obstacleCourse[i] = ObstacleType.BigAll;
        }
        return StartCoroutine(SpawnObstacles(obstacleCourse, spawnRate));
    }

    IEnumerator SpawnObstacles(ObstacleType[] obstacleCourse, float spawnRate) {
            isCurrentCoroutineFinished = false;
            for (int i = 0; i < obstacleCourse.Length; i++) {
                Obstacle obstacle = Instantiate(_obstaclePrefab, Vector3.zero, Quaternion.identity).GetComponent<Obstacle>();
                obstacle.CreateObstacle(obstacleCourse[i]);
                yield return new WaitForSeconds(spawnRate);
            }
            yield return new WaitForSeconds(spawnRate);
            isCurrentCoroutineFinished = true;
    }
    IEnumerator SpawnInfinity() {
        ObstacleType[] obstacleTypes = {ObstacleType.BigDown, ObstacleType.BigLeft, ObstacleType.BigRight, ObstacleType.BigUp, ObstacleType.SmallRandom};
        while (true) {
            //Obstacle obstacle = obstacles.First(o => o.isInUse == false);
            //obstacle.transform.position = new Vector3(Random.Range(-9f, 9f), Random.Range(4f, 12f), 60);
            //obstacle.isInUse = true;
            Obstacle obstacle = Instantiate(_obstaclePrefab, Vector3.zero, Quaternion.identity).GetComponent<Obstacle>();
            obstacle.CreateObstacle((ObstacleType) UnityEngine.Random.Range(0, 5));
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    IEnumerator SpawnPickup() {
        while(true) {
            Instantiate(_pickupPrefab, new Vector3(UnityEngine.Random.Range(leftMax, rightMax), UnityEngine.Random.Range(downMax, upMax), UnityEngine.Random.Range(60, 70)), Quaternion.identity);
            yield return new WaitForSeconds(_spawnRatePickup);
        }
    }
}
