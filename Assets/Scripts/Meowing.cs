using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;

public class Meowing : MonoBehaviour {

    public GameObject userTable;
    public double startingSecondsBetweenMeows = 10;
    private double timer = 0;
    public List<AudioClip> audioClips;
    Random r = new Random();

    void Update() {

        if (!userTable.GetComponent<UserTableScript>().IsGameStarted()) {
            return;
        }

        if (userTable.GetComponent<UserTableScript>().IsVictory() || PlayerPrefs.GetString("sound").Equals("soundOff")) {
            return;
        }

        timer += Time.deltaTime;
        double secondsBetweenMeows = startingSecondsBetweenMeows + Math.Pow(userTable.GetComponent<UserTableScript>().GetPoints(), 2) / 2.0;
        
        if (timer >= secondsBetweenMeows) {

            timer -= secondsBetweenMeows;

            GameObject randomCat = userTable.GetComponent<UserTableScript>().GetRandomCat();

            int next = r.Next(0, audioClips.Count);
            randomCat.GetComponent<AudioSource>().clip = audioClips[next];
            randomCat.GetComponent<AudioSource>().Play();
        }
    }
}
