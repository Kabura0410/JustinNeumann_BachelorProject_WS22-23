using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxHealth;

    public int health;

    public float speed;
    public float jumpForce;
    private float moveInput;

    [HideInInspector] public Rigidbody2D rb;

    private bool isGrounded;
    private bool isOnSemiPlatform;
    public Transform groundCheck;
    public float checkRadius;
    public float checkRadiusForSemiGround;
    public LayerMask whatIsGround;
    public LayerMask whatIsSemiGround;

    private int extraJumps;
    public int extraJumpsValue;

    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField] private Collider2D playerCollider;

    private Collider2D currentSemiCollider;

    public float jumpTime;
    private float _jumpTime;

    private bool isJumping;


    private float vertical;
    public float ladderSpeed;
    private bool isLadder;
    private bool isClimbing;

    private float startGravity;

    public float StartGravity
    {
        get
        {
            return startGravity;
        }
    }

    [SerializeField] private float maxYVelocity;
    [SerializeField] private float maxXVelocity;

    [Range(0f,1f)][SerializeField] private float xDrag;
    [Range(0f,1f)][SerializeField] private float yDrag;

    private bool knockbackReceived;

    public float enemyKnockbackDuration;
    public float weaponKnockbackDuration;

    public SpriteRenderer reloadIndicator;

    private bool canMove = true;

    private int currentCoins;

    public int CurrentCoins
    {
        get
        {
            return currentCoins;
        }
    }

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreLayerCollision(9,11);
        startGravity = rb.gravityScale;
    }

    void FixedUpdate()
    {

        if (canMove)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            isOnSemiPlatform = Physics2D.OverlapCircle(groundCheck.position, checkRadiusForSemiGround, whatIsSemiGround);
            if (isOnSemiPlatform)
            {
                if(currentSemiCollider == null)
                {
                    currentSemiCollider = Physics2D.OverlapCircle(groundCheck.position, checkRadiusForSemiGround, whatIsSemiGround);
                }
            }
            else
            {
                currentSemiCollider = null;
            }

            if (!knockbackReceived)
            {
                moveInput = Input.GetAxisRaw("Horizontal");
                rb.AddForce(new Vector2(moveInput * speed, rb.velocity.y));
            }





            //Apply maximum y velocity 
            if(rb.velocity.y > maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxYVelocity);
            }
            if (rb.velocity.y < -maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, -maxYVelocity);
            }


            //Apply maximum x velocity 
            if (rb.velocity.x > maxXVelocity)
            {
                rb.velocity = new Vector2(maxXVelocity, rb.velocity.y);
            }
            if (rb.velocity.x < -maxXVelocity)
            {
                rb.velocity = new Vector2(-maxXVelocity, rb.velocity.y);
            }



            //Apply drag in x direction when no knockback
            if (!knockbackReceived)
            {
                rb.velocity = new Vector2(rb.velocity.x * xDrag, rb.velocity.y);
            }

            //Apply drag in y direction
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yDrag);


            if (isClimbing)
            {
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, vertical * ladderSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.gravityScale = startGravity;
            }

            if(Input.GetKey(KeyCode.Space) && isJumping)
            {
                if(_jumpTime > 0)
                {
                    rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                    _jumpTime -= Time.fixedDeltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }
        }
    }

    void Update()
    {
        if (!canMove) return;
        CheckFlip();
        if(isGrounded == true)
        {
            extraJumps = extraJumpsValue;
            if (isOnSemiPlatform)
            {
                if (Input.GetKeyDown(KeyCode.S) && currentSemiCollider != null)
                {
                    Physics2D.IgnoreCollision(currentSemiCollider, playerCollider, true);
                    StartCoroutine(ReactivateCollision(currentSemiCollider));
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);
            extraJumps--;
            isJumping = true;
            _jumpTime = jumpTime;
        }


        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        vertical = Input.GetAxis("Vertical");

        if(isLadder && Input.GetKey(KeyCode.W) && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }

    private void CheckFlip()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        playerSprite.flipX = direction.x > 0 ? false : true;
    }

    public void GetKnockback(Vector3 _direction, float _intensity, float _knockbackDuration)
    {
        rb.AddForce(_direction * _intensity, ForceMode2D.Impulse);
        knockbackReceived = true;
        StartCoroutine(DelayedKnockbackDeactivation(_knockbackDuration));
    }

    public void GetDamage(int _amount, Vector3 _direction, float _intensity, float _knockbackDuration)
    {
        health -= _amount;
        GameManager.instance.UpdateHealthBars();
        GetKnockback(_direction, _intensity, _knockbackDuration);
        if(health <= 0)
        {
            Die();
        }
    }

    public void ToggleMovement()
    {
        canMove = !canMove;
    }

    public void IncreaseCoins(int _amount)
    {
        currentCoins += _amount;
    }

    private void Die()
    {
        GameManager.instance.KillPlayer();
    }

    private IEnumerator ReactivateCollision(Collider2D _targetCollider)
    {
        yield return new WaitForSecondsRealtime(.2f);
        Physics2D.IgnoreCollision(_targetCollider, playerCollider, false);
    }

    private IEnumerator DelayedKnockbackDeactivation(float _duration)
    {
        yield return new WaitForSecondsRealtime(_duration);
        knockbackReceived = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }

}

    
