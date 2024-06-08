using UnityEngine;
using UnityEngine.SceneManagement;

public class MoverBackground : MonoBehaviour
{
    public float moveDistanceX = 24f; // Відстань руху по X
    public float moveSpeed = 1f; // Швидкість руху

    private Transform background1;
    private Transform background2;
    [SerializeField] private GameObject Spawner;

    private void Start()
    {
        background1 = transform.Find("Background1");
        background2 = transform.Find("Background2");

        // Завантажуємо координати з BackgroundData
        background1.position = new Vector3(BackgroundData.Instance.startX1, background1.position.y, background1.position.z);
        background2.position = new Vector3(BackgroundData.Instance.startX2, background2.position.y, background2.position.z);
    }

    private void FixedUpdate()
    {
        MoveBackground();
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Перевірка, чи індекс активної сцени не дорівнює 1
        if (activeSceneIndex == 1 || activeSceneIndex == 8)
        {
            Spawner.SetActive(false);
        }
        else
            Spawner.SetActive(true);
    }

    private void MoveBackground()
    {
        // Рухаємо фони
        Vector3 targetPosition1 = new Vector3(background1.position.x - moveDistanceX, background1.position.y, background1.position.z);
        Vector3 targetPosition2 = new Vector3(background2.position.x - moveDistanceX, background2.position.y, background2.position.z);
        float step = moveSpeed * Time.fixedDeltaTime;
        background1.position = Vector3.MoveTowards(background1.position, targetPosition1, step);
        background2.position = Vector3.MoveTowards(background2.position, targetPosition2, step);

        // Перевіряємо, чи досягнута кінцева позиція першого фону, і перезапускаємо його
        if (background1.position.x <= BackgroundData.Instance.startX1 - moveDistanceX)
        {
            background1.position = new Vector3(BackgroundData.Instance.startX1, background1.position.y, background1.position.z);
        }

        // Перевіряємо, чи досягнута кінцева позиція другого фону, і перезапускаємо його
        if (background2.position.x <= BackgroundData.Instance.startX2 - moveDistanceX)
        {
            background2.position = new Vector3(BackgroundData.Instance.startX2, background2.position.y, background2.position.z);
        }
    }
}
