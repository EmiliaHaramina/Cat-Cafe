using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public GameObject movementText;

    public void ChangeMovement() {

        if (Utility.ChangeMovement().Equals("teleportation")) {
            movementText.GetComponent<TMPro.TextMeshProUGUI>().text = "Movement: Teleportation";
        } else {
            movementText.GetComponent<TMPro.TextMeshProUGUI>().text = "Movement: Continuous";
        }
    }

}