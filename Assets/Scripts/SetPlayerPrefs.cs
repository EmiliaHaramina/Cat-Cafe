using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour {

    void Start()
    {
        Debug.Log(PlayerPrefs.HasKey("sound"));
        Debug.Log(PlayerPrefs.GetString("sound"));
        Debug.Log(PlayerPrefs.HasKey("movement"));
        Debug.Log(PlayerPrefs.GetString("movement"));

        Utility.InitializeSound();
        Utility.InitializeMovement();

        Debug.Log(PlayerPrefs.HasKey("sound"));
        Debug.Log(PlayerPrefs.GetString("sound"));
        Debug.Log(PlayerPrefs.HasKey("movement"));
        Debug.Log(PlayerPrefs.GetString("movement"));
    }
}
