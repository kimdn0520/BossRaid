using UnityEngine;

public class WarriorController : BaseHero
{
    public override float MoveSpeed { get; set; } = 5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); // �θ��� OnNetworkSpawn ���� ����
        // ���縸�� �ʱ�ȭ �ڵ尡 �ִٸ� ���⿡ �߰�
    }

    public override void PerformAttack()
    {
        // �� �ڵ�� ���������� ����˴ϴ�.
        Debug.Log("����: (����) ������ ������ �����մϴ�! ���� ����: " + LastReceivedInput.LookDirection);

        // ���⿡ ������ ����, �ִϸ��̼� ����ȭ(ClientRpc) ���� �߰�
    }
}