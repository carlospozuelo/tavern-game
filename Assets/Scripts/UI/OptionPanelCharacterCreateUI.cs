using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanelCharacterCreateUI : MonoBehaviour
{
    public List<ClothingItem> items;

    public Button[] colorButtons;

    public ClothingItem.ClothingType type;
    public TMPro.TextMeshProUGUI text;

    public int selected = 0;

    private void Start()
    {
        // While on development, load ALL options into the items list.
        // In the future, this should be changed so that it only loads the more "basic" customization options.

        items = ClothingController.GetAll(type);
        Select();
    }

    public void Next()
    {
        selected++;
        if (selected >= items.Count) { selected = 0; }

        Select();
    }

    public void Previous()
    {
        selected--;
        if (selected <= 0) { selected = items.Count - 1; }

        Select();
    }

    private void Select()
    {
        ClothingController.Wear(items[selected]);
        text.text = StringType() + "\n" + selected;
    }

    private string StringType()
    {
        if (type == ClothingItem.ClothingType.Legs) return "Pants";
        if (type == ClothingItem.ClothingType.Torso) return "Shirt";
        if (type == ClothingItem.ClothingType.Faces) return "Face";

        return type.ToString();
    }
}
