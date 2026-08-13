using System.Collections;
using UnityEngine;

public class SinkRock : MonoBehaviour
{
    public float sinkDelay = 10f;
    public float sinkSpeed = 1f;
    public float sinkDistance = 15f;

    private bool triggered = false;
    private float sinkTargetY;
    private Vector3 originalPosition;
    void Start()
    {
        originalPosition = transform.position;
        sinkTargetY = originalPosition.y - sinkDistance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!triggered&&other.CompareTag("Player"))
        {
            triggered = true;
            Invoke(nameof(StartSink), sinkDelay);
        }
    }
    void StartSink()
    {
        StartCoroutine(Sink());
    }
IEnumerator Sink()
    {
        while (transform.position.y > sinkTargetY)
        {
            transform.position -= new Vector3(0, sinkSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
}
