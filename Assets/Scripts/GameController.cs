using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public Transform stackableParent;

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
    private GameObject stackableItemGameObjectPrefab;

    public static GameObject GenerateStackableItem(string stackableId, List<Ingredient> ingredients, int stacks = 1)
    {
        GameObject gameObject = Instantiate(instance.stackableItemGameObjectPrefab, Vector3.zero, Quaternion.identity, instance.stackableParent);

        StackableItem stackableItem = gameObject.GetComponent<StackableItem>();

        Ingredient ingredient = CraftingController.GetIngredient(stackableId);

        stackableItem.SetIngredients(ingredients);
        stackableItem.SetValue(ingredient.CalculateValue(ingredients));

        stackableItem.SetIngredient(ingredient);

        stackableItem.SetStacks(stacks);

        gameObject.name = stackableItem.GetName();

        TavernStockController.RegenerateStock();

        return gameObject;
    }

    public static GameObject GenerateStackableItem(string stackableId, int stacks = 1)
    {
        return GenerateStackableItem(stackableId, new List<Ingredient>(), stacks);
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

    public static void DropItem(Ingredient ingredient, bool noise = false)
    {
        GameObject item = GenerateStackableItem(ingredient.ingredientName);

        DropItem(item.GetComponent<Item>(), noise);
    }

    public static void DropItem(Item toBeDropped, bool noise = false)
    {
        Vector3 n = noise ? new Vector3(Random.Range(0f,1f), Random.Range(0f,1f)) : Vector3.zero;
        DropItem(toBeDropped, PlayerMovement.GetPosition() + n, true);
    }

    public static void DropItem(Ingredient ingredient, Vector3 position, bool block) {
        GameObject item = GenerateStackableItem(ingredient.ingredientName);

        DropItem(item.GetComponent<Item>(), position, block);
    }

    public static void DropItem(Item toBeDropped, Vector3 position, bool block)
    {
        GameObject g = Instantiate(instance.droppedItemPrefab, position, Quaternion.identity, LocationController.GetCurrentLocationDroppable());

        DroppedItem d = g.GetComponent<DroppedItem>();
        d.Initialize(toBeDropped, block);
    }
}
