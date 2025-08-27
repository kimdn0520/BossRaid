using UnityEngine;

// 서버와 클라이언트 간에 주고받을 입력 데이터 구조체
// RPC를 통해 전송되어야 하므로 INetworkSerializable을 구현합니다.
public struct InputPayload : Unity.Netcode.INetworkSerializable
{
    public Vector2 MoveInput;
    public Vector2 LookDirection;
    public bool IsAttackPressed;
    public bool IsSkillPressed;

    public void NetworkSerialize<T>(Unity.Netcode.BufferSerializer<T> serializer) where T : Unity.Netcode.IReaderWriter
    {
        serializer.SerializeValue(ref MoveInput);
        serializer.SerializeValue(ref LookDirection);
        serializer.SerializeValue(ref IsAttackPressed);
        serializer.SerializeValue(ref IsSkillPressed);
    }
}