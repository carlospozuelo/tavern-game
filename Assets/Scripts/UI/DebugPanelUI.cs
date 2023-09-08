using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanelUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private GameObject panel;

    public static DebugPanelUI instance;

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

    public void Debug(string str)
    {
        panel.SetActive(true);
        text.text = str;
    }
}
