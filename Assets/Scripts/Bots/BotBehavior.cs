using UnityEngine;

public class BotBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("�������� �������� ����.")]
    public float moveSpeed = 2f;

    [Tooltip("�����, ����� ������� ��� ������ ��� ���������.")]
    public float behaviorChangeInterval = 5f;

    [Tooltip("����������� ����, ��� ��� ����� ������ �� ����� (�� 0 �� 1).")]
    [Range(0f, 1f)]
    public float idleProbability = 0.7f;

    [Tooltip("�������� �������� ����.")]
    public float rotationSpeed = 5f;

    [Header("Jump Settings")]
    [Tooltip("���� ������.")]
    public float jumpForce = 5f;

    [Tooltip("�������� �� �����. ����, � ������� ���������� ��������.")]
    public LayerMask groundLayer;

    [Tooltip("��������� ��� �������� �� �����.")]
    public float groundCheckDistance = 0.1f;

    [Tooltip("����������� ����� ����� ��������.")]
    public float minJumpInterval = 2f; // ����������� �������� ����� ��������

    [Tooltip("������������ ����� ����� ��������.")]
    public float maxJumpInterval = 5f; // ������������ �������� ����� ��������

    [Tooltip("���� �� ������. ��������, 0.1 � 10% ����.")]
    [Range(0f, 1f)]
    public float jumpChance = 0.1f;

    [Header("Hand Settings")]
    [Tooltip("����� ��������� ������� � ����� ����.")]
    public Transform handPoint;

    private Vector3 moveDirection;
    private bool isStanding;
    private bool isGrounded;
    private float jumpTimer; // ������ ��� �������
    private float nextJumpTime; // ����� ���������� ���������� ������
    private Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // �������� Rigidbody ��� ������
        InvokeRepeating(nameof(ChangeBehavior), 0f, behaviorChangeInterval); // ������ ��������� ������ N ������
    }

    private void Update()
    {
        if (!isStanding)
        {
            Move();
        }

        // �������� �� �����
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // ��������, ������� �� ����� ��� ������
        jumpTimer += Time.deltaTime;

        if (isGrounded && jumpTimer >= nextJumpTime)
        {
            // ���������� ������ � ������������� ����� ��������
            jumpTimer = 0f;
            nextJumpTime = Random.Range(minJumpInterval, maxJumpInterval); // ��������� ����� �� ���������� ������

            // ������ � ������
            if (Random.value < jumpChance)
            {
                Jump();
            }
        }
    }

    private void ChangeBehavior()
    {
        if (Random.value < idleProbability)
        {
            Stand();
        }
        else
        {
            Walk();
        }
    }

    private void Stand()
    {
        isStanding = true;
        moveDirection = Vector3.zero;
        animator.SetBool("move", false);
    }

    private void Walk()
    {
        isStanding = false;

        // ���������� ��������� ����������� ��������
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    private void Move()
    {
        if (moveDirection != Vector3.zero)
        {
            // ������� ����
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            animator.SetBool("move", true);

            // ������������ ���� � ������� ��������, ������ �� ��� Y (�� ���������)
            Vector3 targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection); // ������� ���������� ��� ��������
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // ������� �������
            }
        }
    }


    private void Jump()
    {
        if (rb != null)
        {
            // ��������� ���� ������ � Rigidbody
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    // ������ Gizmos ��� �������� �����
    private void OnDrawGizmos()
    {
        // ���� ��������� � ������ ��������������, ������ Gizmos
        if (isGrounded)
        {
            Gizmos.color = Color.green; // ���� ��� ����, ��� ��� �� �����
        }
        else
        {
            Gizmos.color = Color.red; // ���� ��� ����, ��� ��� �� �� �����
        }

        // ������ ����� ���� �� ������� ���� ��� �������� �� �����
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        // ������ ����� �� �����, ��� ���������� ��������
        Gizmos.DrawSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
    }
}
