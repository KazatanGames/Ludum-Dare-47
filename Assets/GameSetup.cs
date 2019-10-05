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
    public float captureDistance;
    public float despawnDistance;
    public float targetFrogs;
    public float frogChance;
    public float targetDragons;
    public float dragonChance;
}
