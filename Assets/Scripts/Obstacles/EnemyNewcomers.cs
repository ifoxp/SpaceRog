using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyNewcomers : NetworkBehaviour
{
    [SerializeField] private int health = 2;  // Кількість життя
    [SerializeField] private GameObject explosionPrefab;  // Префаб вибуху
    private Rigidbody2D rb;                     // Rigidbody2D компонент ворожого корабля
    public Vector2 targetPosition;             // Позиція, до якої корабель рухається
    private float timer = 1.75f;                // Таймер для контролю стрільби
    public Transform player;                   // Посилання на об'єкт гравця
    public float speedRotate = 5f;   // Швидкість обертання корабля
    private float screenWidth, screenHeight;
    private float visibleWidth, visibleHeight;

    public float speedHorizontal;    // Швидкість руху корабля по горизонталі
    public float speedVertical;      // Швидкість руху корабля по вертикалі

    public GameObject enemy;         // Сам корабель (потрібний для вимкнення коллайдера при старті)
    [SerializeField] private int scoreForDead = 3;     // Кількість очок, які гравець отримує за знищення ворога
    [SerializeField] private int maxMoneyForDead = 2;     // Кількість грошей, які гравець отримує за знищення ворога
    [SerializeField] private Transform enemyR;         // Transform компонент корабля, що відповідає за його обертання

    [SerializeField] private GameObject coinPrefab; // Префаб монетки
    [SerializeField] private Vector3 coinScale = new Vector3(1f, 1f, 1f); // Розмір монети
    private bool leave = true;
    private Spawner spawner;

    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource gunshotAudioSource;


    public bool isClient=false;
    private void Start()
    {
        float[] IndexD = new float[3];
        if (!isClient)
        {
            if (PlayerPrefs.GetInt("Difficult") == 0)
            {
                IndexD[0] = -1;
                IndexD[1] = -1.5f;
                IndexD[2] = -1.5f;
            }
            else if (PlayerPrefs.GetInt("Difficult") == 2)
            {
                IndexD[0] = 1;
                IndexD[1] = 1.5f;
                IndexD[2] = 1.5f;
            }
            else
            {
                IndexD[0] = 0;
                IndexD[1] = 0;
                IndexD[2] = 0;
            }
        }
        speedRotate += IndexD[0];
        speedHorizontal += IndexD[1];
        speedVertical += IndexD[2];

        // Отримуємо посилання на об'єкт класу Spawner
        spawner = FindObjectOfType<Spawner>();
        rb = GetComponent<Rigidbody2D>();                     // Отримуємо посилання на Rigidbody2D
        
        player = GameObject.FindGameObjectWithTag("Player").transform;   // Отримуємо посилання на гравця за тегом "Player"
        if (isClient)
        {
            RandomPlayerAgr arg = GetComponent<RandomPlayerAgr>();
            InvokeRepeating("arg.ChangePlayer(ref player)", 0f, 1f);
        }
        SetRandomTargetPosition();


        // Встановлюємо початкову позицію корабля залежно від цільової позиції
        if (!isClient)
        {
            if (targetPosition.x > 0)
                enemy.transform.position = new Vector3(Random.Range(-12, 0), enemy.transform.position.y, enemy.transform.position.z);
            else
                enemy.transform.position = new Vector3(Random.Range(0, 12), enemy.transform.position.y, enemy.transform.position.z);

            if (player.position.y > 0)
                enemy.transform.position = new Vector3(enemy.transform.position.x, -5, enemy.transform.position.z);
            else
                enemy.transform.position = new Vector3(enemy.transform.position.x, 5, enemy.transform.position.z);
        }


        AudioSource foundAudioSource = FindAudioSource("Sounds");
        if (foundAudioSource != null)
        {
            gunshotAudioSource = foundAudioSource;
        }
        // Получить размеры экрана
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        // Получить размеры видимой области
        visibleWidth = Camera.main.orthographicSize * 2.0f * screenWidth / screenHeight / 2;
        visibleHeight = Camera.main.orthographicSize * 2.0f / 2;

        // Вывести результаты
        float x1 = (visibleWidth * (-1)) + 1.5f, x2 = (visibleHeight * (-1)) + 1.5f;
        LookAtPlayer(100);// Направляємо корабель на гравця при старті
    }
    private AudioSource FindAudioSource(string objectName)
    {
        AudioController[] musicManagers = FindObjectsOfType<AudioController>();
        foreach (AudioController musicManager in musicManagers)
        {
            if (musicManager.gameObject.name == objectName)
            {
                return musicManager.audio_;
            }
        }
        return null;
    }

    private void FixedUpdate()
    {
        
        MoveToTargetPosition();        // Рухаємо корабель до цільової позиції
        if (Mathf.Abs(enemyR.position.x) < visibleWidth && Mathf.Abs(enemyR.position.y) < visibleHeight)
        {
            if (leave)
                enemy.GetComponent<CapsuleCollider2D>().enabled = true;   // Вмикнення коллайдера, коли корабель знаходиться в певному діапазоні позицій
        }
        LookAtPlayer(1);                  // Направляємо корабель на гравця
    }
    private void SetRandomTargetPosition()
    {
        targetPosition = new Vector2(Random.Range((visibleWidth * (-1)) + 1.5f, (visibleWidth * (1)) - 1.5f), Random.Range((visibleHeight * (-1)) + 1.5f, (visibleHeight * (1)) - 1.5f));   // Встановлюємо випадкову цільову позицію в межах заданих значень
    }
    private void MoveToTargetPosition()
    {
        Vector2 currentPosition = transform.position;           // Поточна позиція корабля
        Vector2 forwardDirection = enemyR.up;                   // Напрямок, в який дивиться корабель
        Vector2 movement = forwardDirection * speedVertical;    // Вектор руху вперед

        rb.velocity = movement;                                 // Встановлюємо швидкість руху корабля
    }

    private void LookAtPlayer(int speed)
    {
        Vector2 direction = player.position - transform.position;  // Вектор напрямку до гравця
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;   // Кут між вектором напрямку і віссю X з відповідним зсувом

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);     // Обертання
        enemyR.rotation = Quaternion.Slerp(enemyR.rotation, rotation, speedRotate * speed * Time.deltaTime);   // Плавне обертання корабля
    }
    private void PlayDeathSound()
    {
        if (deathSound != null && gunshotAudioSource != null)
        {
            gunshotAudioSource.PlayOneShot(deathSound);
        }
    }
    private void SpawnCoins(int count)
    {
        float minX = -0.4f; // Мінімальне значення по x
        float maxX = 0.4f;  // Максимальне значення по x
        float minY = -0.1f; // Мінімальне значення по y
        float maxY = 0.2f;  // Максимальне значення по y
        if (count > 0)
        {
            GameObject[] coins = new GameObject[count]; // Масив для збереження створених монет

            for (int i = 0; i < count; i++)
            {
                // Генеруємо випадкові координати для монети в заданих проміжках
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);

                // Створюємо монету з випадковими координатами
                GameObject coinInstance = Instantiate(coinPrefab, transform.position + new Vector3(randomX, randomY, 0f), Quaternion.identity);
                coinInstance.transform.localScale = coinScale; // Задаємо розмір монети
                coins[i] = coinInstance; // Додаємо монету до масиву

                // Збільшуємо кількість грошей у ScoreManager
                ScoreManager.Instance.SetMoney(1);
                PlayerPrefs.SetInt("MoneyDay" + $"{PlayerPrefs.GetInt("Difficult")}", PlayerPrefs.GetInt("MoneyDay" + $"{PlayerPrefs.GetInt("Difficult")}") + 1);
                PlayerPrefs.SetInt("MaxMoney" + $"{PlayerPrefs.GetInt("Difficult")}", PlayerPrefs.GetInt("MaxMoney" + $"{PlayerPrefs.GetInt("Difficult")}") + 1);
            }

            // Видалення монет окремо
            foreach (GameObject coin in coins)
            {
                Destroy(coin, 0.4f); // Знищуємо кожну монету через 0.4 секунди
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        health--;  // Зменшуємо кількість життя

        if (health <= 0)
        {
            if (other.gameObject.CompareTag("Bullet") && leave)
            {
                leave = false;
                // enemy.GetComponent<CapsuleCollider2D>().enabled = false;
                // Отримуємо кут повороту ворожого корабля
                float enemyRotation = Random.Range(0, 360);

                // Створюємо екземпляр анімації вибуху з правильним поворотом
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.Euler(0f, 0f, enemyRotation));
                PlayDeathSound();
                Destroy(explosion, 0.6f);  // Знищуємо анімацію вибуху через 0.75 секунди


                Destroy(gameObject);  // Знищуємо поточний екземпляр ворожого корабля
                ScoreManager.Instance.SetScore(scoreForDead);
                int t = Random.Range(0, (maxMoneyForDead + PlayerPrefs.GetInt("Difficult")));

                SpawnCoins(t);
                PlayerPrefs.SetInt("Enemys" + $"{PlayerPrefs.GetInt("Difficult")}", PlayerPrefs.GetInt("Enemys" + $"{PlayerPrefs.GetInt("Difficult")}") + 1);
                PlayerPrefs.SetInt("EnemysDay" + $"{PlayerPrefs.GetInt("Difficult")}", PlayerPrefs.GetInt("EnemysDay" + $"{PlayerPrefs.GetInt("Difficult")}") + 1);

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Polia"))
        {
            enemy.GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }
}
