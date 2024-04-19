using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Localization.Platform.Android;
using UnityEngine;

public class NetworkObstaclesSpawb : NetworkBehaviour
{
    public GameObject[] PipePrefabs;

    private float[] x = { 0, 0 };

    [ServerRpc]
    private void RequestFireServerRpc(Vector3 dir, int who, float x1, float x2, Vector3 position, Quaternion rotation)
    {
        FireClientRpc(dir, who, x1, x2, position, rotation);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 dir, int who, float x1, float x2, Vector3 position, Quaternion rotation)
    {
        if (!IsOwner) ExecuteShoot(dir, who, x1, x2, position, rotation);
    }

    private void ExecuteShoot(Vector3 dir, int who, float x1, float x2, Vector3 position, Quaternion rotation)
    {
        var prefabToInstantiate = PipePrefabs[who];

        var instance = Instantiate(prefabToInstantiate, position, rotation);

            IsEnemyNewcomers(instance, x1, x2);
    }

    public void Gun(int s, Transform Where)
    {
        if (!IsOwner) return;
        var dir = transform.forward;
        x[0] = Random.Range(-12, 0);
        x[1] = Random.Range(0, 12);

        // Send off the request to be executed on all clients
        RequestFireServerRpc(dir, s, x[0], x[1], Where.position, Where.rotation);

        // Fire locally immediately
        ExecuteShoot(dir, s, x[0], x[1], Where.position, Where.rotation);
    }

    private void IsEnemyNewcomers(GameObject pref, float x1, float x2)
    {
        EnemyNewcomers b = pref.GetComponent<EnemyNewcomers>();
        float[] IndexD = new float[3];
        b.isClient = true;
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
        b.speedRotate += IndexD[0];
        b.speedHorizontal += IndexD[1];
        b.speedVertical += IndexD[2];
        if (b.targetPosition.x > 0)
            b.enemy.transform.position = new Vector3(x1, b.enemy.transform.position.y, b.enemy.transform.position.z);
        else
            b.enemy.transform.position = new Vector3(x2, b.enemy.transform.position.y, b.enemy.transform.position.z);

        if (b.player.position.y > 0)
            b.enemy.transform.position = new Vector3(b.enemy.transform.position.x, -5, b.enemy.transform.position.z);
        else
            b.enemy.transform.position = new Vector3(b.enemy.transform.position.x, 5, b.enemy.transform.position.z);
    }
}
