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
    private bool gameStarted = false;
    private int firstPlayerPoints = 0;
    private int secondPlayerPoints = 0;

    public GameObject GetRandomCat() {
        return catObjects[r.Next(0, catObjects.Count)];
    }

    public bool IsVictory() {
        return points == numberOfCats;
    }

    public int GetPoints() {
        return points;
    }

    public bool IsGameStarted() {
        return gameStarted;
    }

    void Update() {

        if (gameStarted) {
            return;
        }

        if (cats.Count != 0) {
            Debug.Log("First player");
            gameStarted = true;
        }

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.Count == numberOfCats) {
            ShowCatsForOtherPlayer();
            Debug.Log("8 cats!");
            gameStarted = true;
        }
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

    public void ShowCatsForOtherPlayer() {
        var hash = PhotonNetwork.CurrentRoom.CustomProperties;

        GameObject catParent = GameObject.Find("Cats");

        foreach (String catName in hash.Keys) {
            GameObject cat = Utility.FindChildFromParent(catParent, catName);
            cat.SetActive(true);

            catObjects.Add(cat);
            GameVariables.AddCat(cat);
        }
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        string name = collisionObject.name;
        Debug.Log("Hit: " + name);

        if (name.StartsWith("Cat") && !name.StartsWith("CatF") && !cats.Contains(name)) {
            cats.Add(name);
            if (points < numberOfCats) {
                if (PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                    points++;
                } else if (PhotonNetwork.CurrentRoom.Name.Equals("Vs")) {
                    var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;

                    //string player = (string) hashtable[name + "Grab"];
                    //Debug.Log(player);
                    string player = (string) hashtable[name + "Grab"];
                    Debug.Log(player);
                    if (player.Equals("#01 \"")) {
                        firstPlayerPoints++;
                    } else {
                        secondPlayerPoints++;
                    }
                    Debug.Log("First player points: " + firstPlayerPoints);
                    Debug.Log("Second player points: " + secondPlayerPoints);
                }
            }

            GameObject body = Utility.FindChildFromParent(collisionObject, "Cat.L");
            Material bodyMaterial = body.GetComponent<Renderer>().material;

            GameObject leftEye = Utility.FindChildFromParent(collisionObject, "Cat.L_Eye.L");
            Material leftEyeMaterial = leftEye.GetComponent<Renderer>().material;

            GameObject rightEye = Utility.FindChildFromParent(collisionObject, "Cat.L_Eye.R");
            Material rightEyeMaterial = rightEye.GetComponent<Renderer>().material;

            collisionObject.SetActive(false);

            GameObject finalCatParent = GameObject.Find("CatsFinalPosition");
            string finalCatName;
            if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                finalCatName = "CatFinal" + points;
            } else {
                finalCatName = "CatFinal" + (firstPlayerPoints + secondPlayerPoints);
            }
            Debug.Log(finalCatName);

            GameObject finalCat = Utility.FindChildFromParent(finalCatParent, finalCatName);

            Debug.Log(finalCat);
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
            hash.Remove(name + "Grab");
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        if (points == numberOfCats) {
            Debug.Log("Victory!");
        }

        if (firstPlayerPoints > numberOfCats / 2) {
            Debug.Log("First player victory!");
        } else if (secondPlayerPoints > numberOfCats / 2) {
            Debug.Log("Second player victory!");
        }

        Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());

    }

}
