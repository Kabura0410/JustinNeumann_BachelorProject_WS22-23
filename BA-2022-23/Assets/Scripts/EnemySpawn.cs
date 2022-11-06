using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public enum directionType
    {
        left,
        right
    }

    [SerializeField] private directionType direction;

    public float minSpawnDelay;
    public float maxSpawnDelay;

    private float actualSpawnDelay;

    public bool boosted;

    void Update()
    {
        if(actualSpawnDelay > 0)
        {
            actualSpawnDelay -= Time.deltaTime;
            if(actualSpawnDelay <= 0)
            {
                CalculateSpawnTime();
                if(GameManager.instance.GetRemainingEnemyAmount() > 0)
                {
                    SpawnEnemy();
                }
            }
        }
    }

    public void CalculateSpawnTime()
    {
        actualSpawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
    }

    private void SpawnEnemy()
    {
        GameObject go = Instantiate(GameManager.instance.GetSpawnObject(), transform.position, Quaternion.identity);
        switch (direction)
        {
            case directionType.left:
                go.GetComponent<Enemy>().SetStartDirection(Vector3.left);
                break;
            case directionType.right:
                go.GetComponent<Enemy>().SetStartDirection(Vector3.right);
                break;
        }
        GameManager.instance.ReduceEnemySpawnAmount();
        GameManager.instance.allSpawnedEnemies.Add(go.GetComponent<Enemy>());
    }
}
