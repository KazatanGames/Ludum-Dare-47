using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GameSetup
{
    public int tileGenerationDistance;
    public float tileWidth;
    public float cameraMinFov;
    public float cameraMaxFov;
    public float playerMaxSpeed;
    public float playerAccel;
    public float playerDeccel;
    public float playerMinHeight;
    public float playerMaxHeight;
}
