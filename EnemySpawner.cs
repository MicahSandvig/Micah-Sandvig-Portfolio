using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple Spawner
//Will spawn enemies around the player continuosly at set interval

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject swarmerPrefab;

    [SerializeField]
    private float swarmerInterval = 3.5f;

    public Transform target;
    public Vector3 offset;

    void Update()
    {
        Vector3 desiredPosition = target.position + (Vector3)offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 5f * Time.deltaTime);
    }

    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval, swarmerPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);

        Vector2 randomCircle = Random.insideUnitCircle.normalized * 15f;
        Vector3 spawnPosition = new Vector3(
            target.position.x + randomCircle.x,
            target.position.y + randomCircle.y,
            0f // keep on 2D plane
        );

        GameObject newEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);

        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
