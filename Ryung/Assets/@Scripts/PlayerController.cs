using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [Header("Movement Settings")]
    public float moveSpeed = 12f;         // �ְ� �ӵ�
    public float acceleration = 60f;      // ���ӵ�
    public float deacceleration = 60f;    // ���ӵ�
    public float frictionAmount = 0.2f;   // ������

    [Header("Jump Settings")]
    public float jumpForce = 15f;         // ���� ��
    public float fallMultiplier = 2.5f;   // �ϰ� �� �߷� ����
    public float jumpCutMultiplier = 5f;  // ���� Ű ���� �� �ϰ� ����

    [Header("Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Grapple Reference")]
    public Camera mainCamera;             // ������ ���� ���� ī�޶�
    [HideInInspector] public Vector2 mousePos; // �ٸ� ��ũ��Ʈ���� ������ ���콺 ��ǥ

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");


        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (jumpCutMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    void ApplyMovement()
    {
        float targetSpeed = moveInput * moveSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;

        float speedDif = targetSpeed - rb.linearVelocity.x;

        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        if (isGrounded && Mathf.Abs(moveInput) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.linearVelocity.x);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
