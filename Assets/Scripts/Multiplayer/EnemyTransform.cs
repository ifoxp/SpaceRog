using Unity.Netcode;
using UnityEngine;

public class EnemyTransform : NetworkBehaviour
{
    [SerializeField] private float teleportDistance = 0.2f;
    [SerializeField] private float interpolationDistance = 0.1f;

    private NetworkVariable<PlayerNetworkState> enemyState;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyState = new NetworkVariable<PlayerNetworkState>(writePerm: NetworkVariableWritePermission.Server);
    }

    private void Update()
    {
        if (IsServer) TransmitState();
        else ConsumeState();
    }

    #region Transmit State

    private void TransmitState()
    {
        var state = new PlayerNetworkState
        {
            Position = rb.position,
            Rotation = transform.rotation.eulerAngles
        };

        enemyState.Value = state;
    }

    #endregion

    #region Consume State

    private void ConsumeState()
    {
        Vector3 serverPosition = enemyState.Value.Position;
        Vector3 clientPosition = rb.position;

        float deltaX = Mathf.Abs(serverPosition.x - clientPosition.x);
        float deltaY = Mathf.Abs(serverPosition.y - clientPosition.y);

        if (deltaX > teleportDistance || deltaY > teleportDistance)
        {
            rb.position = serverPosition;
        }
        else if (deltaX > interpolationDistance || deltaY > interpolationDistance)
        {
            rb.position = Vector3.Lerp(clientPosition, serverPosition, Time.deltaTime);
        }
    }

    #endregion

    private struct PlayerNetworkState : INetworkSerializable
    {
        private float posX, posY;
        private short rotZ;

        internal Vector3 Position
        {
            get => new Vector3(posX, posY, 0);
            set
            {
                posX = value.x;
                posY = value.y;
            }
        }

        internal Vector3 Rotation
        {
            get => new Vector3(0, 0, rotZ);
            set => rotZ = (short)value.z;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref posX);
            serializer.SerializeValue(ref posY);
            serializer.SerializeValue(ref rotZ);
        }
    }
}
