using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class UserTableScript : MonoBehaviour
{

    public int points = 0;
    public List<string> cats = new List<string>();
    public int numberOfCats = 8;
    public int rangeOfCats = 125;

    // Start is called before the first frame update
    void Start()
    {

        List<int> randomNumbers = new List<int>();

        Random r = new Random();
        GameObject catParent = GameObject.Find("Cats");

        while (randomNumbers.Count < numberOfCats)
        {
            int rInt = r.Next(1, rangeOfCats);
            Debug.Log(rInt);

            if (!randomNumbers.Contains(rInt))
            {
                randomNumbers.Add(rInt);

                string catName = "Cat" + rInt;
                Debug.Log(catName);
                GameObject cat = findChildFromParent(catParent, catName);
                cat.SetActive(true);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        string name = collisionObject.name;
        Debug.Log("Hit: " + name);

        if (name.StartsWith("Cat") && !name.StartsWith("CatF") && !cats.Contains(name))
        {

            cats.Add(name);
            if (points < 8) {
                points++;
            }

            GameObject body = findChildFromParent(collisionObject, "Cat.L");
            Material bodyMaterial = body.GetComponent<Renderer>().material;

            GameObject leftEye = findChildFromParent(collisionObject, "Cat.L_Eye.L");
            Material leftEyeMaterial = leftEye.GetComponent<Renderer>().material;

            GameObject rightEye = findChildFromParent(collisionObject, "Cat.L_Eye.R");
            Material rightEyeMaterial = rightEye.GetComponent<Renderer>().material;

            collisionObject.SetActive(false);

            GameObject finalCatParent = GameObject.Find("CatsFinalPosition");
            string finalCatName = "CatFinal" + points;

            GameObject finalCat = findChildFromParent(finalCatParent, finalCatName);

            GameObject finalBody = findChildFromParent(finalCat, "Cat.L");
            finalBody.GetComponent<Renderer>().material = bodyMaterial;

            GameObject finalLeftEye = findChildFromParent(finalCat, "Cat.L_Eye.L");
            finalLeftEye.GetComponent<Renderer>().material = leftEyeMaterial;

            GameObject finalRightEye = findChildFromParent(finalCat, "Cat.L_Eye.R");
            finalRightEye.GetComponent<Renderer>().material = rightEyeMaterial;

            finalCat.transform.localScale = collisionObject.transform.localScale;

            finalCat.SetActive(true);

        }

        if (points == 8) {
            Debug.Log("Victory!");
        }

    }

    private GameObject findChildFromParent(GameObject parent, string name)
    {

        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in trs)
        {
            if (t.name.Equals(name))
            {
                return t.gameObject;
            }
        }

        return null;

    }

}
