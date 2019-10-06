using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField]
    protected GameObject arrowIcon;
    [SerializeField]
    protected TrailRenderer playerTrail;
    [SerializeField]
    protected GameObject playerFlying;
    [SerializeField]
    protected GameObject playerStanding;
    [SerializeField]
    protected MusicManager musicManager;
    [Header("Game Setup")]
    [SerializeField]
    protected GameSetup config;
    [Header("Tiles")]
    [SerializeField]
    protected GameObject grassTilePrefab;
    [SerializeField]
    protected GameObject bushTilePrefab;
    [Header("Other Prefabs")]
    [SerializeField]
    protected GameObject homePrefab;
    [SerializeField]
    protected GameObject frogPrefab;
    [SerializeField]
    protected GameObject dragonPrefab;
    [SerializeField]
    protected GameObject heliPrefab;
    [SerializeField]
    protected GameObject shieldPrefab;
    [SerializeField]
    protected GameObject[] treePrefabs;
    [SerializeField]
    protected GameObject[] rockPrefabs;
    [SerializeField]
    protected GameObject winShowerPrefab;
    [Header("UI")]
    [SerializeField]
    protected Text uiFrogs;
    [SerializeField]
    protected Text uiDragonScales;
    [SerializeField]
    protected Animator uiBookAnimator;
    [SerializeField]
    protected RectTransform uiGreenPotion;
    [SerializeField]
    protected RectTransform uiRedPotion;
    [Header("Audio")]
    [SerializeField]
    protected AudioSource audioMusic;
    [SerializeField]
    protected AudioSource audioSfx;
    [Header("Audio Clips")]
    [SerializeField]
    protected AudioClip sfxFrogPickup;
    [SerializeField]
    protected AudioClip sfxDragonPickup;
    [SerializeField]
    protected AudioClip sfxDrink;
    [SerializeField]
    protected AudioClip sfxGood;
    [SerializeField]
    protected AudioClip sfxGood2;
    [SerializeField]
    protected AudioClip sfxShieldDown;

    protected Vector2 playerSpeed;
    protected float camFov;
    Vector2Int playerTile;
    protected Inventory inv;
    protected bool bookOpen = false;
    protected float greenPotionTime = 0f;
    protected float redPotionTime = 0f;
    protected bool shieldLife = false;
    protected float treeOffset = 1f;
    protected bool dying = false; public bool Alive { get { return !dying; } }
    protected float dieTime = 0f;
    protected AudioSource playerAudio;
    protected float camOffset;
    protected bool pFlying;
    protected bool killedHeli;
    protected float killedHeliTime;

    protected Dictionary<Vector2Int, TileInstance> tiles;

    protected List<Frog> frogs;
    protected List<Dragon> dragons;
    protected Heli heli;

    protected GameObject shield;
    protected Home home;

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
        treeOffset = config.tileWidth / 2.5f;
        playerSpeed = new Vector2();
        playerTile = Vector2Int.zero;
        camFov = config.cameraMinFov;
        tiles = new Dictionary<Vector2Int, TileInstance>();
        frogs = new List<Frog>();
        dragons = new List<Dragon>();
        inv = new Inventory();
        inv.frogs = 0;
        inv.dragonScales = 6;
        playerAudio = player.GetComponent<AudioSource>();
        pFlying = false;
        playerFlying.SetActive(false);
        playerStanding.SetActive(true);
        killedHeli = false;
        killedHeliTime = 0f;

        camOffset = 0f;

        home = Instantiate(homePrefab, Vector3.zero, Quaternion.identity).GetComponent<Home>();

        shield = Instantiate(shieldPrefab, Vector3.zero, Quaternion.identity);
        shield.SetActive(false);

        cauldronIcon.SetActive(false);
        arrowIcon.SetActive(false);

        GenerateWorld();
    }

    private void Update()
    {
        UpdatePotions();
        GetInputs();
        if (dying) {
            dieTime -= Time.deltaTime;
            Player.transform.Rotate(Vector3.up, 720f * Time.deltaTime);
            if (dieTime <= 0f)
            {
                dying = false;
                Player.transform.position = new Vector3(0f, 0f, -1f);
                playerTrail.Clear();
            }
        }
        MovePlayer();
        UpdateShield();
        UpdateUI();

        if (killedHeli)
        {
            killedHeliTime -= Time.deltaTime;
            if (killedHeliTime <= 0f)
            {
                SceneManager.LoadScene("MenuScene");
            }
        }
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
            if (greenPotionTime <= 0f)
            {
                greenPotionTime = 0f;
                audioMusic.pitch = 1f;
            }
        }
        if (redPotionTime > 0f)
        {
            redPotionTime -= Time.deltaTime;
            if (redPotionTime <= 0f)
            {
                redPotionTime = 0f;
                shieldLife = false;
            }
        }
    }

    protected void GetInputs()
    {
        float targetCamFov = config.cameraMinFov;

        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputVector.Normalize();

        if (dying || killedHeli || inputVector.magnitude == 0)
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

        if (Alive && !killedHeli)
        {
            Vector3 eulerAngles = player.transform.eulerAngles;
            eulerAngles.y = Mathf.Atan2(playerSpeed.x, playerSpeed.y) * (180 / Mathf.PI);
            player.transform.eulerAngles = eulerAngles;
        }

        // also lerp the camfov to 0
        camFov = Mathf.Lerp(camFov, targetCamFov, Time.deltaTime);

        if (Alive && bookOpen && Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            MixGreen();
        }

        if (Alive && bookOpen && Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            MixRed();
        }
    }

    protected void MovePlayer()
    {
        float psm = playerSpeed.magnitude;

        Vector3 pos = player.transform.position;
        pos.x += playerSpeed.x;
        pos.z += playerSpeed.y;
        pos.y = Mathf.Lerp(config.playerMinHeight, config.playerMaxHeight, psm / config.playerMaxSpeed);
        player.transform.position = pos;

        if (pos.magnitude <= config.cauldronDistance && psm <= config.cauldronSpeed)
        {
            if (!bookOpen)
            {
                uiBookAnimator.SetTrigger("OpenSpellBook");
                bookOpen = true;
            }

            cauldronIcon.SetActive(false);
            arrowIcon.SetActive(false);
        } else if (bookOpen)
        {
            uiBookAnimator.SetTrigger("CloseSpellBook");
            bookOpen = false;

            cauldronIcon.SetActive(true);
            arrowIcon.SetActive(true);
        }

        if (cauldronIcon.activeSelf)
        {
            cauldronIcon.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            cauldronIcon.transform.LookAt(Vector3.zero);
            cauldronIcon.transform.Translate(cauldronIcon.transform.forward * 0.55f, Space.World);
            cauldronIcon.transform.eulerAngles = Vector3.zero;
            cauldronIcon.transform.Translate(cauldronIcon.transform.up * player.transform.position.y);
        }
        if (arrowIcon.activeSelf)
        {
            arrowIcon.transform.position = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            arrowIcon.transform.LookAt(Vector3.zero);
            arrowIcon.transform.Translate(arrowIcon.transform.forward * 0.7f, Space.World);
            arrowIcon.transform.Translate(arrowIcon.transform.up * player.transform.position.y);
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
        if (Alive)
        {
            foreach (Frog f in frogs)
            {
                float fd = Vector2.Distance(new Vector2(f.transform.position.x, f.transform.position.z), new Vector2(pos.x, pos.z));
                if (fd < config.captureDistance)
                {
                    deadFrogs.Add(f);
                }
                else if (fd > config.despawnDistance)
                {
                    farFrogs.Add(f);
                }
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

        if (deadFrogs.Count > 0)
        {
            // play frog capture audio
            SfxClip(sfxFrogPickup);
        }

        List<Dragon> deadDragons = new List<Dragon>();
        List<Dragon> farDragons = new List<Dragon>();
        if (Alive)
        {
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

        if (deadDragons.Count > 0)
        {
            // play frog capture audio
            SfxClip(sfxDragonPickup);
        }

        if (heli != null)
        {
            float fd = Vector2.Distance(new Vector2(heli.transform.position.x, heli.transform.position.z), new Vector2(pos.x, pos.z));
            if (fd < config.captureDistance && redPotionTime > 0f)
            {
                Destroy(heli.gameObject);
                heli = null;
                killedHeli = true;
                killedHeliTime = 8f;
                musicManager.Finish();
                Instantiate(winShowerPrefab, new Vector3(player.transform.position.x, 0f, player.transform.position.z), Quaternion.identity);
            }
            else if (fd > config.despawnDistance)
            {
                Destroy(heli.gameObject);
                heli = null;
            }
        }

        psm = playerSpeed.magnitude;

        // standing or flying
        if ((!pFlying && (psm >= 0.05f || player.transform.position.y > 0.05f)) || dying)
        {
            playerFlying.SetActive(true);
            playerStanding.SetActive(false);
            pFlying = true;
        } else if (pFlying && Alive && (psm < 0.05f && player.transform.position.y <= 0.05f))
        {
            playerFlying.SetActive(false);
            playerStanding.SetActive(true);
            pFlying = false;
        }

    }

    protected void MoveCamera()
    {
        if (bookOpen)
        {
            camOffset = Mathf.SmoothStep(camOffset, -1.32f, Time.deltaTime * 8f);
        }
        else
        {
            camOffset = Mathf.SmoothStep(camOffset, 0f, Time.deltaTime * 12f);
        }
        mainCamera.transform.position = new Vector3(player.transform.position.x + camOffset, mainCamera.transform.position.y, player.transform.position.z - 0.5f);
        mainCamera.fieldOfView = camFov;
    }

    protected void GenerateWorld()
    {
        int startX = playerTile.x - (config.tileGenerationDistance + 2);
        int endX = playerTile.x + (config.tileGenerationDistance + 3);
        int startY = playerTile.y - (config.tileGenerationDistance + 2);
        int endY = playerTile.y + (config.tileGenerationDistance + 3);

        for (int xx = startX; xx < endX; xx++)
        {
            for (int yy = startY; yy < endY; yy++)
            {
                CheckNewTile(xx, yy, true, false);
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
            CheckNewTile(xx, yy, true, true);
        }
    }

    protected void GenerateWorldY(int dy)
    {
        int yy = dy > 0 ? playerTile.y + config.tileGenerationDistance : playerTile.y - config.tileGenerationDistance;

        for (int xx = playerTile.x - config.tileGenerationDistance; xx < playerTile.x + config.tileGenerationDistance + 1; xx++)
        {
            CheckNewTile(xx, yy, true, true);
        }
    }

    protected void UpdateUI()
    {
        uiFrogs.text = inv.frogs.ToString();
        uiDragonScales.text = inv.dragonScales.ToString();

        uiGreenPotion.sizeDelta = new Vector2(32f, 55f * (greenPotionTime / config.greenPotionTime));
        uiRedPotion.sizeDelta = new Vector2(32f, 55f * (redPotionTime / config.redPotionTime));
    }

    protected void CheckNewTile(int x, int y, bool spawnScenery, bool spawnEnemy)
    {
        Vector2Int pos = new Vector2Int(x, y);
        if (!tiles.ContainsKey(pos))
        {
            NewTile(pos);
        } else
        {
            spawnScenery = false;
        }

        if (spawnEnemy && Vector2Int.Distance(pos, Vector2Int.zero) > config.homeDistance)
        {
            if (frogs.Count < config.targetFrogs && Random.value <= config.frogChance)
            {
                SpawnFrog(pos);
                spawnScenery = false;
            }
            else if (dragons.Count < config.targetDragons && Random.value <= config.dragonChance)
            {
                SpawnDragon(pos);
                spawnScenery = false;
            }
            else if (heli == null && !killedHeli && Random.value <= config.heliChance)
            {
                SpawnHeli(pos);
                spawnScenery = false;
            }
        }
        if (spawnScenery) {
            if (Random.value <= config.smallTreeChance)
            {
                SpawnTree(1, pos);
            }
            else if (Random.value <= config.mediumTreeChance)
            {
                SpawnTree(2, pos);
            }
            else if (Random.value <= config.tallTreeChance)
            {
                SpawnTree(3, pos);
            }
            else if (Random.value <= config.smallRockChance)
            {
                SpawnRock(1, pos);
            }
        }
    }

    protected void NewTile(Vector2Int pos)
    {
        GameObject tilePrefab;
        if (Random.value <= config.bushTileChance)
        {
            tilePrefab = Instantiate(bushTilePrefab, new Vector3(pos.x * config.tileWidth, 0f, pos.y * config.tileWidth), Quaternion.identity);
        } else
        {
            tilePrefab = Instantiate(grassTilePrefab, new Vector3(pos.x * config.tileWidth, 0f, pos.y * config.tileWidth), Quaternion.identity);
        }
        tilePrefab.transform.localScale = new Vector3(config.tileWidth, 1f, config.tileWidth);
        tilePrefab.transform.eulerAngles = new Vector3(0f, 90f * Random.Range(0, 3), 0f);
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
        GameObject go = Instantiate(dragonPrefab, new Vector3(tilePos.x * config.tileWidth, 0f, tilePos.y * config.tileWidth), Quaternion.identity);
        go.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
        dragons.Add(
            go.GetComponent<Dragon>()
        );
    }

    protected void SpawnHeli(Vector2Int tilePos)
    {
        heli = Instantiate(heliPrefab, new Vector3(tilePos.x * config.tileWidth, 0f, tilePos.y * config.tileWidth), Quaternion.identity).GetComponent<Heli>();
        heli.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
    }

    protected void UpdateShield()
    {
        shield.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.2f, player.transform.position.z);
        shield.SetActive(shieldLife);
    }

    protected void SpawnTree(int levels, Vector2Int pos)
    {
        float scale = Random.Range(0.75f, 1.5f);

        Vector3 basePos = new Vector3(pos.x * config.tileWidth + Random.Range(-treeOffset, treeOffset), 0f, pos.y * config.tileWidth + Random.Range(-treeOffset, treeOffset));

        for (int i = 0; i < levels; i++)
        {
            GameObject tree = treePrefabs[Random.Range(0, treePrefabs.Length - 1)];
            GameObject treeInstance = Instantiate(tree, basePos + new Vector3(Random.Range(-0.1f, 0.1f), (i + 1) * 0.2f, Random.Range(-0.1f, 0.1f)), Quaternion.identity);

            treeInstance.transform.localScale = new Vector3(scale, scale, scale);
            treeInstance.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

            scale *= Random.Range(0.5f, 0.8f);
        }
    }

    protected void SpawnRock(int levels, Vector2Int pos)
    {
        float scale = Random.Range(0.75f, 1.5f);

        Vector3 basePos = new Vector3(pos.x * config.tileWidth + Random.Range(-treeOffset, treeOffset), 0f, pos.y * config.tileWidth + Random.Range(-treeOffset, treeOffset));

        for (int i = 0; i < levels; i++)
        {
            GameObject rock = rockPrefabs[Random.Range(0, rockPrefabs.Length - 1)];
            GameObject treeInstance = Instantiate(rock, basePos + new Vector3(Random.Range(-0.1f, 0.1f), (i + 1) * 0.05f, Random.Range(-0.1f, 0.1f)), Quaternion.identity);

            treeInstance.transform.localScale = new Vector3(scale, scale, scale);
            treeInstance.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);

            scale *= Random.Range(0.5f, 0.8f);
        }
    }

    protected void SfxClip(AudioClip clip)
    {
        audioSfx.Stop();
        audioSfx.clip = clip;
        audioSfx.Play();
    }

    public void Die()
    {
        if (!Alive) return;
        if (redPotionTime > 0f)
        {
            if (shieldLife)
            {
                shieldLife = false;
                redPotionTime = 1f;
            }
            SfxClip(sfxShieldDown);
            return;
        }
        dieTime = playerAudio.clip.length;
        dying = true;
        playerAudio.Play();
    }

    public void MixGreen()
    {
        if (inv.frogs < 6) return;

        inv.frogs -= 6;
        greenPotionTime = config.greenPotionTime;
        audioMusic.pitch = config.greenMusicPitch;
        SfxClip(sfxDrink);
        home.greenBurst.Play();
    }

    public void MixRed()
    {
        if (inv.dragonScales < 3) return;

        inv.dragonScales -= 3;
        redPotionTime = config.redPotionTime;
        shieldLife = true;
        SfxClip(sfxDrink);
        home.redBurst.Play();
    }
}
