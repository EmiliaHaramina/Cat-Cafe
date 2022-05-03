using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class Meowing : MonoBehaviour {

    public GameObject userTable;
    public double startingSecondsBetweenMeows = 10;
    private double timer = 0;
    public List<AudioClip> audioClips;
    Random r = new Random();

    void Update() {

        if (userTable.GetComponent<UserTableScript>().IsVictory() || PlayerPrefs.GetString("sound").Equals("soundOff")) {
            return;
        }

        timer += Time.deltaTime;
        double secondsBetweenMeows = startingSecondsBetweenMeows + Math.Pow(userTable.GetComponent<UserTableScript>().GetPoints(), 2) / 2.0;
        
        if (timer >= secondsBetweenMeows) {
            timer -= secondsBetweenMeows;
            Debug.Log(secondsBetweenMeows + " seconds");

            GameObject randomCat = userTable.GetComponent<UserTableScript>().GetRandomCat();
            Debug.Log(randomCat);

            int next = r.Next(0, audioClips.Count);
            Debug.Log(next);
            randomCat.GetComponent<AudioSource>().clip = audioClips[next];
            randomCat.GetComponent<AudioSource>().Play();
        }
    }
}
