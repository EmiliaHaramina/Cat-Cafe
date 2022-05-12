using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CatGrabAnimation : MonoBehaviour {

    public RuntimeAnimatorController grabAnimatorController;
    public RuntimeAnimatorController letGoAnimatorController;
    public Animator catAnimator;

    public void OnGrabAnimation() {
        catAnimator.runtimeAnimatorController = grabAnimatorController;
        catAnimator.Play("Entry");

        // TODO: Using PhotonNetwork.LocalPlayer maybe i can store "CatXGrab" - "Player" in RoomProperties and then in collision check who grabbed the cat last
        var hash = PhotonNetwork.CurrentRoom.CustomProperties;

        hash.Remove(this.name + "Grab");
        hash.Add(this.name + "Grab", PhotonNetwork.LocalPlayer);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
    }

    public void OnLetGoAnimation() {
        catAnimator.runtimeAnimatorController = letGoAnimatorController;
        catAnimator.Play("Entry");
    }
}
