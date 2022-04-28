using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonScript : MonoBehaviour
{
    public GameObject mainMenu;
    public Utility util;

    void Start()
    {
        util = new Utility();
    }

    public void OpenSettings() {
        GameObject main = util.findChildFromParent(mainMenu, "Main");
        main.SetActive(false);

        GameObject settings = util.findChildFromParent(mainMenu, "Settings");
        settings.SetActive(true);

    }
}
