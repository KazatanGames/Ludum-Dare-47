using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE = null;
    
    [Header("Game Objects")]
    [SerializeField]
    protected GameObject player; public GameObject Player { get { return player; } }
    [SerializeField]
    protected Camera mainCamera;
    [SerializeField]
    protected GameObject cauldronIcon;
    [Header("Game Setup")]
    [SerializeField]
    protected GameSetup config;
    [Header("Tiles")]
    [SerializeField]
    protected GameObject grassTilePrefab;
    [Header("Other Prefabs")]
    [SerializeField]
    protected GameObject homePrefab;
    [SerializeField]
    protected GameObject frogPrefab;
    [SerializeField]
    protected GameObject dragonPrefab;
    [SerializeField]
    protected GameObject shieldPrefab;
    [Header("UI")]
    [SerializeField]
    protected Text uiFrogs;
    [SerializeField]
    protected Text uiDragonScales;
    [SerializeField]
    protected Animator uiBookAnimator;

    protected Vector2 playerSpeed;
    protected float camFov;
    Vector2Int playerTile;
    protected Inventory inv;
    protected bool bookOpen = false;
    protected float greenPotionTime = 0f;
    protected float redPotionTime = 0f;
    protected int shieldLife = 0;

    protected Dictionary<Vector2Int, TileInstance> tiles;

    protected List<Frog> frogs;
    protected List<Dragon> dragons;

    protected GameObject shield;

    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else if (INSTANCE != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (player == null)
        {
            throw new UnassignedReferenceException("player must be set on GameManager");
        }

        if (mainCamera == null)
        {
            throw new UnassignedReferenceException("mainCamera must be set on GameManager");
        }

        Initialize();
    }

    protected void Initialize()
    {
        playerSpeed = new Vector2();
        playerTile = Vector2Int.zero;
        camFov = config.cameraMinFov;
        tiles = new Dictionary<Vector2Int, TileInstance>();
        frogs = new List<Frog>();
        dragons = new List<Dragon>();
        inv = new Inventory();
        inv.frogs = 30;
        inv.dragonScales = 30;

        Instantiate(homePrefab, Vector3.zero, Quaternion.identity);

        shield = Instantiate(shieldPrefab, Vector3.zero, Quaternion.identity);
        shield.SetActive(false);

        cauldronIcon.SetActive(false);

        GenerateWorld();
    }

    private void Update()
    {
        UpdatePotions();
        GetInputs();
        MovePlayer();
        UpdateShield();
        UpdateUI();
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    protected void UpdatePotions()
    {
        if (greenPotionTime > 0f)
        {
            greenPotionTime -= Time.deltaTime;
        }
        if (greenPotionTime > 0f)
        {
            redPotionTime -= Time.deltaTime;
        }
    }

    protected void GetInputs()
    {
        float targetCamFov = config.cameraMinFov;

        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputVector.Normalize();

        if (inputVector.magnitude == 0)
        {
            // slow the player to a stop
            playerSpeed = Vector2.Lerp(playerSpeed, Vector2.zero, Time.deltaTime * config.playerDeccel);
        } else
        {
            float accel = greenPotionTime > 0 ? config.playerAccel * 5f : config.playerAccel;

            // increase the speed to the max
            playerSpeed = Vector2.Lerp(playerSpeed, inputVector * config.playerMaxSpeed, Time.deltaTime * accel);

            // also lerp the targetCamFov to max
            targetCamFov = Mathf.Lerp(config.cameraMinFov, config.cameraMaxFov, playerSpeed.magnitude * (greenPotionTime > 0 ? 2f : 1f) / config.playerMaxSpeed);
        }


        Vector3 eulerAngles = player.transform.eulerAngles;
        eulerAngles.y = Mathf.Atan2(playerSpeed.x, playerSpeed.y) * (180 / Mathf.PI);
        player.transform.eulerAngles = eulerAngles;

        // also lerp the camfov to 0
        camFov = Mathf.Lerp(camFov, targetCamFov, Time.deltaTime);

    }

    protected void MovePlayer()
    {
        Vector3 pos = player.transform.position;
        pos.x += playerSpeed.x;
        pos.z += playerSpeed.y;
        pos.y = Mathf.Lerp(config.playerMinHeight, config.playerMaxHeight, playerSpeed.magnitude / config.playerMaxSpeed);
        player.transform.position = pos;

        if (pos.magnitude <= config.cauldronDistance)
        {
            if (!bookOpen)
            {
                uiBookAnimator.SetTrigger("OpenSpellBook");
                bookOpen = true;
            }

            cauldronIcon.SetActive(false);
        } else if (bookOpen)
        {
            uiBookAnimator.SetTrigger("CloseSpellBook");
            bookOpen = false;

            cauldronIcon.SetActive(true);
        }

        if (cauldronIcon.activeSelf)
        {
            cauldronIcon.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            cauldronIcon.transform.LookAt(Vector3.zero);
            cauldronIcon.transform.Translate(cauldronIcon.transform.forward * 0.75f, Space.World);
            cauldronIcon.transform.eulerAngles = Vector3.zero;
            cauldronIcon.transform.Translate(cauldronIcon.transform.up * player.transform.position.y);
        }

        Vector2Int newPlayerTile = new Vector2Int(Mathf.FloorToInt(player.transform.position.x / config.tileWidth), Mathf.FloorToInt(player.transform.position.z / config.tileWidth));
        if (newPlayerTile != playerTile)
        {
            Vector2Int diff = newPlayerTile - playerTile;
            playerTile = newPlayerTile;
            GenerateMoreWorld(diff.x, diff.y);
        }

        List<Frog> deadFrogs = new List<Frog>();
        List<Frog> farFrogs = new List<Frog>();
        foreach (Frog f in frogs)
        {
            float fd = Vector2.Distance(new Vector2(f.transform.position.x, f.transform.position.z), new Vector2(pos.x, pos.z));
            if (fd < config.captureDistance)
            {
                deadFrogs.Add(f);
            } else if (fd > config.despawnDistance)
            {
                farFrogs.Add(f);
            }
        }
        foreach(Frog f in deadFrogs)
        {
            frogs.Remove(f);
            Destroy(f.gameObject);
            inv.frogs++;
        }
        foreach (Frog f in farFrogs)
        {
            frogs.Remove(f);
            Destroy(f.gameObject);
        }

        List<Dragon> deadDragons = new List<Dragon>();
        List<Dragon> farDragons = new List<Dragon>();
        foreach (Dragon d in dragons)
        {
            float fd = Vector2.Distance(new Vector2(d.transform.position.x, d.transform.position.z), new Vector2(pos.x, pos.z));
            if (fd < config.captureDistance)
            {
                deadDragons.Add(d);
            }
            else if (fd > config.despawnDistance)
            {
                farDragons.Add(d);
            }
        }
        foreach (Dragon d in deadDragons)
        {
            dragons.Remove(d);
            Destroy(d.gameObject);
            inv.dragonScales++;
        }
        foreach (Dragon d in farDragons)
        {
            dragons.Remove(d);
            Destroy(d.gameObject);
        }
    }

    protected void MoveCamera()
    {
        mainCamera.transform.position = new Vector3(player.transform.position.x, mainCamera.transform.position.y, player.transform.position.z - 0.5f);
        mainCamera.fieldOfView = camFov;
    }

    protected void GenerateWorld()
    {
        int startX = playerTile.x - config.tileGenerationDistance;
        int endX = playerTile.x + config.tileGenerationDistance + 1;
        int startY = playerTile.y - config.tileGenerationDistance;
        int endY = playerTile.y + config.tileGenerationDistance + 1;

        for (int xx = startX; xx < endX; xx++)
        {
            for (int yy = startY; yy < endY; yy++)
            {
                CheckNewTile(xx, yy, false);
            }
        }
    }

    protected void GenerateMoreWorld(int x, int y)
    {
        int dx = x > 0 ? 1 : -1;
        int dy = y > 0 ? 1 : -1;

        while (x != 0)
        {
            GenerateWorldX(dx);
            x -= dx;
        }
        while (y != 0)
        {
            GenerateWorldY(dy);
            y -= dy;
        }
    }

    protected void GenerateWorldX(int dx)
    {
        int xx = dx > 0 ? playerTile.x + config.tileGenerationDistance : playerTile.x - config.tileGenerationDistance;

        for (int yy = playerTile.y - config.tileGenerationDistance; yy < playerTile.y + config.tileGenerationDistance + 1; yy++)
        {
            CheckNewTile(xx, yy, true);
        }
    }

    protected void GenerateWorldY(int dy)
    {
        int yy = dy > 0 ? playerTile.y + config.tileGenerationDistance : playerTile.y - config.tileGenerationDistance;

        for (int xx = playerTile.x - config.tileGenerationDistance; xx < playerTile.x + config.tileGenerationDistance + 1; xx++)
        {
            CheckNewTile(xx, yy, true);
        }
    }

    protected void UpdateUI()
    {
        uiFrogs.text = inv.frogs.ToString();
        uiDragonScales.text = inv.dragonScales.ToString();
    }

    protected void CheckNewTile(int x, int y, bool spawn)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (!tiles.ContainsKey(pos))
        {
            NewTile(pos);
        }

        if (spawn && Vector2Int.Distance(pos, Vector2Int.zero) > config.homeDistance)
        {
            if (frogs.Count < config.targetFrogs && Random.value <= config.frogChance)
            {
                SpawnFrog(pos);
            } else if (dragons.Count < config.targetDragons && Random.value <= config.dragonChance)
            {
                SpawnDragon(pos);
            }
        }
    }

    protected void NewTile(Vector2Int pos)
    {
        GameObject tilePrefab = Instantiate(grassTilePrefab, new Vector3(pos.x * config.tileWidth, 0f, pos.y * config.tileWidth), Quaternion.identity);
        tilePrefab.transform.localScale = new Vector3(config.tileWidth, 1f, config.tileWidth);
        tiles.Add(pos, new TileInstance(pos, tilePrefab, new TileData()));
    }

    protected void SpawnFrog(Vector2Int tilePos)
    {
        frogs.Add(
            Instantiate(frogPrefab, new Vector3(tilePos.x * config.tileWidth, 0f, tilePos.y * config.tileWidth), Quaternion.identity).GetComponent<Frog>()
        );
    }

    protected void SpawnDragon(Vector2Int tilePos)
    {
        dragons.Add(
            Instantiate(dragonPrefab, new Vector3(tilePos.x * config.tileWidth, 0f, tilePos.y * config.tileWidth), Quaternion.identity).GetComponent<Dragon>()
        );
    }

    protected void UpdateShield()
    {
        shield.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.2f, player.transform.position.z);

        if (shieldLife > 0)
        {
            shield.SetActive(true);
        } else
        {
            shield.SetActive(false);
        }
    }


    public void MixGreen()
    {
        if (inv.frogs < 10) return;

        inv.frogs -= 10;
        greenPotionTime = config.greenPotionTime;
    }

    public void MixRed()
    {
        if (inv.dragonScales < 5) return;

        inv.dragonScales -= 5;
        redPotionTime = config.redPotionTime;
        shieldLife = 3;
    }
}
