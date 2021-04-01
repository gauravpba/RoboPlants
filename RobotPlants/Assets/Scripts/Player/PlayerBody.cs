using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
public class PlayerBody : NetworkBehaviour
{
    #region Variables

    [Header("Movement Variables")]
    public float maxSpeed = 2f;
    public float jumpForce = 300f;
    public bool facingRight = true;
    public LayerMask jumpableLayers;
    bool canMove = true;
    [HideInInspector] public float moveInput;

    [Header("Ground Checking")]
    public bool isOnGround;
    public bool wasOnGroundLastCheck;

    [Header("Inventory")]
    GameObject heldPickupItemTilePrefab;

    [Header("Interaction")]
    List<PickupItem> interactablesInRange;

    [Header("Item Use")]
    int targetedTileX, targetedTileY;

    [Header("Watering")]
    [SyncVar(hook =nameof(OnWaterLevelChange))]
    float waterAmount = 1f;
    bool canWater = true;
    SpriteMask waterSpriteMask;
    //SpriteRenderer waterSpriteMaskRef;
    GameObject waterSpriteObject;

    [Header("Audio")]
    public List<AudioClip> audioClips;

    [Header("Component References")]
    Animator animator;
    Rigidbody rb;
    SpriteRenderer sr;
    PlayerInteractTrigger playerInteractTrigger;

    [Header("Object/Component References and Prefabs")]
    public GameObject pickupItemPrefab;

    public GameObject playerControllerPrefab;
    [HideInInspector] public PlayerController playerController; //The playercontroller, spawned by the playerBody

    public GameObject playerCameraControllerPrefab;
    [HideInInspector] public PlayerCameraController playerCameraController; //The player's camera

    public GameObject playerUIManagerPrefab;
    [HideInInspector] public PlayerUIManager playerUIManager; //The UI manager component

    public GameObject audioManagerPrefab;
    [HideInInspector] public AudioManager audioManager; //The UI manager component

    #endregion

    #region Unity Methods

    private void Awake()
    {
        waterSpriteMask = GetComponentInChildren<SpriteMask>();

        //Get SpriteRenderers
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < srs.Length; i++)
        {
            if (srs[i].gameObject.name.Equals("PlayerSprite")) sr = srs[i];
            //else if (srs[i].gameObject.name.Equals("WaterSpriteMaskRef")) waterSpriteMaskRef = srs[i];
            else if (srs[i].gameObject.name.Equals("WaterSprite")) waterSpriteObject = srs[i].gameObject;
        }

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInteractTrigger = GetComponentInChildren<PlayerInteractTrigger>();

