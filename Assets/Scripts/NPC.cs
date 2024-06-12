using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : StaticNPC, TimeSubscriber
{

    private string location;

    public enum NPC_Status {
        JUST_SPAWNED,
        WALKING_TOWARDS_SEAT,
        SITTING,
        WAITING_FOR_ORDER,
        LEAVING
    }

    [SerializeField]
    private NPC_Status status;


    public void SetLocation(string location) { this.location = location; }
    public string GetLocation() { return location; }

    [SerializeField]
    private float speed = 2;

    [SerializeField]
    private GameObject textBubble;

    [SerializeField]
    private Image desireImage;

    [SerializeField]
    private TextMeshProUGUI desireText;

    [SerializeField]
    private Animator bubbleAnimator;

    [SerializeField]
    private Gradient waitingGradient;

    [SerializeField]
    private Image loadBar;

    [SerializeField]
    private int waitTicksForOrders = 5;

    [SerializeField]
    private Sprite happySprite, sadSprite;

    public override void Initialize(Dictionary<ClothingItem.ClothingType, ClothingItem> clothes) {
        if (initialized) return;
        base.Initialize(clothes);
        Initialize();

        status = NPC_Status.JUST_SPAWNED;
        loadBar.fillAmount = 1;

        if (isActiveAndEnabled)
        {
            coroutine = StartCoroutine(Exist());
        }

        initialized = true;
    }
    private Coroutine coroutine;

    private void OnEnable()
    {
        if (initialized)
        {
            switch (status)
            {
                case NPC_Status.JUST_SPAWNED:
                    bench?.SoftGetUp();
                    coroutine = StartCoroutine(Exist());
                    break;
                case NPC_Status.WALKING_TOWARDS_SEAT:
                    bench?.SoftGetUp();
                    coroutine = StartCoroutine(Exist());
                    break;
                case NPC_Status.SITTING:
                    bench?.SoftGetUp();
                    coroutine = StartCoroutine(Sit(bench));
                    break;
                case NPC_Status.WAITING_FOR_ORDER:
                    base.Sit(transform.position, bench);
                    coroutine = StartCoroutine(ExistSittingAfterDecission());
                    break;
                case NPC_Status.LEAVING:
                    coroutine = StartCoroutine(Leave());
                    break;

            }
        }
    }

    

    private void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = null;

       
    }


    private void Destroy()
    {
        TimeController.Unsubscribe(this);
        NPCController.DestroyNPC(this);

        StopCoroutine(coroutine);
        coroutine = null;

        gameObject.SetActive(false);
        NPCController.AddPooledNPC(gameObject);
        initialized = false;
    }

    private IEnumerator Exist()
    {
        existingSitted = false;
        decidedToOrder = false;


        status = NPC_Status.JUST_SPAWNED;
        while (true)
        {
            if (this.bench != null)
            {
                yield return WalkTowardsBench(this.bench);
            }
            else
            {
                bench = NPCController.PopRandomBench();
                yield return WalkTowardsBench(bench);
                if (!sitting)
                {
                    // Something went wrong. Reinstall the bench
                    Stop();
                }
            }

            if (NPCController.BenchesAvailable() <= 0)
            {
                // Nothing else to do -> Leave
                Stop();
                yield return Leave();
            }
        }
       
    }

    private IEnumerator WalkTowardsBench(Bench bench)
    {
        status = NPC_Status.WALKING_TOWARDS_SEAT;
        if (bench == null)
        {
            yield break;
        }
        Vector3 benchPosition = bench.GetPosition();
        Pathfinding pathfinding = LocationController.GetPathfindingAgent(location).GetPathfinding();
        List<PathNode> path = pathfinding.GetPathToClosestReachableTile(transform.position, benchPosition);
        
        if (path == null) { yield break; }


        foreach (PathNode node in path)
        {
            if (bench == null || path == null || bench.IsBusy())
            {
                // This check is done in case the bench is picked up mid-routine. 
                Stop();
                this.bench = null;
                yield break;
            }
            
            Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
            //transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            yield return MoveRoutine(worldPos);
        }
        // Reached the bench
        yield return Sit(bench);
        
    }

    private IEnumerator MoveRoutine(Vector3 endPoint)
    {
        Vector2 direction = (endPoint - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, endPoint);
        int failures = 0;
        while (distance > .15f && failures < 10)
        {
            if (Vector2.Distance(transform.position, endPoint) >= distance)
            {
                // Distance increased !!
                direction = (endPoint - transform.position).normalized;
                failures++;
            }

            distance = Vector2.Distance(transform.position, endPoint);

            animator.SetFloat("AnimMoveX", direction.normalized.x);
            animator.SetFloat("AnimMoveY", direction.normalized.y);
            animator.SetFloat("MovementMagnitude", direction.magnitude);
            rb.velocity = direction * speed;
            yield return new WaitForSeconds(.05f);
        }

        if (failures >= 10) {
            // Corrected position !!
            Debug.LogWarning("Corrected Position !! ");
            rb.position = endPoint;
        }
    }

    private IEnumerator Leave()
    {
        if (bench != null)
        {
            bench.GetUp(gameObject, 0, -1);
            base.GetUp(0, -1);
        }
        animator.SetBool("Sitting", false);
        sitting = false;

        status = NPC_Status.LEAVING;
        yield return SimpleWalkTowards(Portal.GetPortal("Tavern entrance").GetPosition());

        TimeController.Unsubscribe(this);
        // Despawn (Placeholder. They should "cross" the portal")
        Destroy();
    }

    private string EXIST_SITTING = "ExistSitting";
    private IEnumerator ExistSitting()
    {
        TimeController.Subscribe(this, EXIST_SITTING, 1, 1, true);
        existingSitted = true;
        yield return new WaitUntil(() => !existingSitted);

        loadBar.fillAmount = 1;
        loadBar.color = waitingGradient.Evaluate(loadBar.fillAmount);

        yield return ExistSittingAfterDecission();
        
    }

    private IEnumerator ExistSittingAfterDecission()
    {
        if (decidedToOrder)
        {
            yield return Order();
        }

        status = NPC_Status.LEAVING;
        // Get up and leave.
        yield return new WaitForSecondsRealtime(2f);
        yield return Leave();
    }

    private bool existingSitted = false;
    private bool decidedToOrder = false;

    private void ExistSittingNotify()
    {
        // A chance to do:
        // 20% Nothing
        // 80% Order something
        float n = Random.Range(0f, 1f);

        if (n < .8f)
        {
            decidedToOrder = true;
            existingSitted = false;
        }
    }

    private IEnumerator Sit(Bench bench)
    {
        status = NPC_Status.SITTING;
        Stop();
        if (bench == null) { yield break; }
        sitting = false;
        if (bench.Interact(this)) {
            yield return ExistSitting();
        } else
        {
            this.bench = null;
        }

        yield break;
    }

    private Ingredient desire;

    private IEnumerator Order()
    {
        status = NPC_Status.WAITING_FOR_ORDER;
        desire = TavernStockController.GetRandomIngredient();

        if (desire == null )
        {
            // No stock
            yield break;
        }
        desireImage.sprite = desire.sprite;
        desireImage.gameObject.SetActive(true);
        
        bubbleAnimator.SetTrigger("Open");
        bubbleAnimator.SetTrigger("Open Load");

        TimeController.SubscribeIfNotAlready(this, CANCEL_ORDER, waitTicksForOrders, 1, false);
        TimeController.SubscribeIfNotAlready(this, UPDATE_WAITING_TIME, 1, waitTicksForOrders, false);


        // loadBar.fillAmount = 1;
        yield return new WaitUntil(() => desire == null);
        bubbleAnimator.SetTrigger("Close Load");
    }


    private IEnumerator SimpleWalkTowards(Vector3 position)
    {
        Pathfinding pathfinding = LocationController.GetPathfindingAgent(location).GetPathfinding();
        List<PathNode> path = pathfinding.GetPathToClosestReachableTile(transform.position, position);

        if (path == null)
        {
            yield break;
        }

        foreach (PathNode node in path)
        {
            if (path == null)
            {
                // This check is done in case the bench is picked up mid-routine. 
                yield break;
            }

            Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
            //transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            yield return MoveRoutine(worldPos);
        }
    }

    public void Interact(GameObject itemHeld)
    {
        // If the npc wants something, try and take it.
        if (desire != null)
        {
            if (itemHeld != null && itemHeld.TryGetComponent(out StackableItem stackable))
            {
                if (stackable.GetIngredient().Equals(desire)) {
                    Satisfy(stackable);
                }
            }
        }
        // Otherwise, start a dialog
    }

    private void Satisfy(StackableItem item)
    {
        // Cancel desire
        desire = null;
        bubbleAnimator.SetTrigger("Close");

        // Give MONEY
        PlayerInventory.ModifyGold(item.GetBasePrice());
        // Reduce stacks
        item.Consume();
        InventoryUI.instance.UpdateUI();
        TimeController.Unsubscribe(this, CANCEL_ORDER);
        TimeController.Unsubscribe(this, UPDATE_WAITING_TIME);
        // Set happy animation
        bubbleAnimator.SetTrigger("Bounce");
        desireImage.sprite = happySprite;
    }
    private string CANCEL_ORDER = "Cancel order", UPDATE_WAITING_TIME = "Update waiting time";
    public void Notify(string text)
    {
        if (text.Equals(CANCEL_ORDER))
        {
            if (gameObject.activeSelf)
            {
                desire = null;
                bubbleAnimator.SetTrigger("Close");
                // Set sad animation
                bubbleAnimator.SetTrigger("Bounce");
                desireImage.sprite = sadSprite;
            } else
            {
                status = NPC_Status.LEAVING;
            }
        } else if (text.Equals(UPDATE_WAITING_TIME))
        {
            if (loadBar != null)
            {
                loadBar.fillAmount -= (1f / waitTicksForOrders);
                loadBar.color = waitingGradient.Evaluate(loadBar.fillAmount);
            }
        } else if (text.Equals(EXIST_SITTING))
        {
            ExistSittingNotify();
        }
    }
}
