using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class HelicopterBehaviour : MonoBehaviour
{
    public Transform landingPosition;
    public float landingSpeed = 5f;
    private bool landed = false;
    public Animator animator;
    public AudioSource audio;
    // Indicates that the helicopter is ready to land
    public void BeginLanding()
    {
        landed = true;
        StartCoroutine(HelicopterDelay());
    }

    void Update()
    {
        // If the BeginLanding method is called, move the helicopter downward until it reaches the landing position then switch of audio and animations
        if(landed)
        {
            transform.position = Vector3.MoveTowards(transform.position, landingPosition.position, landingSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position,landingPosition.position)<0.1f)
            {
                animator.enabled = false;
                audio.enabled = false;
                landed = false;
            }
        }
    }
    // Enables the sound and audio after a delay
    IEnumerator HelicopterDelay()
    {
        yield return new WaitForSeconds(2f);
        animator.enabled = true;
        audio.enabled = true;
    }
}
