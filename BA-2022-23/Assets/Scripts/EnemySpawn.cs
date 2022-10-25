using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyObject;


    public enum directionType
    {
        left,
        right
    }

    [SerializeField] private directionType direction;

    [SerializeField] private float minSpawnDelay;
    [SerializeField] private float maxSpawnDelay;

    private float actualSpawnDelay;

    void Start()
    {
        SpawnEnemy();
        CalculateSpawnTime();
    }

    void Update()
    {
        if(actualSpawnDelay > 0)
        {
            actualSpawnDelay -= Time.deltaTime;
            if(actualSpawnDelay <= 0)
            {
                CalculateSpawnTime();
                SpawnEnemy();
            }
        }
    }

    private void CalculateSpawnTime()
    {
        actualSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnEnemy()
    {
        GameObject go = Instantiate(enemyObject, transform.position, Quaternion.identity);
        switch (direction)
        {
            case directionType.left:
                go.GetComponent<Enemy>().SetStartDirection(Vector3.left);
                break;
            case directionType.right:
                go.GetComponent<Enemy>().SetStartDirection(Vector3.right);
                break;
        }
    }
}
