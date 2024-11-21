using UnityEngine;

public class BotBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Скорость движения бота.")]
    public float moveSpeed = 2f;

    [Tooltip("Время, через которое бот меняет своё поведение.")]
    public float behaviorChangeInterval = 5f;

    [Tooltip("Вероятность того, что бот будет стоять на месте (от 0 до 1).")]
    [Range(0f, 1f)]
    public float idleProbability = 0.7f;

    [Tooltip("Скорость поворота бота.")]
    public float rotationSpeed = 5f;

    [Header("Jump Settings")]
    [Tooltip("Сила прыжка.")]
    public float jumpForce = 5f;

    [Tooltip("Проверка на землю. Слой, в котором происходит проверка.")]
    public LayerMask groundLayer;

    [Tooltip("Дистанция для проверки на землю.")]
    public float groundCheckDistance = 0.1f;

    [Tooltip("Минимальное время между прыжками.")]
    public float minJumpInterval = 2f; // минимальный интервал между прыжками

    [Tooltip("Максимальное время между прыжками.")]
    public float maxJumpInterval = 5f; // максимальный интервал между прыжками

    [Tooltip("Шанс на прыжок. Например, 0.1 — 10% шанс.")]
    [Range(0f, 1f)]
    public float jumpChance = 0.1f;

    [Header("Hand Settings")]
    [Tooltip("Точка крепления объекта в руках бота.")]
    public Transform handPoint;

    private Vector3 moveDirection;
    private bool isStanding;
    private bool isGrounded;
    private float jumpTimer; // Таймер для прыжков
    private float nextJumpTime; // Время следующего возможного прыжка
    private Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // Получаем Rigidbody для физики
        InvokeRepeating(nameof(ChangeBehavior), 0f, behaviorChangeInterval); // Меняем поведение каждые N секунд
    }

    private void Update()
    {
        if (!isStanding)
        {
            Move();
        }

        // Проверка на землю
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        // Проверка, настало ли время для прыжка
        jumpTimer += Time.deltaTime;

        if (isGrounded && jumpTimer >= nextJumpTime)
        {
            // Сбрасываем таймер и устанавливаем новый интервал
            jumpTimer = 0f;
            nextJumpTime = Random.Range(minJumpInterval, maxJumpInterval); // случайное время до следующего прыжка

            // Прыжок с шансом
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

        // Генерируем случайное направление движения
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    private void Move()
    {
        if (moveDirection != Vector3.zero)
        {
            // Двигаем бота
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            animator.SetBool("move", true);

            // Поворачиваем бота в сторону движения, только по оси Y (по вертикали)
            Vector3 targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection); // Создаем кватернион для поворота
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Плавный поворот
            }
        }
    }


    private void Jump()
    {
        if (rb != null)
        {
            // Применяем силу прыжка к Rigidbody
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    // Рисуем Gizmos для проверки земли
    private void OnDrawGizmos()
    {
        // Если находимся в режиме редактирования, рисуем Gizmos
        if (isGrounded)
        {
            Gizmos.color = Color.green; // Цвет для того, что бот на земле
        }
        else
        {
            Gizmos.color = Color.red; // Цвет для того, что бот не на земле
        }

        // Рисуем линию вниз от позиции бота для проверки на землю
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);

        // Рисуем сферу на месте, где происходит проверка
        Gizmos.DrawSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);
    }
}
