using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE = null;
    
    [Header("Game Objects")]
    [SerializeField]
    protected GameObject player;
    [SerializeField]
    protected Camera mainCamera;
    [Header("Game Setup")]
    [SerializeField]
    protected GameSetup config;
    [Header("Tiles")]
    [SerializeField]
    protected GameObject grassTilePrefab;
    [Header("Other Prefabs")]
    [SerializeField]
    protected GameObject homePrefab;

    protected Vector2 playerSpeed;
    protected float camFov;
    Vector2Int playerTile;

    protected Dictionary<Vector2Int, TileInstance> tiles;

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

        Instantiate(homePrefab, Vector3.zero, Quaternion.identity);

        GenerateWorld();
    }

    private void Update()
    {
        GetInputs();
        MovePlayer();
    }

    private void LateUpdate()
    {
        MoveCamera();
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
            // increase the speed to the max
            playerSpeed = Vector2.Lerp(playerSpeed, inputVector * config.playerMaxSpeed, Time.deltaTime * config.playerAccel);

            // also lerp the camfov to max
            targetCamFov = Mathf.Lerp(config.cameraMinFov, config.cameraMaxFov, playerSpeed.magnitude / config.playerMaxSpeed);
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

        Vector2Int newPlayerTile = new Vector2Int(Mathf.FloorToInt(player.transform.position.x / config.tileWidth), Mathf.FloorToInt(player.transform.position.z / config.tileWidth));
        if (newPlayerTile != playerTile)
        {
            playerTile = newPlayerTile;
            GenerateWorld();
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
                Vector2Int pos = new Vector2Int(xx, yy);
                if (!tiles.ContainsKey(pos))
                {
                    GameObject tilePrefab = Instantiate(grassTilePrefab, new Vector3(xx * config.tileWidth, 0f, yy * config.tileWidth), Quaternion.identity);
                    tilePrefab.transform.localScale = new Vector3(config.tileWidth, 1f, config.tileWidth);
                    tiles.Add(pos, new TileInstance(pos, tilePrefab, new TileData()));
                }
            }
        }
    }

}
