using UnityEngine;
using System.Collections;

public class DanCam : MonoBehaviour
{

    protected Camera cam;

    protected float wantedAspectRatio = 16f / 9f;

    // Use this for initialization
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentAspectRatio = (float)Screen.width / (float)Screen.height;

        float inset = 1.0f - currentAspectRatio / wantedAspectRatio;

        cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
    }
}
