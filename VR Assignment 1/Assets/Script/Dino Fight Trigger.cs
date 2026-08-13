using UnityEngine;

public class DinoFightTrigger : MonoBehaviour
{
    public Animator animator;
    public AudioSource sound;
    public string animationName;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
       //Play animation with the name and have the dino look at the user
            animator.SetBool(animationName, true);
            sound.enabled = true;

            Vector3 directionToPlayer = other.transform.position - transform.position;
            directionToPlayer.y = 0f;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = lookRotation;
            }
            sound.enabled = true;
        }
    }
    //Turn off the audio and animation
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool(animationName, false);
            sound.enabled = false;
        }
    }
}
