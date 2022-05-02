using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meowing : MonoBehaviour {

    public GameObject userTable;
    public double secondsBetweenMeows = 10;
    private double timer = 0;

    void Update() {
        timer += Time.deltaTime;
        
        if (timer >= secondsBetweenMeows) {
            timer -= secondsBetweenMeows;
            Debug.Log(secondsBetweenMeows + " seconds");

            GameObject randomCat = userTable.GetComponent<UserTableScript>().GetRandomCat();
            Debug.Log(randomCat);
            randomCat.GetComponent<AudioSource>().Play();
        }
    }
}
