using UnityEngine;

// ������ Ŭ���̾�Ʈ ���� �ְ���� �Է� ������ ����ü
// RPC�� ���� ���۵Ǿ�� �ϹǷ� INetworkSerializable�� �����մϴ�.
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