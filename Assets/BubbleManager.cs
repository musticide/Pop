using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    float timeSinceSpawn = 0;
    float spawnInterval = 0;
    BubblePool bubblePool;

    private void Start()
    {
        bubblePool = GetComponent<BubblePool>();
    }
    private void Update()
    {
        if (timeSinceSpawn >= spawnInterval)
        {
            SpawnBubble();
            timeSinceSpawn = 0;
            spawnInterval = Random.Range(0.1f, 2.0f);
        }
        timeSinceSpawn += Time.deltaTime;
    }

    void SpawnBubble()
    {
        bubblePool.GetBubble(GetSpawnPos());
    }

    Vector3 GetSpawnPos()
    {
        Vector3 randPos = new Vector3(0, -10, 0);
        randPos.x = Random.Range(-2.5f, 2.5f);
        return randPos;

    }
}
