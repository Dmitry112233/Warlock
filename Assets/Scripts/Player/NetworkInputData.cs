using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementInput;
    public Vector3 aimInput;
    public Vector3 fireInput;
    public NetworkBool isFireBallButtonPresed;
    public NetworkBool isStompButtonPresed;
}
