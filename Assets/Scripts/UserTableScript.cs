using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

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

    public GameObject welcomeSingleplayer;
    public GameObject welcomeCoop;
    public GameObject welcomeVs;
    public GameObject waiting;
    public GameObject pointsCoop;
    public GameObject pointsVs;
    public GameObject victoryCoop;
    public GameObject victoryVs;

    public bool multiplayerInit = false;

    private List<string> catNames = new List<string>();

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

        if (numberOfCats == 0) {
            numberOfCats = 1;
        }

        guidePosition = guide.transform.position;
        guideRotation = guide.transform.rotation;

        guideAnimator = guide.GetComponent<Animator>();
        guideAnimator.runtimeAnimatorController = talkingAnimatorController;

        if (!PhotonNetwork.IsConnected) {
            GuideInit();
        }
    }

    void Update() {

        if (gotPoint) {
            pointTimer += Time.deltaTime;

            if (pointTimer >= timeForPoint) {
                gotPoint = false;
                pointTimer = 0;
                guideAnimator.runtimeAnimatorController = idleAnimatorController;
            }
        }

        guide.transform.position = guidePosition;

        if (PhotonNetwork.CurrentRoom != null
            && ((PhotonNetwork.CurrentRoom.Name.Equals("coop") && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("points") && points != (int) PhotonNetwork.CurrentRoom.CustomProperties["points"])
            || PhotonNetwork.CurrentRoom.Name.Equals("Vs") && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("firstplayerPoints") && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("secondPlayerPoints")
            && (firstPlayerPoints != (int) PhotonNetwork.CurrentRoom.CustomProperties["firstPlayerPoints"] || secondPlayerPoints != (int) PhotonNetwork.CurrentRoom.CustomProperties["secondPlayerPoints"]))) {
            Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;

            foreach (String catName in catNames) {
                if (!hash.ContainsKey(catName)) {
                    Debug.Log("In update: " + catName);
                    Debug.Log("Points on player: " + points + ", points on network: " + PhotonNetwork.CurrentRoom.CustomProperties["points"]);
                    GameObject cats = GameObject.Find("Cats");
                    CatCollision(Utility.FindChildFromParent(cats, catName));
                    if (PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                        UpdatePoints();
                    } else {
                        UpdateBothPlayerPoints();
                    }
                    CheckVictory();
                }
            }
        }

        if (gameStarted) {
            return;
        }

        if (multiplayerInit && PhotonNetwork.CurrentRoom != null) {
            Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
            if (PhotonNetwork.CurrentRoom.Name.Equals("Coop") && hash.Count == 0) {
                welcomeCoop.SetActive(false);
                waiting.SetActive(true);
                hash.Add("firstPlayer", "ready");
            } else if (PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                welcomeCoop.SetActive(false);
                pointsCoop.SetActive(true);
                InitializeCats();
                gameStarted = true;
                hash.Remove("firstPlayer");
                hash.Add("secondPlayer", "ready");
            } else if (PhotonNetwork.CurrentRoom.Name.Equals("Vs") && hash.Count == 0) {
                welcomeVs.SetActive(false);
                waiting.SetActive(true);
                hash.Add("firstPlayer", "ready");
            } else if (PhotonNetwork.CurrentRoom.Name.Equals("Vs")) {
                welcomeVs.SetActive(false);
                pointsVs.SetActive(true);
                InitializeCats();
                gameStarted = true;
                hash.Remove("firstPlayer");
                hash.Add("secondPlayer", "ready");
            }

            multiplayerInit = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("secondPlayer")) {
            ShowCatsForOtherPlayer();
            gameStarted = true;

            waiting.SetActive(false);

            if (PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                pointsCoop.SetActive(true);
            } else {
                pointsVs.SetActive(true);
            }

            Hashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
            hash.Remove("secondPlayer");
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    public void SingleplayerInit() {
        InitializeCats();
        gameStarted = true;
        welcomeSingleplayer.SetActive(false);
        pointsCoop.SetActive(true);
    }

    public void MultiplayerInit() {
        multiplayerInit = true;
    }

    public void GuideInit() {
        if (!PhotonNetwork.IsConnected) {
            welcomeSingleplayer.SetActive(true);
        } else if (PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
            welcomeCoop.SetActive(true);
        } else {
            welcomeVs.SetActive(true);
        }

        guideAnimator.runtimeAnimatorController = talkingAnimatorController;
    }

    public void InitializeCats() {
        Hashtable hash = null;
        if (PhotonNetwork.IsConnected) {
            hash = PhotonNetwork.CurrentRoom.CustomProperties;
            hash.Add("firstPlayerPoints", 0);
            hash.Add("secondPlayerPoint", 0);
            hash.Add("points", 0);
        }

        List<int> randomNumbers = new List<int>();

        GameObject catParent = GameObject.Find("Cats");

        while (randomNumbers.Count < numberOfCats) {
            int rInt = r.Next(1, rangeOfCats);

            if (!randomNumbers.Contains(rInt))
            {
                randomNumbers.Add(rInt);

                string catName = "Cat" + rInt;
                GameObject cat = Utility.FindChildFromParent(catParent, catName);
                cat.SetActive(true);

                catObjects.Add(cat);
                if (PhotonNetwork.IsConnected) {
                    catNames.Add(catName);
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
        Debug.Log(hash);

        GameObject catParent = GameObject.Find("Cats");

        foreach (String catName in hash.Keys) {
            if (!catName.StartsWith("Cat")) {
                continue;
            }
            GameObject cat = Utility.FindChildFromParent(catParent, catName);
            cat.SetActive(true);

            catObjects.Add(cat);
            catNames.Add(catName);
        }

        guideAnimator.runtimeAnimatorController = idleAnimatorController;
    }

    void OnCollisionEnter(Collision collision) {
        GameObject collisionObject = collision.gameObject;
        string name = collisionObject.name;

        if (name.StartsWith("Cat") && !name.StartsWith("CatF") && !cats.Contains(name)) {
            Debug.Log("In collision: " + name);
            CatCollision(collisionObject);
        }

        CheckVictory();
    }

    private void CheckVictory() {
        if (points == numberOfCats) {
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;

            pointsCoop.SetActive(false);
            victoryCoop.SetActive(true);

            GameObject victoryText = Utility.FindChildFromParent(victoryCoop, "VictoryCoopText");
            if (points == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "Thank you! You found our cat!";
            } else {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "Thank you! You found all " + points + " of our cats!";
            }
        }

        if (firstPlayerPoints > numberOfCats / 2) {
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;

            pointsVs.SetActive(false);
            victoryVs.SetActive(true);

            GameObject victoryText = Utility.FindChildFromParent(victoryVs, "VictoryVsText");
            if (firstPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + firstPlayerPoints + " cat!\n\nThe other player has found " + secondPlayerPoints + " cats!";
            } else if (firstPlayerPoints == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player has found most of the cats!\n\nYou have found " + secondPlayerPoints + " cats!\n\nThe other player has found " + firstPlayerPoints + " cat!";
            } else if (secondPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + firstPlayerPoints + " cats!\n\nThe other player has found " + secondPlayerPoints + " cat!";
            } else if (secondPlayerPoints == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player has found most of the cats!\n\nYou have found " + secondPlayerPoints + " cat!\n\nThe other player has found " + firstPlayerPoints + " cats!";
            } else if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + firstPlayerPoints + " cats!\n\nThe other player has found " + secondPlayerPoints + " cats!";
            } else {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player has found most of the cats!\n\nYou have found " + secondPlayerPoints + " cats!\n\nThe other player has found " + firstPlayerPoints + " cats!";
            }
        } else if (secondPlayerPoints > numberOfCats / 2) {
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;

            pointsVs.SetActive(false);
            victoryVs.SetActive(true);

            GameObject victoryText = Utility.FindChildFromParent(victoryVs, "VictoryVsText");
            if (firstPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 2) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + secondPlayerPoints + " cats!\n\nThe other player found " + firstPlayerPoints + " cat!";
            } else if (firstPlayerPoints == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player found most of the cats!\n\nYou have found " + firstPlayerPoints + " cat!\n\nThe other player found " + secondPlayerPoints + " cats!";
            } else if (secondPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 2) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + secondPlayerPoints + " cat!\n\nThe other player has found " + firstPlayerPoints + " cats!";
            } else if (secondPlayerPoints == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player has found most of the cats!\n\nYou have found " + firstPlayerPoints + " cats!\n\nThe other player has found " + secondPlayerPoints + " cat!";
            } else if (PhotonNetwork.LocalPlayer.ActorNumber == 2) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found most of the cats!\n\nYou have found " + secondPlayerPoints + " cats!\n\nThe other player found " + firstPlayerPoints + " cats!";
            } else {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "The other player found most of the cats!\n\nYou have found " + firstPlayerPoints + " cats!\n\nThe other player found " + secondPlayerPoints + " cats!";
            }
        }  else if (firstPlayerPoints + secondPlayerPoints == numberOfCats) {
            guideAnimator.runtimeAnimatorController = victoryAnimatorController;

            pointsVs.SetActive(false);
            victoryVs.SetActive(true);

            GameObject victoryText = Utility.FindChildFromParent(victoryVs, "VictoryVsText");
            if (firstPlayerPoints == 1) {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "It's a draw, I guess you have to sit together!\n\n\nBoth of you found " + firstPlayerPoints + " cat!";
            } else {
                victoryText.GetComponent<TMPro.TextMeshProUGUI>().text = "It's a draw, I guess you have to sit together!\n\n\nBoth of you found " + firstPlayerPoints + " cats!";
            }
        }

        if (PhotonNetwork.IsConnected) {
            Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        }
    }

    private void UpdatePoints() {
        GameObject pointText = Utility.FindChildFromParent(pointsCoop, "PointsCoopText");
        if (points == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + points + " cat so far!";
        } else {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + points + " cats so far!";
        }
    }

    private void UpdateBothPlayerPoints() {
        GameObject pointText = Utility.FindChildFromParent(pointsVs, "PointsVsText");
        if (firstPlayerPoints == 1 && secondPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + firstPlayerPoints + " cat so far!\n\nThe other player has found " + secondPlayerPoints + " cat so far!";
        } else if (firstPlayerPoints == 1 && secondPlayerPoints == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + secondPlayerPoints + " cat so far!\n\nThe other player has found " + firstPlayerPoints + " cat so far!";
        } else if (firstPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + firstPlayerPoints + " cat so far!\n\nThe other player has found " + secondPlayerPoints + " cats so far!";
        } else if (firstPlayerPoints == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + secondPlayerPoints + " cat so far!\n\nThe other player has found " + firstPlayerPoints + " cats so far!";
        } else if (secondPlayerPoints == 1 && PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + firstPlayerPoints + " cats so far!\n\nThe other player has found " + secondPlayerPoints + " cat so far!";
        } else if (secondPlayerPoints == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + secondPlayerPoints + " cats so far!\n\nThe other player has found " + firstPlayerPoints + " cat so far!";
        } else if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + firstPlayerPoints + " cats so far!\n\nThe other player has found " + secondPlayerPoints + " cats so far!";
        } else {
            pointText.GetComponent<TMPro.TextMeshProUGUI>().text = "You have found " + secondPlayerPoints + " cats so far!\n\nThe other player has found " + firstPlayerPoints + " cats so far!";
        }
    }

    private void CatCollision(GameObject collisionObject) {
        string name = collisionObject.name;
        cats.Add(name);
        if (points < numberOfCats && firstPlayerPoints + secondPlayerPoints < numberOfCats) {
            if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
                points++;
                UpdatePoints();

                if (PhotonNetwork.IsConnected) {
                    var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
                    hashtable.Remove("points");
                    hashtable.Add("points", points);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
                }
            } else if (PhotonNetwork.CurrentRoom.Name.Equals("Vs")) {
                var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;

                int player = (int)hashtable[name + "Grab"];
                if (player == 1) {
                    firstPlayerPoints++;
                    hashtable.Remove("firstPlayerPoints");
                    hashtable.Add("firstPlayerPoints", firstPlayerPoints);
                } else {
                    secondPlayerPoints++;
                    hashtable.Remove("secondPlayerPoints");
                    hashtable.Add("secondPlayerPoints", secondPlayerPoints);
                }

                UpdateBothPlayerPoints();
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }
        }

        if (PhotonNetwork.IsConnected) {
            var hash = PhotonNetwork.CurrentRoom.CustomProperties;
            catNames.Remove(name);
            hash.Remove(name);
            hash.Remove(name + "Grab");
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
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
        if (!PhotonNetwork.IsConnected || PhotonNetwork.CurrentRoom.Name.Equals("Coop")) {
            finalCatName = "CatFinal" + points;
        } else {
            finalCatName = "CatFinal" + (firstPlayerPoints + secondPlayerPoints);
        }

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

        if (!IsVictory()) {
            guide.transform.rotation = guideRotation;
            guideAnimator.runtimeAnimatorController = pointAnimatorController;
            gotPoint = true;
            pointTimer = 0;
        }
    }

    public void BackToMainMenu() {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Start Menu");
    }

}
