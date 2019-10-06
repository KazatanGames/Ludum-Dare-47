using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [SerializeField]
    protected GameObject player;

    private void Awake()
    {
        if (player == null)
        {
            player = GameManager.INSTANCE.Player;
        }
    }

    void Update()
    {
        transform.position = player.transform.position;
    }
}
