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

    public LayerMask whatIsLadder;

    [SerializeField] private float movementSpeed;

    public Rigidbody2D rb;

    [SerializeField] private float maxYVelocity;

    private bool ignoreMovementLogic = true;

    [SerializeField] private float logicDelay;

    [SerializeField] private SpriteRenderer enemySprite;

    public bool isClimbing;

    [SerializeField] private float ladderJumpHeight;

    [SerializeField] private float attackSpeed;
    private float _attackSpeed;

    private bool doMove = true;

    [SerializeField] private float knockbackDuration;

    [SerializeField] private float knockbackIntensity;


    [Header("Groundcheck")]
    public bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;

    public enum FocusType
    {
        none,
        player,
        crystal
    }

    public FocusType focusType;

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
        if(Mathf.Abs(rb.velocity.y) > maxYVelocity)
        {
            if(rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxYVelocity);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, maxYVelocity);
            }
        }
        if(health <= 0)
        {
            GameObject go = Instantiate(deathEffect, transform.position, Quaternion.identity);
            GameManager.instance.StartCoroutine(GameManager.instance.DeleteParticleDelayed(go, 2));
            GameManager.instance.allSpawnedEnemies.Remove(this);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        CheckWalls();
        DoMovement();
        DoTimer();
        if (!doMove)
        {
            DoAttack();
        }
        
    }

    private void DoMovement()
    {
        if (doMove)
        {
            if (!isClimbing)
            {
                transform.position += direction * Time.deltaTime * movementSpeed;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                transform.position += Vector3.up * Time.deltaTime * movementSpeed;
            }
        }
    }

    private void DoTimer()
    {
        if(_attackSpeed > 0)
        {
            _attackSpeed -= Time.fixedDeltaTime;
        }
    }

    private void CheckWalls()
    {
        if(direction == Vector3.right)
        {
            if(Physics2D.OverlapCircle(rightRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                if (!isClimbing)
                {
                    ChangeDirection();
                }
            }
            else 
            {
                if(Physics2D.OverlapCircle(rightRayPos.position, 0.2f, whatIsLadder))
                {
                    isClimbing = true;
                }
                else
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        rb.AddForce(new Vector2(0, ladderJumpHeight));
                    }
                    else
                    {
                        isClimbing = false;
                    }
                }
            }
        }
        if(direction == Vector3.left)
        {
            if (Physics2D.OverlapCircle(leftRayPos.position, 0.1f, whatIsGround) && !ignoreMovementLogic)
            {
                if (!isClimbing)
                {
                    ChangeDirection();
                }
            }
            else 
            {
                if (Physics2D.OverlapCircle(leftRayPos.position, 0.2f, whatIsLadder))
                {
                    isClimbing = true;
                }
                else
                {
                    if (isClimbing)
                    {
                        isClimbing = false;
                        rb.AddForce(new Vector2(0, ladderJumpHeight));
                    }
                    else
                    {
                        isClimbing = false;
                    }
                }
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

    public void ChangeFocus(FocusType _focusType)
    {
        focusType = _focusType;
    }

    public void DoAttack()
    {
        if(_attackSpeed <= 0)
        {
            switch (focusType)
            {
                case FocusType.none:
                    break;
                case FocusType.player:
                    GameManager.instance.player.GetDamage(damage);
                    float x = (GameManager.instance.player.transform.position - transform.position).normalized.x;
                    if(x > 0)
                    {
                        GameManager.instance.player.rb.velocity = Vector3.zero;
                        GameManager.instance.player.GetKnockback(new Vector3(1, .5f, 0), knockbackIntensity);
                    }
                    else
                    {
                        GameManager.instance.player.rb.velocity = Vector3.zero;
                        GameManager.instance.player.GetKnockback(new Vector3(-1, .5f, 0), knockbackIntensity);
                    }
                    break;
                case FocusType.crystal:
                    GameManager.instance.crystal.GetDamage(damage);
                    break;
            }
            _attackSpeed = attackSpeed;
        }
    }

    public void DoContactDamage()
    {
        GameManager.instance.player.GetDamage(damage);
    }

    public void GetKnockback(Vector3 _direction, float _intensity)
    {
        StartCoroutine(KnockBack(_direction, _intensity, 0));
    }

    private IEnumerator KnockBack(Vector3 _direction, float _intensity, float timer)
    {
        timer += Time.fixedDeltaTime;
        rb.AddForce(new Vector2(_direction.x * _intensity, _direction.y * _intensity / 4) + Vector2.up * _intensity / 8);
        yield return new WaitForEndOfFrame();
        if (timer < knockbackDuration)
        {
            StartCoroutine(KnockBack(_direction, _intensity, timer));
        }
    }

    public void ToggleMovement()
    {
        doMove = !doMove;
    }

    public IEnumerator ToggleMovementDelayed(float _time)
    {
        yield return new WaitForSecondsRealtime(_time);
        ToggleMovement();
    }
}
