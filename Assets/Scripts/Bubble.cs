using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float speed = 2f;
    private BubblePool bubblePool;

    void Start()
    {
        bubblePool = FindObjectOfType<BubblePool>();
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Debug.Log("BubblePosition: " + Camera.main.WorldToViewportPoint(transform.position).y);
        if (Camera.main.WorldToViewportPoint(transform.position).y > 1)
        {
            KillBubble();
            Debug.Log("Bubble out of bounds");
            GameManager.Instance.LoseLife();
        }
    }

    void OnMouseDown()
    {
        KillBubble();
        GameManager.Instance.AddScore();
    }

    void KillBubble()
    {
        transform.position = bubblePool.transform.position;
        bubblePool.ReturnObject(gameObject);
        Debug.Log("Bubble Killed");
    }
}

