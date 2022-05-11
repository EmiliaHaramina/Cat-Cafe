using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;

public class UserTableScript : MonoBehaviour {

    private int points = 0;
    public List<string> cats = new List<string>();
    public int numberOfCats = 8;
    public int rangeOfCats = 125;
    private Random r = new Random();
    private List<GameObject> catObjects = new List<GameObject>();

    public GameObject GetRandomCat() {
        return catObjects[r.Next(0, catObjects.Count)];
    }

    public bool IsVictory() {
        return points == numberOfCats;
    }

    public int GetPoints() {
        return points;
    }

    public void InitializeCats() {
        var hash = PhotonNetwork.CurrentRoom.CustomProperties;

        List<int> randomNumbers = new List<int>();

        GameObject catParent = GameObject.Find("Cats");

        while (randomNumbers.Count < numberOfCats) {
            int rInt = r.Next(1, rangeOfCats);
            Debug.Log(rInt);

            if (!randomNumbers.Contains(rInt))
            {
                randomNumbers.Add(rInt);

                string catName = "Cat" + rInt;
                Debug.Log(catName);
                GameObject cat = Utility.FindChildFromParent(catParent, catName);
                cat.SetActive(true);

                catObjects.Add(cat);
                GameVariables.AddCat(cat);
                hash.Add(catName, "HashCat" + catObjects.Count);
            }
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        string name = collisionObject.name;
        Debug.Log("Hit: " + name);

        if (name.StartsWith("Cat") && !name.StartsWith("CatF") && !cats.Contains(name)) {
            cats.Add(name);
            if (points < 8) {
                points++;
            }

            GameObject body = Utility.FindChildFromParent(collisionObject, "Cat.L");
            Material bodyMaterial = body.GetComponent<Renderer>().material;

            GameObject leftEye = Utility.FindChildFromParent(collisionObject, "Cat.L_Eye.L");
            Material leftEyeMaterial = leftEye.GetComponent<Renderer>().material;

            GameObject rightEye = Utility.FindChildFromParent(collisionObject, "Cat.L_Eye.R");
            Material rightEyeMaterial = rightEye.GetComponent<Renderer>().material;

            collisionObject.SetActive(false);

            GameObject finalCatParent = GameObject.Find("CatsFinalPosition");
            string finalCatName = "CatFinal" + points;

            GameObject finalCat = Utility.FindChildFromParent(finalCatParent, finalCatName);

            GameObject finalBody = Utility.FindChildFromParent(finalCat, "Cat.L");
            finalBody.GetComponent<Renderer>().material = bodyMaterial;

            GameObject finalLeftEye = Utility.FindChildFromParent(finalCat, "Cat.L_Eye.L");
            finalLeftEye.GetComponent<Renderer>().material = leftEyeMaterial;

            GameObject finalRightEye = Utility.FindChildFromParent(finalCat, "Cat.L_Eye.R");
            finalRightEye.GetComponent<Renderer>().material = rightEyeMaterial;

            finalCat.transform.localScale = collisionObject.transform.localScale;

            finalCat.SetActive(true);

            catObjects.Remove(collisionObject);
            GameVariables.RemoveCat(collisionObject);

            var hash = PhotonNetwork.CurrentRoom.CustomProperties;
            hash.Remove(name);
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        if (points == numberOfCats) {
            Debug.Log("Victory!");
        }

        Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());

    }

}
