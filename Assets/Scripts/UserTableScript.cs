using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    public GameObject guide;
    private Vector3 guidePosition;
    private Quaternion guideRotation;

    public RuntimeAnimatorController talkingAnimatorController;
    public RuntimeAnimatorController idleAnimatorController;
    public RuntimeAnimatorController pointAnimatorController;
    public RuntimeAnimatorController victoryAnimatorController;
    private Animator guideAnimator;

    public double timeForPoint = 5;
    private bool gotPoint = false;
    private double pointTimer = 0;

    public GameObject welcomeCoop;
    public GameObject welcomeVs;
    public GameObject pointsCoop;
    public GameObject pointsVs;
    public GameObject victoryCoop;
    public GameObject victoryVs;

    public bool coop;

    public GameObject GetRandomCat() {
        return catObjects[r.Next(0, catObjects.Count)];
    }

    public bool IsVictory() {
        return points == numberOfCats || firstPlayerPoints > numberOfCats / 2 || secondPlayerPoints > numberOfCats / 2;
     }

    public int GetPoints() {
        return points + firstPlayerPoints + secondPlayerPoints;
    }

    public bool IsGameStarted() {
        return gameStarted;
    }

    void Start() {

        guidePosition = guide.transform.position;
        guideRotation = guide.transform.rotation;

        guideAnimator = guide.GetComponent<Animator>();
        guideAnimator.runtimeAnimatorController = talkingAnimatorController;

        if (!PhotonNetwork.IsConnected) {
            InitializeCats();
            gameStarted = true;
        }
    }

    void Update() {

        if (gotPoint) {
            pointTimer += Time.deltaTime;
            Debug.Log("Point timer: " + pointTimer);
            Debug.Log("Time for point: " + timeForPoint);

            if (pointTimer >= timeForPoint) {
                gotPoint = false;
                pointTimer = 0;
                guideAnimator.runtimeAnimatorController = idleAnimatorController;
            }
        }

        guide.transform.position = guidePosition;

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
        Hashtable hash = null;
        if (PhotonNetwork.IsConnected) {
            hash = PhotonNetwork.CurrentRoom.CustomProperties;
        }

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
                if (PhotonNetwork.IsConnected) {
                    hash.Add(catName, "HashCat" + catObjects.Count);
                }
            }
        }

        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        }

        guideAnimator.runtimeAnimatorController = idleAnimatorController;
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

        guideAnimator.runtimeAnimatorController = idleAnimatorController;
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        string name = collisionObject.name;
        Debug.Log("Hit: " + name);

        if (name.StartsWith("Cat") && !name.StartsWith("CatF") && !cats.Contains(name)) {
            cats.Add(name);
            if (points < numberOfCats) {
                if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                    points++;
                } else if (PhotonNetwork.CurrentRoom.Name.Equals("Vs")) {
                    var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;

                    int player = (int) hashtable[name + "Grab"];
                    Debug.Log(player);
                    if (player == 1) {
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
            if (!PhotonNetwork.IsConnected|| PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
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

            if (PhotonNetwork.IsConnected) {
                var hash = PhotonNetwork.CurrentRoom.CustomProperties;
                hash.Remove(name);
                hash.Remove(name + "Grab");
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }

            if (!IsVictory()) {
                guide.transform.rotation = guideRotation;
                guideAnimator.runtimeAnimatorController = pointAnimatorController;
                gotPoint = true;
                pointTimer = 0;
            }
        }

        if (points == numberOfCats) {
            Debug.Log("Victory!");
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;
        }

        if (firstPlayerPoints > numberOfCats / 2) {
            Debug.Log("First player victory!");
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;
        } else if (secondPlayerPoints > numberOfCats / 2) {
            Debug.Log("Second player victory!");
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;
        }

        if (PhotonNetwork.IsConnected) {
            Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        }

    }

}
