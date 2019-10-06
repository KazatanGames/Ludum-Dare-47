using UnityEngine;
using System.Collections;

public class DragonMenu : MonoBehaviour
{

    [SerializeField]
    protected GameObject head;


    protected float angle = 0f;
    protected float yVel = 0f;

    protected bool back = false;

    // Update is called once per frame
    void Update()
    {
        if (Mathf.DeltaAngle(head.transform.eulerAngles.y, angle) <= 0.5f)
        {
            angle = Random.Range(0f, 360f);
            if (back)
            {
                angle *= -1f;
            }
            back = !back;
        }

        head.transform.eulerAngles = new Vector3(
            head.transform.eulerAngles.x,
            Mathf.SmoothDampAngle(head.transform.eulerAngles.y, angle, ref yVel, 0.5f),
            head.transform.eulerAngles.z
        );
    }
}
