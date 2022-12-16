using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCrystalTrigger : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Crystal")
        {
            enemy.ChangeFocus(Enemy.FocusType.crystal);
            StartCoroutine(CheckGround());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Crystal")
        {
            enemy.ToggleMovement();
        }
    }


    private IEnumerator CheckGround()
    {
        yield return new WaitForEndOfFrame();
        if (enemy.isGrounded)
        {
            enemy.DoAttack();
            float r = Random.Range(0.2f, 1f);
            StartCoroutine(enemy.ToggleMovementDelayed(r));
        }
        else
        {
            StartCoroutine(CheckGround());
        }
    }
}