using System.Collections;
using UnityEngine;

public class RandomPlayerAgr : MonoBehaviour
{

    public Transform player;
    private int m1 = 0, m2 = 1;

    public void ChangePlayer(ref Transform plTransform)
    {
        // Вибрати випадкового гравця
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Вибираємо перший активний Transform
        foreach (GameObject playerObject in players)
        {
            Transform collider = playerObject.transform;

            // Встановлюємо знайдений активний Transform
            if (m1 == m2)
            {
                plTransform = collider;
                // Завершуємо цикл, оскільки ми знайшли активний Transform
                break;
            }

            if (m2 == 1)
                m2 = 0;
            else
                m2 = 1;
        }
    }
}
