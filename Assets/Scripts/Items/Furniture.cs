using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour, Item, IFurniture
{
    [Tooltip("Optional field. If not empty, the furniture can be rotated before placing, and it will be transformed to the referenced gameobject instead")]
    public GameObject rotateGameObject;

    [SerializeField]
    private Vector2Int size = new Vector2Int(1,1);

    [SerializeField]
    public GameObject originalPrefab;

    public FurnitureData GetFurnitureData()
    {
        return new FurnitureData(transform.position - new Vector3(0, 1), originalPrefab.name);
    }

    public Sprite GetSprite()
    {
        return GetComponent<SpriteRenderer>().sprite;
    }

    public bool placedOnWalls = false;
    [Tooltip("Used for rugs or flooring in general. If set to true, the furniture can be placed below items and below the player as well.")]
    public bool rugLike = false;
    [Tooltip("Used for furniture that can have accessories on top")]
    public bool tableLike = false;
    [Tooltip("Used for furniture that HAS TO be placed on top of a tablelike furniture")]
    public bool canBePlacedOnTable = false;

    [SerializeField]
    private List<GameObject> itemsOnTop = new List<GameObject>();
    [SerializeField]
    private Furniture onTopOf = null;

    [SerializeField]
    private List<GameObject> blocks = new List<GameObject>();

    private void OnDrawGizmosSelected()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals("Table surface"))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(child.position + new Vector3(.5f, -.5f), .5f);
            }
        }
    }

    public void Block(GameObject g)
    {
        blocks.Add(g);
    }

    public void Unblock(GameObject g)
    {
        blocks.Remove(g);
    }

    public bool IsBlocked()
    {
        return blocks.Count > 0;
    }

    public bool HasItemsOnTop() { return itemsOnTop.Count > 0;  }

    public void AddItemOnTop(Furniture item) { itemsOnTop.Add(item.gameObject); item.onTopOf = this; }

    public bool UseItem()
    {
        // Place the item on the grid, using the mouse position.
        // Placeholder
        if (!LocationController.GetCurrentLocation().Equals("Tavern")) { return true; }
        Vector3 worldPosition = GameController.instance.WorldPosition(Input.mousePosition);

        if (CanBePlacedOnATable(worldPosition, out Vector3 tablePosition, out Furniture table))
        {
            GameObject g = PlaceItem(tablePosition);
            table.AddItemOnTop(g.GetComponent<Furniture>());

        } else if (CanBePlaced(GridManager.instance.GridPosition(worldPosition)))
        {
            Vector2 pos = GridManager.instance.SnapPosition(worldPosition);
            PlaceItem(pos);
        }

        return true;
    }

    private GameObject PlaceItem(Vector3 p)
    {
        GameObject g = TavernController.InstantiateFurniture(gameObject, p);

        // Consume item from the inventory
        PlayerInventory.instance.ConsumeItem();
        CancelSelectItem();

        return g;
    }

    public void SelectItem()
    {
        if (!LocationController.GetCurrentLocation().Equals("Tavern")) { return; }

        FurniturePreview.instance.EnablePreview(this, transform.localScale);
    }


    public void CancelSelectItem()
    {
        FurniturePreview.instance.DisablePreview();
    }


    // Returns false if the item would overlap with a non-wall, non rug collider
    private bool Boxcast(Vector3Int topLeftTile)
    {
        Vector2 direction = Vector2.up;
        Vector2 origin = new Vector2(topLeftTile.x, topLeftTile.y + 1) + new Vector2(size.x, -size.y) / 2;

        RaycastHit2D[] rays = Physics2D.BoxCastAll(origin, size, 0f, direction, 0f);

        foreach (var ray in rays) { 
            if (!ray.collider.name.Equals("Wall") && !ray.collider.isTrigger && (ray.collider.TryGetComponent(out Furniture f) && !f.rugLike))
            {
                return false;
            }
        }


        return true;
    }

    public Furniture SearchTable()
    {
        if (BoxcastOnTableLike(transform.position, out Vector3 p, out Furniture table))
        {
            return table;
        }

        return null;
    }

    private bool BoxcastOnTableLike(Vector3 mouse, out Vector3 position, out Furniture table)
    {
        Vector2 direction = Vector2.up;
        Vector2 origin = new Vector2(mouse.x, mouse.y);
        table = null;

        RaycastHit2D[] rays = Physics2D.BoxCastAll(origin, size, 0f, direction, 0f);

        foreach (var ray in rays)
        {
            if (ray.collider.gameObject.name.Equals("Table surface"))
            {
                position = ray.collider.transform.position;
                table = ray.collider.GetComponentInParent<Furniture>();
                return true;
            }

        }

        position = Vector3.zero;
        return false;
    }

    // This method will be called if the current position of the furniture is invalid
    public void ReplaceWallFurniture()
    {
        if (placedOnWalls)
        {
            if (!GridManager.instance.IsEntirelyInATilemap(GridManager.instance.GridPosition(transform.position + new Vector3(0, -1)), size, "FurnishableWall")) {
                int attemps = 0;
                Vector3 offset = Vector2.zero;
                while (attemps < 10 && !GridManager.instance.IsEntirelyInATilemap(GridManager.instance.GridPosition(transform.position + offset + new Vector3(0,-1)), size, "FurnishableWall"))
                {
                    offset.y = offset.y + 1;
                    attemps++;
                }
                if (GridManager.instance.IsEntirelyInATilemap(GridManager.instance.GridPosition(transform.position + offset + new Vector3(0, -1)), size, "FurnishableWall")) {
                    transform.position = GridManager.instance.GridPosition(transform.position + offset);
                } else
                {
                    Debug.LogWarning("Item has to be picked up!");
                }
            }

        }
    }

    public bool CanBePlaced(Vector3Int topLeftTile, bool checkCollisions = true)
    {

        if (!TavernController.IsActive()) { return false; }
        if (GameController.instance.DistanceToPlayer(topLeftTile + new Vector3(size.x, -size.y) / 2) >= GameController.instance.maxDistanceToPlaceItems) { return false;  }

        if (placedOnWalls)
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, "FurnishableWall")) { return false; }
        } else
        {
            if (!GridManager.instance.IsEntirelyInATilemap(topLeftTile, size, "Floor")) { return false; }
        }
        
        if (canBePlacedOnTable)
        {
            return false;
           // if (!BoxcastOnTableLike(topLeftTile)) { return false;  }   
        } else if (!rugLike && checkCollisions && TryGetComponent(out Collider2D c) && !Boxcast(topLeftTile)) { return false; } 


        return true;
    }

    public bool CanBePlacedOnATable(Vector3 mouse, out Vector3 position, out Furniture table)
    {
        position = mouse;
        table = null;

        if (canBePlacedOnTable)
        {
            if (BoxcastOnTableLike(mouse, out Vector3 tablePosition, out Furniture t)) {
                position = tablePosition;
                table = t;
                return true;
            } else
            {
                return false;
            }
        }

        return false;
    }

    public bool IsInsideObject(Vector3 worldPosition)
    {
        worldPosition = GridManager.instance.GridPosition(worldPosition);

        float maxX = transform.position.x + size.x;
        float maxY = transform.position.y - size.y;

        return worldPosition.x >= transform.position.x && worldPosition.x < maxX
            && worldPosition.y < transform.position.y && worldPosition.y >= maxY;
    }

    public bool IsPartiallyInsideObject(Vector3 worldPosition)
    {
        worldPosition = GridManager.instance.GridPosition(worldPosition);

        float maxX = transform.position.x + size.x;
        float maxY = transform.position.y - size.y + 1;

        bool resul = worldPosition.x >= transform.position.x && worldPosition.x <= maxX
            && worldPosition.y <= transform.position.y && worldPosition.y >= maxY;

        // Debug.Log(resul + ", " + worldPosition + " " + transform.position);

        return resul;
    }

    public void PickUp()
    {
        if (LocationController.GetCurrentLocation().Equals("Tavern") && isActiveAndEnabled)
        {
            if (GameController.instance.DistanceToPlayer(transform.position + new Vector3(size.x, -size.y) / 2) < GameController.instance.maxDistanceToPlaceItems)
            {
                if (PlayerInventory.instance.GetCurrentItem() == null)
                {
                    PlayerInventory.instance.SetCurrentItem(originalPrefab);
                    TavernController.RemoveFurniture(gameObject);
                    InventoryUI.instance.UpdateSpriteHotbar(this, PlayerInventory.instance.currentItem);
                    if (onTopOf != null)
                    {
                        onTopOf.itemsOnTop.Remove(gameObject);
                    }

                    Destroy(gameObject);
                }
                else if (PlayerInventory.StoreAnywhere(originalPrefab))
                {
                    TavernController.RemoveFurniture(gameObject);

                    if (onTopOf != null)
                    {
                        onTopOf.itemsOnTop.Remove(gameObject);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    public string GetName()
    {
        return originalPrefab.name;
    }

    public GameObject GetOriginalPrefab()
    {
        return originalPrefab;
    }
}
