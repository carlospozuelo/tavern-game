using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTavernTemp : MonoBehaviour
{
    [Serializable]
    public class GameObjectList
    {
        public List<GameObject> list = new List<GameObject>();

        public GameObject tavernExterior;
    }

    public int currentUpgrade;
    public List<GameObjectList> tavernUpgrades;


    private static UpgradeTavernTemp instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }
    public static void Upgrade()
    {
        if (instance.currentUpgrade < instance.tavernUpgrades.Count -1)
        {
            instance.currentUpgrade++;
        }
        else { instance.currentUpgrade = 0; }

        TavernController.UpgradeTavern(instance.tavernUpgrades[instance.currentUpgrade].list);

        TownController.UpgradeTavern(instance.tavernUpgrades[instance.currentUpgrade].tavernExterior);
    }
}
