using UnityEngine;
using System.Collections;

public class Frog : MonoBehaviour
{

    [SerializeField]
    protected GameObject body;
    [SerializeField]
    protected AudioSource audioJump;
    [SerializeField]
    protected float jumpSpeed = 5f;
    [SerializeField]
    protected float jumpTime = 0.25f;
    [SerializeField]
    protected float minWaitTime = 0.25f;
    [SerializeField]
    protected float maxWaitTime = 1f;
    [SerializeField]
    protected float jumpHeight = 0.2f;

    protected bool jumping = false;

    protected float nextAction = 0f;

    protected float speed = 0f;

    // Update is called once per frame
    void Update()
    {
        nextAction -= Time.deltaTime;
        if (nextAction <= 0f)
        {
            if (jumping)
            {
                Land();
            } else
            {
                Jump();
            }
        } else if (!jumping)
        {
            speed = Mathf.Lerp(0f, speed, Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;
    }

    protected void Jump()
    {
        if (jumping) return;
        body.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0.5f, 0f);
        jumping = true;

        Vector3 angles = transform.eulerAngles;
        angles.y = Random.value * 360f;
        transform.eulerAngles = angles;

        transform.position += transform.up * jumpHeight;

        speed = jumpSpeed;

        audioJump.Play();

        nextAction = jumpTime;
    }

    protected void Land()
    {
        if (!jumping) return;
        body.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0f, 0f);
        jumping = false;

        transform.position -= transform.up * jumpHeight;

        nextAction = Random.Range(0.5f, 2.5f);
    }
}
