using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class End : MonoBehaviour
{

    public TextMeshProUGUI countText;

    //Update the on screen text
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Helicopter"))
        {
            countText.text = $"Game Over";
        }

    }
}
