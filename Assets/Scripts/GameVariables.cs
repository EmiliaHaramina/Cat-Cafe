using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVariables {

    private static List<GameObject> catObjects = new List<GameObject>();

    public static List<GameObject> GetCatObjects() {
        return catObjects;
    }

}
