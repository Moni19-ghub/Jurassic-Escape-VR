using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scanner : MonoBehaviour
{
    private HashSet<GameObject> scannedDinos = new HashSet<GameObject>();
    public TextMeshProUGUI countText;
    public HelicopterBehaviour heliController;
    public bool heliTriggered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dino"))
        {
            //If all the dino has not been scanned update the text on the screen 
            if (!scannedDinos.Contains(other.gameObject))
            {
                scannedDinos.Add(other.gameObject);
                countText.text = $"Scanned: {scannedDinos.Count} / 6";

                //If all 6 are scanned trigger the helicopter and update the text
                if(scannedDinos.Count==6&&!heliTriggered)
                {
                    heliTriggered = true;
                    heliController.BeginLanding();
                    countText.text = "Well Done. You have scanned all the dinosaurs. Now run to the helicopter.";
                }
            }
        }        
    }
}
