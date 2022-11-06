using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerScript : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            enemy.ChangeFocus(Enemy.FocusType.player);
            enemy.DoAttack();
            enemy.ToggleMovement();
        }
        if(collision.gameObject.tag == "Crystal" && enemy.isGrounded)
        {
            enemy.ChangeFocus(Enemy.FocusType.crystal);
            enemy.DoAttack();
            enemy.ToggleMovement();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemy.ChangeFocus(Enemy.FocusType.none);
            enemy.ToggleMovement();
        }
    }
}
