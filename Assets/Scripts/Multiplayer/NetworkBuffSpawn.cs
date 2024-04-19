using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkBuffSpawn : NetworkBehaviour
{

    public GameObject[] PipePrefabs;
    private Transform WhereSpawn;
    [ServerRpc]
    private void RequestFireServerRpc(Vector3 dir, int who, float rb, float rot, float moveDirection)
    {
        FireClientRpc(dir, who, rb, rot, moveDirection);
    }

    [ClientRpc]
    private void FireClientRpc(Vector3 dir, int who, float rb, float rot, float moveDirection)
    {
        if (!IsOwner) ExecuteShoot(dir, who, rb, rot, moveDirection);
    }

    private void ExecuteShoot(Vector3 dir, int who, float rb, float rot, float moveDirection)
    {
        var prefabToInstantiate = PipePrefabs[who];

        var instance = Instantiate(prefabToInstantiate, WhereSpawn);
        BuffBrush b = instance.GetComponent<BuffBrush>();
        b.MoveClient = howBool;
        b.mover = rb;
        b.moveDirection = moveDirection;
        b.angle = rot;
    }

    private bool howBool = false;
    private float rb, rot, moveDirection;

    public void Gun(int s)
    {
        if (!IsOwner) return;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float visibleWidth = Camera.main.orthographicSize * 2.0f * screenWidth / screenHeight;
        visibleWidth /= 2;
        rb = Random.Range(-visibleWidth + 1, visibleWidth - 1);
        moveDirection = transform.position.x < 0f ? 1f : -1f;
        rot = transform.position.x < 0f ? Random.Range(-90f, -90f + 30) : Random.Range(-30 + 90f, 90f);

        
        var dir = transform.forward;

        // Send off the request to be executed on all clients
        RequestFireServerRpc(dir, s, rb, rot, moveDirection);

        // Fire locally immediately
        ExecuteShoot(dir, s, rb, rot, moveDirection);
    }
}
