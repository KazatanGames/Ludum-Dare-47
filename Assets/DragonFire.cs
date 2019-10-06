using UnityEngine;
using System.Collections;

public class DragonFire : MonoBehaviour
{
    [SerializeField]
    protected GameObject fireball;
    [SerializeField]
    protected float life = 1.2f;
    [SerializeField]
    protected float speed = 2f;

    protected float scale;
    protected float rotateSpeedUp;
    protected float rotateSpeedForward;

    // Use this for initialization
    void Awake()
    {
        rotateSpeedUp = Random.Range(500f, 1000f);
        rotateSpeedForward = Random.Range(360f, 720f);
        scale = Random.Range(0.15f, 0.25f);
        fireball.transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;

        if (life <= 0)
        {
            Destroy(this.gameObject);
            return;
        }

        fireball.transform.Rotate(Vector3.up, rotateSpeedUp * Time.deltaTime);
        fireball.transform.Rotate(Vector3.forward, rotateSpeedForward * Time.deltaTime);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        Vector3 ppos = GameManager.INSTANCE.Player.transform.position;
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(ppos.x, ppos.z)) < scale)
        {
            GameManager.INSTANCE.Die();
        }
    }
}
