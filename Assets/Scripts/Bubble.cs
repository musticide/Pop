using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float speed = 2f;
    private BubblePool bubblePool;

    /* public float jitterAmount = 0.2f;
    public float directionChangeInterval = 0.1f; // Time between jitter direction changes
    private Vector3 jitterDirection;
    private float jitterTimer;
    [SerializeField] float halfScreenBoundsWS = 2.5f; */

    void Start()
    {
        bubblePool = FindObjectOfType<BubblePool>();
    }

    private void OnEnable() {
        transform.localScale = Vector3.one * Random.Range(0.7f, 1.3f);
    }

    void Update()
    {
        MoveBubble();

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

    void MoveBubble(){
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        /* jitterTimer += Time.deltaTime;

        // Change jitter direction at fixed intervals
        if (jitterTimer > directionChangeInterval)
        {
            // jitterDirection.x = transform.position.x/halfScreenBoundsWS;
            // jitterDirection.x =  (1 - Mathf.Abs(jitterDirection.x)) * Mathf.Sign(jitterDirection.x);// convert from -1..0..+1 to -0..1..+0  
            jitterDirection.x = Mathf.Sign(transform.position.x) * -1; 
            // jitterDirection.x *= Random.Range(0.0f, 1.0f);

            jitterDirection.x = (Mathf.PerlinNoise1D(Time.time * Random.Range(0,1)) - 0.5f) * 2.0f;

            // jitterDirection = new Vector3(Random.Range(-1, 1), 0,0);
            jitterTimer = 0f;
        }
        jitterDirection.x = (Mathf.PerlinNoise1D(Time.time) - 0.5f) * 2.0f;
        transform.Translate(jitterDirection * jitterAmount * Time.deltaTime); */
    }

    public void KillBubble()
    {
        transform.position = bubblePool.transform.position;
        bubblePool.ReturnObject(gameObject);
        Debug.Log("Bubble Killed");
    }
}

