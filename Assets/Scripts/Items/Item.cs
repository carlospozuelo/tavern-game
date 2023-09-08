using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Item
{


    public Sprite GetSprite();

    public void UseItem();

    public void SelectItem();
    public void CancelSelectItem();

}
