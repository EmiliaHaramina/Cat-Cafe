using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

    public GameObject soundButton;
    public GameObject soundOnImage;
    public GameObject soundOffImage;

    public void ChangeSound() {
        if (Utility.ChangeSound().Equals("soundOn")) {
            soundOnImage.SetActive(true);
            soundOffImage.SetActive(false);
        } else {
            soundOnImage.SetActive(false);
            soundOffImage.SetActive(true);
        }
    }

}
