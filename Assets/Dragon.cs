using UnityEngine;
using System.Collections;

public class Dragon : MonoBehaviour
{

    [SerializeField]
    protected GameObject head;
    [SerializeField]
    protected GameObject neck;
    [SerializeField]
    protected AudioSource audioBreath;
    [SerializeField]
    protected float rotateSpeed = 720f;
    [SerializeField]
    protected float lookDistance = 6.5f;
    [SerializeField]
    protected float range = 3.5f;
    [SerializeField]
    protected float breathTime = 1f;
    [SerializeField]
    protected float recoveryTime = 0.75f;
    [SerializeField]
    protected float firePerSecond = 50f;
    [SerializeField]
    protected GameObject firePrefab;

    protected float recovery = 0f;
    protected float breathed = 0f;
    protected float fireBank = 0f;
    protected float fireRate = 1f;

    private void Awake()
    {
        fireRate = 1f / firePerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        bool firing = false;
        if (recovery > 0)
        {
            recovery -= Time.deltaTime;
        } else
        {
            recovery = 0f;
        }

        float playerDist = Vector3.Distance(GameManager.INSTANCE.Player.transform.position, transform.position);

        if (playerDist <= lookDistance)
        {
            TurnHead();
        }

        if (recovery == 0) {
            if (playerDist <= range)
            {
                firing = true;
                Fire();
                breathed += Time.deltaTime;
                if (breathed >= breathTime)
                {
                    recovery = recoveryTime;
                    breathed = 0f;
                }
            }
            else
            {
                if (breathed > 0f)
                {
                    breathed -= Time.deltaTime;
                    if (breathed < 0f)
                    {
                        breathed = 0f;
                    }
                }
            }
        }
        if (firing)
        {
            if (!audioBreath.isPlaying)
            {
                audioBreath.Play();
            }
        } else
        {
            audioBreath.Stop();
        }
    }

    protected void TurnHead()
    {
        float oldY = head.transform.eulerAngles.y;
        head.transform.LookAt(GameManager.INSTANCE.Player.transform.position);
        float newY = 180f + head.transform.eulerAngles.y;
        float diff = newY - oldY;
        if ( diff <= -180 )
        {
            diff += 360f;
        } else if (diff >= 180f)
        {
            diff -= 360f;
        }
        float maxDiff = rotateSpeed * Time.deltaTime;
        head.transform.eulerAngles = new Vector3(90f, oldY + Mathf.Clamp(diff, -maxDiff, maxDiff), 0f);
    }

    protected void Fire()
    {
        fireBank += Time.deltaTime;

        while(fireBank >= fireRate)
        {
            fireBank -= fireRate;
            DragonFire df = Instantiate(firePrefab, head.transform.position, Quaternion.identity).GetComponent<DragonFire>();
            df.transform.eulerAngles = new Vector3(0f, head.transform.eulerAngles.y - 180f, 0f);
            df.transform.Translate(df.transform.forward * (fireBank + 0.4f), Space.World);
        }
    }
}
