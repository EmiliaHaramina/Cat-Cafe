using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InitializeGame : MonoBehaviour {

    void Start() {

        Debug.Log(Utility.GetMovement());

        if (Utility.GetMovement().Equals("continuous")) {
            GetComponent<TeleportationProvider>().enabled = false;
            GetComponent<ActionBasedSnapTurnProvider>().enabled = false;
            GetComponent<ActionBasedContinuousMoveProvider>().enabled = true;
            GetComponent<ActionBasedContinuousTurnProvider>().enabled = true;
            Debug.Log("Bye");
        }
    }
}
