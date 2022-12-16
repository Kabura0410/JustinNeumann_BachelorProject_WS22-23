using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    private Vector2 startpos;
    public GameObject player;
    public float parallaxEffect;

    void Start()
    {
        startpos = transform.position;
    }

    void FixedUpdate()
    {
        float xDist = (player.transform.position.x * parallaxEffect);
        float yDist = (player.transform.position.y * parallaxEffect / 2);
        transform.position = new Vector3(startpos.x + (-xDist), transform.position.y + (-yDist), transform.position.z);
    }
}
