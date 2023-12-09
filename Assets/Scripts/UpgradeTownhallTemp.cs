using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeTownhallTemp : MonoBehaviour
{
    [Serializable]
    public class GameObjectList
    {
        public GameObject thInterior;

        public GameObject thExterior;
    }

    public List<GameObjectList> townhalls = new List<GameObjectList>();
    public int current;

    public static UpgradeTownhallTemp instance;

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
        if (instance.current < instance.townhalls.Count - 1)
        {
           
            instance.current++;
        } else
        {
            instance.current = 0;
        }

        //TavernController.UpgradeTavern(instance.tavernUpgrades[instance.currentUpgrade].list);

        TownController.UpgradeTH(instance.townhalls[instance.current].thExterior);

    }

}
