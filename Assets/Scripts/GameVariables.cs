using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVariables {

    private static List<GameObject> catObjects = new List<GameObject>();
    private static bool gameStarted = false;

    public static List<GameObject> GetCatObjects() {
        return catObjects;
    }

    public static void AddCat(GameObject cat) {
        if (!gameStarted) {
            gameStarted = true;
        }

        catObjects.Add(cat);
    }

    public static void RemoveCat(GameObject cat) {
        catObjects.Remove(cat);
    }

}
