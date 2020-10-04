using UnityEngine;
using System.Collections;

public class ParticlesManager : SingletonMonoBehaviour<ParticlesManager>
{

    [SerializeField]
    public GameObject destroyedCrateParticlesPrefab;


    public void DestroyCrateParticles(Vector3 position)
    {
        GameObject.Instantiate(destroyedCrateParticlesPrefab, position, Quaternion.identity, transform);
    }
}
