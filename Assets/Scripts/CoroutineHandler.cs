using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler instance;

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

    private bool isItemCooldown = false;
    private Coroutine itemCooldownCoroutine;
    public static bool IsItemCooldown() { return instance.isItemCooldown; }

    private IEnumerator ItemCD(float time)
    {
        isItemCooldown = true;
        yield return new WaitForSeconds(time);
        isItemCooldown = false;
    }

    private void ItemCDPriv(float time)
    {
        if (IsItemCooldown())
        {
            Debug.LogWarning("Requested to start an item cooldown while the previous had not finished.");
            if (itemCooldownCoroutine != null) { StopCoroutine(itemCooldownCoroutine); }
        }

        StartCoroutine(ItemCD(time));
    }

    public static void ItemCooldown(float time)
    {
        instance.ItemCDPriv(time);
    }
}
