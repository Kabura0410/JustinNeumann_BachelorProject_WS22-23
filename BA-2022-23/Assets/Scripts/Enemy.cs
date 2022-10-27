using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health;
    public GameObject deathEffect;
    public GameObject damageEffect;
    public int damage;

    private Vector3 direction;

    public Transform leftRayPos, rightRayPos;

    public LayerMask whatIsGround;

    [SerializeField] private float movementSpeed;

    private Rigidbody2D rb;

    [SerializeField] private float maxYVelocity;

    private bool ignoreMovementLogic = true;

    [SerializeField] private float logicDelay;

    [SerializeField] private SpriteRenderer enemySprite;

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(9,9);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(logicDelay > 0)
        {
            logicDelay -= Time.deltaTime;
            if(logicDelay <= 0)
            {
                ignoreMovementLogic = false;
            }
        }
        CheckWalls();
        DoMovement();
        if(Mathf.Abs(rb.velocity.y) > maxYVelocity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxYVelocity);
        }
        if(health <= 0)
        {
            GameObject go = Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
            Destroy(gameObject);
        }
    }

    private void DoMovement()
    {
        transform.position += direction * Time.deltaTime * movementSpeed;
    }

    private void CheckWalls()
    {
        if(direction == Vector3.right)
        {
            if(Physics2D.OverlapCircle(rightRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                ChangeDirection();
            }
        }
        if(direction == Vector3.left)
        {
            if (Physics2D.OverlapCircle(leftRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                ChangeDirection();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        GameObject go = Instantiate(damageEffect, transform.position, Quaternion.identity);
        GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
    }

    public void SetStartDirection(Vector3 _direction)
    {
        direction = _direction;
        enemySprite.flipX = direction.x > 0 ? false : true;
    }

    public void ChangeDirection()
    {
        if(direction == Vector3.right)
        {
            direction = Vector3.left;
        }
        else
        {
            direction = Vector3.right;
        }
        enemySprite.flipX = direction.x > 0 ? false : true;
    }

}
