using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour
{
    public GameObject bubblePrefab;
    public int poolSize = 10;
    private Queue<GameObject> pool;

    void Awake()
    {
        pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bubblePrefab, this.transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetBubble()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            Debug.Log("Object dequeued");
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("Bubble Instantiated! Pool size not enough");
            GameObject obj = Instantiate(bubblePrefab);
            obj.SetActive(true);
            return obj;
        }
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        Debug.Log("Obj Set Inactive");
        pool.Enqueue(obj);
    }
}

