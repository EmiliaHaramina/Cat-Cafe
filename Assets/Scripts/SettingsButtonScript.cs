using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonScript : MonoBehaviour
{
    public GameObject mainMenu;

    public void OpenSettings() {
        GameObject main = Utility.FindChildFromParent(mainMenu, "Main");
        main.SetActive(false);

        GameObject settings = Utility.FindChildFromParent(mainMenu, "Settings");
        settings.SetActive(true);
    }

    public void CloseSettings() {
        GameObject main = Utility.FindChildFromParent(mainMenu, "Main");
        main.SetActive(true);

        GameObject settings = Utility.FindChildFromParent(mainMenu, "Settings");
        settings.SetActive(false);
    }
}
