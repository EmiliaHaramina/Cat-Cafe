using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{

    private bool sound = false;
    private string movement = "teleportation";

    public GameObject findChildFromParent(GameObject parent, string name)
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

    public bool changeSound() {

    }

    public string changeMovement() {

    }

}
