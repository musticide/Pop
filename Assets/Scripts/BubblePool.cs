using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BubblePool : MonoBehaviour
{
    public GameObject bubblePrefab;
    public int poolSize = 10;
    private Queue<GameObject> pool;
    private List<Transform> bubbleTransforms = new List<Transform>(25);
    private Vector4[] bubblePosScales = new Vector4[25];

    void Start()
    {
        pool = new Queue<GameObject>();

        // Debug.Log("Pool Size: " + poolSize.ToString());

        for (int i = 0; i < poolSize; i++)
        {
            // Debug.Log("BubbleNo: " + i);
            GameObject obj = Instantiate(bubblePrefab, this.transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            bubbleTransforms.Add(obj.transform);
        }
    }

    private void Update() {
        bubblePosScales = bubbleTransforms.Select(t => new Vector4(t.position.x, t.position.y, t.position.z, t.localScale.x)).ToArray();
        Debug.Log("bubble Pos:" + bubblePosScales[0]);
        Shader.SetGlobalVectorArray("_BTrans", bubblePosScales);
    }

    public GameObject GetBubble(Vector3 pos)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            // Debug.Log("Object dequeued");
            obj.transform.position = pos;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogWarning("Bubble Instantiated! Pool size not enough");
            GameObject obj = Instantiate(bubblePrefab, this.transform);
            obj.transform.position = pos;
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