        interactablesInRange = new List<PickupItem>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        Keyboard kb = Keyboard.current;
        if (kb.lKey.wasPressedThisFrame)
        {
            if (GetGridDataAtMousePosition().environmentTile != null) GetGridDataAtMousePosition().environmentTile.SetColored(true);
        }
    }

    private void OnDestroy()
    {
        DeInitialize();
    }

    private void FixedUpdate()
    {

        GroundCheck();
        //Don't execute if not the local player
        if (!isLocalPlayer) return;
        

        UpdateHorizontalMove();
        ClampOnMaxSpeed();

        CalculateVeritcalVelocity();
    }

    #endregion

    #region Custom Methods

    #region Initialization

    void Initialize()
    {
        //Don't execute if not the local player
        if (!isLocalPlayer) return;

        //If the playerCameraController reference is null, create the player
        if (playerCameraController == null)
        {
            playerCameraController = Instantiate(playerCameraControllerPrefab).GetComponent<PlayerCameraController>();
            playerCameraController.playerBody = this;
            playerCameraController.SetFollowTarget(transform);
        }

        //If the playerUIManager reference is null, create the canvas
        if (playerUIManager == null)
        {
            playerUIManager = Instantiate(playerUIManagerPrefab).GetComponent<PlayerUIManager>();
            playerUIManager.playerBody = this;
            //playerUIManager.playerCamera = playerCameraController.activeCam;
        }

        //If the audioManager reference is null, create the canvas
        if (audioManager == null)
        {
            audioManager = Instantiate(audioManagerPrefab).GetComponent<AudioManager>();
            audioManager.playerBody = this;
        }

        //If the playerBody reference is null, create the player
        if (playerController == null)
        {
            playerController = Instantiate(playerControllerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
            playerController.playerBody = this;
            playerController.playerCameraController = playerCameraController;
            playerController.playerUIManager = playerUIManager;
            //playerController.CallOnEnable(); <--- Replaced by Start()
        }
    }

    public void DeInitialize()
    {
        //If references exist, destroy their objects and nullify their references
        //Specifically destroy the objects in the opposite order of when they were instantiated to prevent null ref errors

        if (playerController != null)
        {
            Destroy(playerController.gameObject);
            playerController = null;
        }

        if (audioManager != null)
        {
            Destroy(audioManager.gameObject);
            audioManager = null;
        }

        if (playerUIManager != null)
        {
            Destroy(playerUIManager.gameObject);
            playerUIManager = null;
        }

        if (playerCameraController != null)
        {
            Destroy(playerCameraController.gameObject);
            playerCameraController = null;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        gameObject.name = "LocalPlayer";
    }


    #endregion

    #region Movement


    public void SetMovementInput(float horizontalInput)
    {
        //Don't execute if not the local player
        if (!isLocalPlayer) return;

        //Update move input
        if (canMove) moveInput = horizontalInput;
        else moveInput = 0f;

        //Update animation state
        animator.SetFloat("horiSpeed", Mathf.Abs(moveInput));
        animator.SetBool("horiMove", (moveInput != 0f));

        //If facing direction changes, switch side
        if (moveInput == 1f)
        {
            if(!facingRight)
            {
                facingRight = true;
                sr.flipX = !facingRight;
                waterSpriteMask.transform.localPosition = Vector3.zero; 
                playerInteractTrigger.SetSide(facingRight);
                if(NetworkServer.localClientActive)
                    CmdProvideFlipStateToServer(sr.flipX,0);
               
            }
        }
        else if (moveInput == -1f)
        {
            if (facingRight)
            {
                facingRight = false;
                sr.flipX = !facingRight;
                waterSpriteMask.transform.localPosition = new Vector3(0.20901f, 0f, 0f); 
                playerInteractTrigger.SetSide(facingRight);
                if (NetworkServer.localClientActive)
                    CmdProvideFlipStateToServer(sr.flipX, 0.20901f);
                
            }
        }
    }

    void EnableMove() { canMove = true; }
    void DisableMove() { canMove = false; }


    [Command]
    void CmdProvideFlipStateToServer(bool state, float x)
    {
        Debug.Log("Test");
        // make the change local on the server
        sr.flipX = state;
        playerInteractTrigger.SetSide(!state);
        if (x == 0)
        {
            waterSpriteMask.transform.localPosition = Vector3.zero;
        }
        else
        {
            waterSpriteMask.transform.localPosition = new Vector3(0.20901f, 0f, 0f);
        }

        // forward the change also to all clients
        RpcSendFlipState(state,x);
    }

    // invoked by the server only but executed on ALL clients
    [ClientRpc]
    void RpcSendFlipState(bool state,float x)
    {
        // skip this function on the LocalPlayer 
        // because he is the one who originally invoked this
        if (isLocalPlayer) return;

        Debug.Log("Test");
        //make the change local on all clients
        sr.flipX = state;
        playerInteractTrigger.SetSide(!state);
        if(x == 0)
        {
            waterSpriteMask.transform.localPosition = Vector3.zero;
        }
        else
        {
            waterSpriteMask.transform.localPosition = new Vector3(0.20901f, 0f, 0f);
        }

    }

    void UpdateHorizontalMove()
    {
        //Moving Right
        if (moveInput == 1f)
        {
            if (rb.velocity.x < maxSpeed)
            {
                rb.AddForce(rb.transform.right * maxSpeed, ForceMode.VelocityChange);
            }
        }

        //Moving Left
        if (moveInput == -1f)
        {
            if (rb.velocity.x > -maxSpeed)
            {
                rb.AddForce(rb.transform.right * -maxSpeed, ForceMode.VelocityChange);
            }
        }
    }

    void CalculateVeritcalVelocity()
    {
        //Debug.Log(rb.velocity);
        animator.SetFloat("vertSpeed", rb.velocity.y);
    }

    void ClampOnMaxSpeed()
    {
        //Not moving right
        if (moveInput != 1f)
        {
            if (rb.velocity.x > 0f)
            {
                rb.AddForce(rb.transform.right * -rb.velocity.x, ForceMode.VelocityChange);
            }
        }

        //Not moving left
        if (moveInput != -1f)
        {
            if (rb.velocity.x < 0f)
            {
                rb.AddForce(rb.transform.right * -rb.velocity.x, ForceMode.VelocityChange);
            }
        }

        //Moving too fast to the right
        if (rb.velocity.x > maxSpeed)
        {
            rb.AddForce(rb.transform.right * -(rb.velocity.x - maxSpeed), ForceMode.VelocityChange);
        }

        //Moving too fast to the left
        if (rb.velocity.x < -maxSpeed)
        {
            rb.AddForce(rb.transform.right * -(rb.velocity.x + maxSpeed), ForceMode.VelocityChange);
        }
    }

    public void Jump()
    {
        //Don't execute if not the local player
        //if (!isLocalPlayer) return;

        if (isOnGround)
        {
            animator.SetTrigger("jump");
            rb.AddForce(rb.transform.up * jumpForce);
        }
    }

    #endregion

    #region Items

    public void SwapItem()
    {
        //Don't execute if not the local player
        //if (!isLocalPlayer) return;

        Debug.Log("SwapItem");
        if (interactablesInRange.Count > 0) GrabItem(interactablesInRange[0]);
        else if (heldPickupItemTilePrefab != null) DropItem();
    }
   
    public void UseItem()
    {
        //Don't execute if not the local player
        //if (!isLocalPlayer) return;

        Debug.Log("UseItem");

        //If we're holding an item, and the targeted Tile is empty
        if (heldPickupItemTilePrefab != null)
        {
            Debug.Log("Held Item Exists");
            //If the environment tile exists and is interactable
            GameManager.GridData gridTarget = GetGridDataAtMousePosition();
            
            if (gridTarget.environmentTile != null && gridTarget.environmentTile.canInteract)
            {
                ////If the tile above this isn't empty, don't allow placing this tile
                //if(!GameManager.instance.GetTileUp(targetedTileX, targetedTileY).IsEmpty())
                //{
                //    //TODO: Play a negative feedback sound----------------------
                //    Debug.Log("NEGATIVE FEEDBACK SOUND");
                //}
                //else
                {
                    Debug.Log("Placing tile " + heldPickupItemTilePrefab.name + " at position " + targetedTileX + ", " + targetedTileY);

                    //Instantiate it at the targetedTilePosition
                    SpawnAndInitTileServer(targetedTileX, targetedTileY, heldPickupItemTilePrefab.name);

                    //Remove item from heldItem slot
                    heldPickupItemTilePrefab = null;
                }
            }
        }

    }

    //Tell Server to instantiate the tile and initialize. Also tell all clients to do the same.
    [Command]
    public void SpawnAndInitTileServer(float x, float y , string name)
    {
        Tile placedTile = Instantiate(FindObjectOfType<NetManager>().spawnPrefabs.Find(prefab => prefab.name == name)
            , new Vector3(x + 0.5f, y + 0.5f, 0f), Quaternion.identity).GetComponent<Tile>();
        placedTile.Init();

        //Notify clients of new server object (make sure spawned object is in NetManager spawnPrefabs
       

        NetworkServer.Spawn(placedTile.gameObject);
    }

    /*
    public void SpawnAndInitTileClients()
    {
        Tile placedTile = Instantiate(heldPickupItemTilePrefab, new Vector3(targetedTileX + 0.5f, targetedTileY + 0.5f, 0f), Quaternion.identity).GetComponent<Tile>();

        placedTile.Init();
    }*/

    public void SetTargetedTile(int x, int y)
    {
        targetedTileX = x;
        targetedTileY = y;
    }

    public void RemoveItem()
    {
        //Don't execute if not the local player
        //if (!isLocalPlayer) return;

        Debug.Log("RemoveItem");

        //Get grid tile
        GameManager.GridData gridTarget = GetGridDataAtMousePosition();

        //If this is a valid grid position, and there's a plant present
        if (gridTarget != null && gridTarget.objectTile != null)
        {
            //If the objectTile is a Plant object
            if (gridTarget.objectTile is Plant)
            {
                //Water the plant
                ((Plant)gridTarget.objectTile).RemoveWithItemDrop();
            }
        }
    }

    #endregion

    #region Actions

    public void Water()
    {
        //Don't execute if not the local player
        //if (!isLocalPlayer) return;

        //Don't water if in the air or out of water or currently watering
        if (!isOnGround || waterAmount <= 0f || !canWater) return;

        //Disable movement during watering
        DisableMove();
        DisableWater();

        Debug.Log("Water");

        //For a 2 wide by 4 tall group of grid tiles in front of the player, attempt to water any plants in those tiles
        for (int x = 0; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                //Get grid tile
                GameManager.GridData gridTarget = GameManager.instance.GetGridDataAtLocation(
                    (int)(Mathf.Round(transform.position.x) + 0.5f + ((facingRight ? (x) : (-x - 1)))),
                    (int)(Mathf.Floor(transform.position.y) + y + 0.5f));

                //If this is a valid grid position, and there's a plant present
                if (gridTarget != null && gridTarget.objectTile != null)
                {
                    //If the objectTile is a Plant object
                    if (gridTarget.objectTile is Plant)
                    {
                        //Water the plant

                        CmdwaterPlant(x, y , facingRight);
                        ((Plant)gridTarget.objectTile).WaterPlant();
                    }
                }
            }
        }

        //Track remaining water
        DecreaseWaterAmount();

        //Play water SFX (audioClips[2])
        audioManager.PlaySFX(audioClips[2], true);

        InstantiateWater(facingRight);

/*
        InstantiateWater
        GameObject waterParticle = Instantiate(FindObjectOfType<NetManager>().spawnPrefabs.Find(prefab => prefab.name == "WaterParticle"), transform.position + (Vector3.right * (facingRight ? 1 : -1)), Quaternion.identity);
        waterParticle.transform.rotation = Quaternion.Euler(new Vector3(0f, (facingRight ? 0f : 180f), 0f));
        NetworkServer.Spawn(waterParticle.gameObject);
*/
        //Reenable movement and watering after one second
        Invoke("EnableMove", 1f);
        Invoke("EnableWater", 1f);
    }

    void EnableWater() { canWater = true; }
    void DisableWater() { canWater = false; }
    int count;
    [Command]
    void CmdwaterPlant(float x, float y , bool facingRight)
    {
        GameManager.GridData gridTarget = GameManager.instance.GetGridDataAtLocation(
                    (int)(Mathf.Round(transform.position.x) + 0.5f + ((facingRight ? (x) : (-x - 1)))),
                    (int)(Mathf.Floor(transform.position.y) + y + 0.5f));

        Debug.Log(gridTarget.objectTile +" " +  count++) ;
        ((Plant)gridTarget.objectTile).WaterPlant();
        WaterPlantClients(x, y, facingRight);
    }

    [ClientRpc]
    public void WaterPlantClients(float x, float y, bool facingRight)
    {
        if (isLocalPlayer) return;
        GameManager.GridData gridTarget = GameManager.instance.GetGridDataAtLocation(
                   (int)(Mathf.Round(transform.position.x) + 0.5f + ((facingRight ? (x) : (-x - 1)))),
                   (int)(Mathf.Floor(transform.position.y) + y + 0.5f));
        
        Debug.Log("Watered");
        ((Plant)gridTarget.objectTile).WaterPlant();
    }

    [Command]
    void InstantiateWater(bool isFacingRight)
    {
        //Play water particle system
        GameObject waterParticle = Instantiate(FindObjectOfType<NetManager>().spawnPrefabs.Find(prefab => prefab.name == "WaterParticle"), transform.position + (Vector3.right * (isFacingRight ? 1 : -1)), Quaternion.identity);
        waterParticle.transform.rotation = Quaternion.Euler(new Vector3(0f, (isFacingRight ? 0f : 180f), 0f));
        NetworkServer.Spawn(waterParticle.gameObject);

    }

    [Command]
    void DecreaseWaterAmount()
    {
        //Decrease water by one quarter, bottoming out at 0f and update water level visual
        waterAmount -= 0.25f;
        UpdateWaterSprite(waterAmount);
    }


    [ClientRpc]
    void UpdateWaterSprite(float waterAmount)
    {
       
        float yLevel = 1f;
        switch(waterAmount)
        {
            default:
            case 1f:
                yLevel = 0f;
                break;

            case 0.75f:
                yLevel = -1.15f;
                break;

            case 0.5f:
                yLevel = -1.45f;
                break;

            case 0.25f:
                yLevel = -1.75f;
                break;

            case 0f:
                yLevel = -2.5f;
                break;
        }

        Debug.Log("Setting Y Level to: " + yLevel);
        waterSpriteObject.transform.localPosition = new Vector3(0f, yLevel, 0f);
        //updateWaterSprite(yLevel);


    }

    
    void OnWaterLevelChange(float oldCount, float newCount)
    {
        if (isLocalPlayer) return;

        float yLevel = 1f;
        switch (newCount)
        {
            default:
            case 1f:
                yLevel = 0f;
                break;

            case 0.75f:
                yLevel = -1.15f;
                break;

            case 0.5f:
                yLevel = -1.45f;
                break;

            case 0.25f:
                yLevel = -1.75f;
                break;

            case 0f:
                yLevel = -2.5f;
                break;
        }

        waterSpriteObject.transform.localPosition = new Vector3(0f, yLevel, 0f);
    }

    /*
    [Command]
    void updateWaterSprite(float yLevel)
    {
        
        DecreaseWaterAmountClients(yLevel);
    }

    [ClientRpc]
    public void DecreaseWaterAmountClients(float yLevel)
    {        
        waterSpriteObject.transform.localPosition = new Vector3(0f, yLevel, 0f);
    }
    */

    public void RefillWater()
    {
        waterAmount = 1f;
        waterSpriteObject.transform.localPosition = Vector3.zero;

    }

/*    public void RefillWaterClient()
    {
        waterAmount = 1f;
        waterSpriteObject.transform.localPosition = Vector3.zero;
    }
*/
    ////Update the water sprite mask to the matching mask sprite for the animation
    //public void SetWaterSpriteMask()
    //{
    //    waterSpriteMask.sprite = waterSpriteMaskRef.sprite;
    //}

    GameManager.GridData GetGridDataAtMousePosition()
    {
        return GameManager.instance.GetGridDataAtLocation(targetedTileX, targetedTileY);
    }

    #endregion

    #region Inventory

    void GrabItem(PickupItem toGrab)
    {
        //If already holding an item, drop it first
        if (heldPickupItemTilePrefab != null) DropItem();

        //Add item to heldItem slot
        heldPickupItemTilePrefab = toGrab.tilePrefab;

        //Remove PickupItem from interactables list
        interactablesInRange.Remove(toGrab);

        //Destroy the pickup item if it exists in the scene, and not from a reference
        Destroy(toGrab.gameObject);
    }

    public void DropItem()
    {
        //Spawn the item in the world
        SpawnItem(pickupItemPrefab);

        //Remove item from heldItem slot
        heldPickupItemTilePrefab = null;
    }

    public void SpawnItem(GameObject itemPrefab)
    {
        //Instantiate dropped item to world
        PickupItem pi = Instantiate(itemPrefab, transform.position + (transform.right * (facingRight ? 2f : -2f)), Quaternion.identity).GetComponent<PickupItem>();
        pi.Initialize(heldPickupItemTilePrefab);
    }

    #endregion

    #region Interaction

    public void AddInteractable(PickupItem toAdd)
    {
        if (!interactablesInRange.Contains(toAdd)) interactablesInRange.Add(toAdd);
    }

    public void RemoveInteractable(PickupItem toRemove)
    {
        interactablesInRange.Remove(toRemove);
    }

    #endregion

    #region Audio

    public void Footstep()
    {
        //Play footstep sfx (random play either audioClips[0] or audioClips[1]
       // audioManager.PlaySFX(audioClips[Random.Range(0, 2)], true);
    }

    #endregion

    #region Ground Checking

    void GroundCheck()
    {
        //Use a raycast to check for ground
        List<Collider> hits = new List<Collider>();
        hits.AddRange(Physics.OverlapSphere(transform.position + (Vector3.down * 1.5f), 0.1f, jumpableLayers));
        hits.AddRange(Physics.OverlapSphere(transform.position + (Vector3.down * 1.5f) + (Vector3.right * 0.85f), 0.1f, jumpableLayers));
        hits.AddRange(Physics.OverlapSphere(transform.position + (Vector3.down * 1.5f) - (Vector3.right * 0.85f), 0.1f, jumpableLayers));
        isOnGround = (hits.Count > 0);

        //If you weren't on the ground but now you are, you've grounded
        if (isOnGround && !wasOnGroundLastCheck) OnGrounded();
        //If you were on the ground but now you aren't, you've jumped
        else if (!isOnGround && wasOnGroundLastCheck) OnAirborne();

        wasOnGroundLastCheck = isOnGround;
    }

    void OnAirborne()
    {
        //Debug.Log("Airborne");
        animator.SetBool("isGrounded", isOnGround);

        rb.useGravity = true;
    }

    void OnGrounded()
    {
        //Debug.Log("Grounded");
        animator.SetBool("isGrounded", isOnGround);

        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    #endregion

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 1.5f), 0.1f);
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 1.5f) + (Vector3.right * 0.85f), 0.1f);
        Gizmos.DrawWireSphere(transform.position + (Vector3.down * 1.5f) - (Vector3.right * 0.85f), 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Floor(transform.position.y) + 0.5f, 0f), Vector3.one);

        Gizmos.color = Color.white;
        for (int x = 0; x < 2; x++)
        {
            for (int y = -2; y < 2; y++)
            {
                Gizmos.DrawWireCube(new Vector3(Mathf.Round(transform.position.x) + 0.5f + ((facingRight ? (x) : (-x - 1))), Mathf.Floor(transform.position.y) + y + 0.5f, 0f), Vector3.one);
            }
        }
    }
#endif
}
