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

    public void SetPositionRatio(float ratio)
    {
        if (ratio <= 0)
        {
            transform.position = startPosition;
        }
        else if (ratio >= 1)
        {
            transform.position = endPosition;
        }
        else if (ratio == 0.5f)
        {
            transform.position = midPosition;
        } else if (ratio < 0.5f)
        {
            ratio = Mathf.InverseLerp(0, 0.5f, ratio);
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
            ratio = Mathf.InverseLerp(0.5f, 1f, ratio);
            float dx = endPosition.x - midPosition.x;
            float dy = endPosition.y - midPosition.y;
            transform.position = new Vector3(
               midPosition.x + (ratio * dx),
               midPosition.y + (Easing.Cubic.In(ratio) * dy),
               startPosition.z
            );
        }

        moonLight.Point();
    }

    private void Awake()
    {
        transform.position = startPosition;
        moonLight.Point();
    }
}
