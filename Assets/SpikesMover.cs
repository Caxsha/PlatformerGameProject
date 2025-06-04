using UnityEngine;

public class SpikesMover : MonoBehaviour
{

    public float distance = 1f;
    public float speed = 3f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);
        transform.position = startPos + Vector3.up * offset;
    }
}
