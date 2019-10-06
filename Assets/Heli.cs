using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heli : MonoBehaviour
{

    [SerializeField]
    protected GameObject body;
    [SerializeField]
    protected GameObject blades;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        blades.transform.Rotate(blades.transform.forward, 1250f * Time.deltaTime, Space.World);

        transform.Rotate(transform.up, 90f * Time.deltaTime);
        transform.Translate(transform.forward * Time.deltaTime * 5f, Space.World);

        Vector3 ppos = GameManager.INSTANCE.Player.transform.position;
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(ppos.x, ppos.z)) < 0.5f)
        {
            GameManager.INSTANCE.Die();
        }
    }

}
