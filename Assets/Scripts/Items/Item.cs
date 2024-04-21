using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item
{


    public Sprite GetSprite();

    public string GetName();
    public string GetDescription();

    public bool UseItem();

    public void SelectItem();
    public void CancelSelectItem();

    public GameObject GetOriginalPrefab();

}
