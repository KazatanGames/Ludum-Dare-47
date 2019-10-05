using UnityEngine;
using System.Collections;

public class Dragon : MonoBehaviour
{

    [SerializeField]
    protected GameObject head;
    [SerializeField]
    protected GameObject neck;
    [SerializeField]
    protected float rotateSpeed = 720f;
    [SerializeField]
    protected float breathTime = 0.25f;
    [SerializeField]
    protected float firePerSecond = 20f;
    [SerializeField]
    protected GameObject firePrefab;

    protected float recovery = 0f;

    // Update is called once per frame
    void Update()
    {
        if (recovery > 0)
        {
            recovery -= Time.deltaTime;
        } else
        {
            recovery = 0f;
        }

        if (recovery == 0)
        {
            TurnHead();
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
}
