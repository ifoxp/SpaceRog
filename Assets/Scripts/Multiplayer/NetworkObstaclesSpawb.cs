using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkObstaclesSpawb : NetworkBehaviour
{
    public GameObject[] PipePrefabs;

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 dir, int who, Vector3 position, Quaternion rotation)
    {
        FireClientRpc(dir, who, position, rotation);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 dir, int who, Vector3 position, Quaternion rotation)
    {
        if (!IsOwner) ExecuteShoot(dir, who, position, rotation);
    }

    private void ExecuteShoot(Vector3 dir, int who, Vector3 position, Quaternion rotation)
    {
        var prefabToInstantiate = PipePrefabs[who];

        var instance = Instantiate(prefabToInstantiate, position, rotation);

        // Отримати компонент EnemyTransform
        var enemyTransform = instance.GetComponent<EnemyTransform>();
        if (enemyTransform != null)
        {
            // Викликати метод, який почне синхронізацію руху противника
            enemyTransform.OnNetworkSpawn();
        }
    }

    public void Gun(int s, Transform Where)
    {
        if (!IsOwner) return;
        var dir = transform.forward;
        // Відправити запит на виконання на всіх клієнтах
        RequestFireServerRpc(dir, s, Where.position, Where.rotation);

        // Вогонь локально негайно
        ExecuteShoot(dir, s, Where.position, Where.rotation);
    }
}
