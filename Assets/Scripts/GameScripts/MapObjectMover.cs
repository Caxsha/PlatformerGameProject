using UnityEngine;

public class MapObjectMover : MonoBehaviour
{
    public enum MoveType { Linear, Circular }

    [Header("Тип движения")]
    public MoveType movementType = MoveType.Linear;

    [Header("Общие настройки движения")]
    public bool enableMovement = true;
    public float moveStartDelay = 0f;
    public float moveDuration = 0f;
    public float movePause = 0f;

    [Header("Линейное движение")]
    public Vector3 moveDirection = Vector3.up;
    public float moveDistance = 2f;
    public float moveSpeed = 1f;
    public bool useEasing = true;

    [Header("Круговое движение")]
    public Vector3 circleCenterOffset = Vector3.zero;
    public float circleRadius = 2f;
    public float circleSpeed = 90f;
    public Vector3 circleAxis = Vector3.up;

    [Header("Вращение")]
    public bool enableRotation = false;
    public Vector3 rotationAxis = Vector3.forward;
    public float rotationSpeed = 90f;
    public float rotationStartDelay = 0f;
    public float rotationDuration = 0f;
    public float rotationPause = 0f;

    private Vector3 startPosition;
    private float moveStartTime;
    private float rotationStartTime;

    void Start()
    {
        startPosition = transform.position;
        moveStartTime = Time.time + moveStartDelay;
        rotationStartTime = Time.time + rotationStartDelay;
    }

    void Update()
    {
        if (enableMovement)
            HandleMovement();

        if (enableRotation)
            HandleRotation();
    }

    void HandleMovement()
    {
        if (Time.time < moveStartTime) return;

        if (moveDuration > 0f)
        {
            float elapsed = Time.time - moveStartTime;
            float cycle = moveDuration + movePause;
            if ((elapsed % cycle) > moveDuration) return;
        }

        switch (movementType)
        {
            case MoveType.Linear:
                DoLinearMovement();
                break;
            case MoveType.Circular:
                DoCircularMovement();
                break;
        }
    }

    void DoLinearMovement()
    {
        float t = Mathf.PingPong((Time.time - moveStartTime) * moveSpeed, 1f);
        if (useEasing)
            t = Mathf.SmoothStep(0f, 1f, t);

        Vector3 offset = moveDirection.normalized * moveDistance * t;
        transform.position = startPosition + offset;
    }

    void DoCircularMovement()
    {
        Vector3 center = startPosition + circleCenterOffset;
        float angle = (Time.time - moveStartTime) * circleSpeed;
        Quaternion rotation = Quaternion.AngleAxis(angle, circleAxis.normalized);
        Vector3 offset = rotation * (Vector3.forward * circleRadius);
        transform.position = center + offset;
    }

    void HandleRotation()
    {
        if (Time.time < rotationStartTime) return;

        if (rotationDuration > 0f)
        {
            float elapsed = Time.time - rotationStartTime;
            float cycle = rotationDuration + rotationPause;
            if ((elapsed % cycle) > rotationDuration) return;
        }

        transform.Rotate(rotationAxis.normalized * rotationSpeed * Time.deltaTime);
    }
}
