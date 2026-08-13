using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class DinoSound : MonoBehaviour
{
    public Animator animator;
    public AudioSource audio;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //switch on the dino's animation and sound
            audio.enabled = true;
            animator.enabled = true;
            
            // The player looks in the diraction of the player
            Vector3 directionToPlayer = other.transform.position - transform.position;
            directionToPlayer.y = 0f;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = lookRotation;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           StartCoroutine(DisableAfterCurrentAnimation());
        }
    }
    //Displays the entire animation then switches off the animation and audio
    private IEnumerator DisableAfterCurrentAnimation()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            float clipLength = clipInfo[0].clip.length;
            yield return new WaitForSeconds(clipLength);
        }

        
        animator.enabled = false;
        audio.enabled = false;
    }
}
