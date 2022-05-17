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

        if (PhotonNetwork.IsConnected) {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;

            hash.Remove(this.name + "Grab");
            hash.Add(this.name + "Grab", PhotonNetwork.LocalPlayer.ActorNumber);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    public void OnLetGoAnimation() {
        catAnimator.runtimeAnimatorController = letGoAnimatorController;
        catAnimator.Play("Entry");
    }
}
