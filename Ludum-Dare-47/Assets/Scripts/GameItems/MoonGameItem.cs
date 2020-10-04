using UnityEngine;
using System.Collections;

public class MoonGameItem : MonoBehaviour
{
    [SerializeField]
    protected Vector3 startPosition;
    [SerializeField]
    protected Vector3 midPosition;
    [SerializeField]
    protected Vector3 endPosition;
    [SerializeField]
    protected PointAtZero moonLight;
    [SerializeField]
    protected GameObject quad;

    protected float ratioTime;

    protected float currentRatio;
    protected float oldRatio;
    protected float targetRatio;

    public void SetPositionRatio(float ratio)
    {
        ratioTime = 0f;
        oldRatio = currentRatio;
        targetRatio = ratio;
    }

    private void Awake()
    {
        transform.position = startPosition;
        moonLight.Point();

        oldRatio = 0;
        currentRatio = 0;
        targetRatio = 0;
    }

    private void Update()
    {
        if (ratioTime >= 1) return;

        ratioTime += Time.deltaTime;
        currentRatio = Mathf.Lerp(oldRatio, targetRatio, Mathf.Clamp(ratioTime, 0, 1));


        if (currentRatio <= 0)
        {
            transform.position = startPosition;
        }
        else if (currentRatio >= 1)
        {
            transform.position = endPosition;
        }
        else if (currentRatio == 0.5f)
        {
            transform.position = midPosition;
        }
        else if (currentRatio < 0.5f)
        {
            float ratio = Easing.Quintic.InOut(Mathf.InverseLerp(0, 0.5f, currentRatio));
            float dx = midPosition.x - startPosition.x; //-3
            float dy = midPosition.y - startPosition.y; // 1
            transform.position = new Vector3(
               startPosition.x + (ratio * dx),
               startPosition.y + (Easing.Cubic.Out(ratio) * dy),
               startPosition.z
            );
        }
        else
        {
            float ratio = Easing.Quintic.InOut(Mathf.InverseLerp(0.5f, 1f, currentRatio));
            float dx = endPosition.x - midPosition.x;
            float dy = endPosition.y - midPosition.y;
            transform.position = new Vector3(
               midPosition.x + (ratio * dx),
               midPosition.y + (Easing.Cubic.In(ratio) * dy),
               startPosition.z
            );
        }

        moonLight.Point();
        quad.transform.rotation = Quaternion.LookRotation(quad.transform.position - Camera.main.transform.position);
    }
}
