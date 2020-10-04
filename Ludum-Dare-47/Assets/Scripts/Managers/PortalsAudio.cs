using UnityEngine;
using System.Collections;

public class PortalsAudio : SingletonMonoBehaviour<PortalsAudio>
{
    [SerializeField]
    public AudioSource left;
    [SerializeField]
    public AudioSource right;
    [SerializeField]
    public AudioSource center;

    [SerializeField]
    public AudioSource left2;
    [SerializeField]
    public AudioSource right2;
}
