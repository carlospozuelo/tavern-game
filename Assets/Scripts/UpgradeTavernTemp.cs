using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTavernTemp : MonoBehaviour
{
    public int currentUpgrade;
    public GameObject[] tavernUpgrades;
    public GameObject[] houseUpgrades;

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
        
        if (instance.currentUpgrade < instance.tavernUpgrades.Length)
        {
            TavernController.UpgradeTavern(instance.tavernUpgrades[instance.currentUpgrade]);
        }
        if (instance.currentUpgrade < instance.houseUpgrades.Length)
        {
            TavernController.UpgradeHouse(instance.houseUpgrades[instance.currentUpgrade]);
        }
        instance.currentUpgrade++;
    }
}
