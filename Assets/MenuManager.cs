using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField]
    protected GameObject player;
    [SerializeField]
    protected GameObject title;
    [SerializeField]
    protected TrailRenderer playerTrail;
    [SerializeField]
    public ParticleSystem greenBurst;
    [SerializeField]
    public ParticleSystem redBurst;

    [Header("Setup")]
    [SerializeField]
    protected float playerWaitTime;
    [SerializeField]
    protected Rect bounds;


    protected float playerWait;
    protected float playerRotation;
    protected bool playerFlying;
    protected bool playerRight;
    protected float nextBurst;
    protected Vector3 titleStart;
    protected bool titleUp = false;
    protected float nextTitle;
    protected float titleTime;

    // Start is called before the first frame update
    void Awake()
    {
        playerFlying = false;
        playerRight = Random.value > 0.5f;
        playerWait = playerWaitTime;
        nextBurst = Random.Range(2f, 6f);
        nextTitle = 2f;
        titleStart = title.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerFlying)
        {
            playerWait -= Time.deltaTime;
            if (playerWait <= 0)
            {
                PlayerFly();
            }
        } else
        {
            player.transform.Translate(player.transform.forward * 10f * Time.deltaTime, Space.World);
            player.transform.Rotate(player.transform.up * Time.deltaTime, playerRotation, Space.World);
            if (!bounds.Contains(new Vector2(player.transform.position.x, player.transform.position.z)))
            {
                playerRight = player.transform.position.x < 0;
                playerFlying = false;
            }
        }

        nextBurst -= Time.deltaTime;
        if (nextBurst <= 0f)
        {
            nextBurst = Random.Range(2f, 6f);
            if (Random.value > 0.5f)
            {
                greenBurst.Play();
            } else
            {
                redBurst.Play();
            }
        }

        if (titleUp)
        {
            titleTime += Time.deltaTime;
            if (titleTime >= 2f)
            {
                titleTime = 2f;
                titleUp = false;
            }
        } else
        {
            titleTime -= Time.deltaTime;
            if (titleTime <= 0f)
            {
                titleTime = 0f;
                titleUp = true;
            }
        }
        float tt = titleTime / 2f;

        title.transform.position = new Vector3(titleStart.x, Mathf.SmoothStep(titleStart.y - 0.2f, titleStart.y + 0.2f, tt), titleStart.z);

        if (Input.GetButtonDown("Submit"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    protected void PlayerFly()
    {
        playerFlying = true;
        playerWait = playerWaitTime;
        player.transform.position = new Vector3(playerRight ? bounds.xMin : bounds.xMax, 0f, Random.Range(bounds.yMin, bounds.yMax));
        playerTrail.Clear();
        player.transform.LookAt(Vector3.zero);
        player.transform.Translate(0f, 0.3f, 0f);
        player.transform.Translate(player.transform.forward * -5f * Time.deltaTime, Space.World);
        playerRotation = Random.Range(-0.5f, 0.5f);
    }
}
