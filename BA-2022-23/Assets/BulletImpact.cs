using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    [SerializeField] private float force;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 15)
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForceAtPosition((collision.gameObject.transform.position - transform.position) * force, transform.position);
        }
    }
}
