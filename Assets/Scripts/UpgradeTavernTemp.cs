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
        if (instance.currentUpgrade < instance.tavernUpgrades.Count)
        {
            TavernController.UpgradeTavern(instance.tavernUpgrades[instance.currentUpgrade].list);
        }
        instance.currentUpgrade++;
    }
}
