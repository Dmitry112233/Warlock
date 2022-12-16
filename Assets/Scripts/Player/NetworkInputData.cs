using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector3 mevementInput;
    public Vector3 aimInput;
    public Vector3 fireInput;
    public NetworkBool isFireBallButtonPresed;
}
