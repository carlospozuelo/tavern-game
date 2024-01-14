using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            
        }
    }

    

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private GameObject droppedItemPrefab;

    public Vector3 WorldPosition(Vector3 p)
    {
        return mainCamera.ScreenToWorldPoint(p);
    }

    public Vector3 WorldMousePosition()
    {
        return WorldPosition(Input.mousePosition);
    }

    public Vector3 ScreenMousePosition()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(BookMenuUI.GetCanvas().GetComponent<RectTransform>(), WorldMousePosition(), mainCamera, out Vector2 localPos)) {
            return localPos;
        }

        return Vector3.zero;
    }

    [SerializeField]
    private GameObject player;

    public float DistanceToPlayer(Vector3 p)
    {
        return Vector2.Distance(p, player.transform.position);
    }

    public float maxDistanceToPlaceItems = 5;

    public static void DropItem(Item toBeDropped)
    {
        DropItem(toBeDropped, PlayerMovement.GetPosition(), true);
    }

    public static void DropItem(Item toBeDropped, Vector3 position, bool block)
    {
        GameObject g = Instantiate(instance.droppedItemPrefab, position, Quaternion.identity, LocationController.GetCurrentLocationDroppable());

        DroppedItem d = g.GetComponent<DroppedItem>();
        d.Initialize(toBeDropped, block);
    }
}
