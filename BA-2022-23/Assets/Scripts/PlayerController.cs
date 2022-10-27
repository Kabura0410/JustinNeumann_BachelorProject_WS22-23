using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public int health;

    public float speed;
    public float jumpForce;
    private float moveInput;

    private Rigidbody2D rb;

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

    [SerializeField] private float knockbackDuration;

    void Start()
    {
        extraJumps = extraJumpsValue;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreLayerCollision(9,11);
    }

    void FixedUpdate()
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


        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);


        if(isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * ladderSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.gravityScale = 5f;
        }

    }

    void Update()
    {
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
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
            isJumping = true;
            _jumpTime = jumpTime;
        }

        if(Input.GetKey(KeyCode.Space) && isJumping)
        {
            if(_jumpTime > 0)
            {
                rb.velocity = Vector2.up * jumpForce * jumpForce;
                _jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
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

    public void GetContactDamage()
    {
        print("Got contact damage");
    }

    private void CheckFlip()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        playerSprite.flipX = direction.x > 0 ? false : true;
    }

    public void GetKnockback(Vector3 _direction, float _intensity)
    {
        StartCoroutine(KnockBack(_direction, _intensity, 0));
    }

    private IEnumerator KnockBack(Vector3 _direction, float _intensity, float timer)
    {
        timer += Time.deltaTime;
        rb.AddForce(new Vector2(_direction.x * _intensity, _direction.y * _intensity / 4));
        yield return new WaitForEndOfFrame();
        if(timer < knockbackDuration)
        {
            StartCoroutine(KnockBack(_direction, _intensity, timer));
        }
    }

    private IEnumerator ReactivateCollision(Collider2D _targetCollider)
    {
        yield return new WaitForSecondsRealtime(.2f);
        Physics2D.IgnoreCollision(_targetCollider, playerCollider, false);
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

    
