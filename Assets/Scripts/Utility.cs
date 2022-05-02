using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    private static string sound = "soundOn";
    private static string movement = "teleportation";

    public static GameObject FindChildFromParent(GameObject parent, string name)
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

    public static string GetSound() {
        return sound;
    }

    public static string GetMovement() {
        return movement;
    }

    public static string InitializeSound() {
        if (PlayerPrefs.HasKey("sound")) {
            sound = PlayerPrefs.GetString("sound");
        } else {
            PlayerPrefs.SetString("sound", "soundOn");
        }

        return sound;
    }

    public static string InitializeMovement() {
        if (PlayerPrefs.HasKey("movement")) {
            movement = PlayerPrefs.GetString("movement");
        } else {
            PlayerPrefs.SetString("movement", "teleportation");
        }

        return movement;
    }

    public static string ChangeSound() {
        string newSound = null;

        if (sound.Equals("soundOn")) {
            newSound = "soundOff";
        } else {
            newSound = "soundOn";
        }

        sound = newSound;
        PlayerPrefs.SetString("sound", newSound);
        return newSound;
    }

    public static string ChangeMovement() {
        string newMovement = null;

        if (movement.Equals("teleportation")) {
            newMovement = "continuous";
        } else {
            newMovement = "teleportation";
        }

        movement = newMovement;
        PlayerPrefs.SetString("movement", newMovement);
        return newMovement;
    }

}
