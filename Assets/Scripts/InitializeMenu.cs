using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeMenu : MonoBehaviour {

    public GameObject soundOnImage;
    public GameObject soundOffImage;
    public GameObject movementText;

    void Start() {
        if (Utility.InitializeSound().Equals("soundOff")) {
            soundOnImage.SetActive(false);
            soundOffImage.SetActive(true);
        }

        if (Utility.InitializeMovement().Equals("continuous")) {
            movementText.GetComponent<TMPro.TextMeshProUGUI>().text = "Movement: Continuous";
        }
    }

}
