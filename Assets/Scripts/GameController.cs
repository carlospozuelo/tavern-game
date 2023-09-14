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

    public Vector3 WorldPosition(Vector3 p)
    {
        return mainCamera.ScreenToWorldPoint(p);
    }

    public Vector3 WorldMousePosition()
    {
        return WorldPosition(Input.mousePosition);
    }

    [SerializeField]
    private GameObject player;

    public float DistanceToPlayer(Vector3 p)
    {
        return Vector3.Distance(p, player.transform.position);
    }

    public float maxDistanceToPlaceItems = 5;
}
