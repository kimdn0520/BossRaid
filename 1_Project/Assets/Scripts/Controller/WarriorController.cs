using UnityEngine;

public class WarriorController : BaseHero
{
    public override float MoveSpeed { get; set; } = 5f;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); // 부모의 OnNetworkSpawn 로직 실행
        // 전사만의 초기화 코드가 있다면 여기에 추가
    }

    public override void PerformAttack()
    {
        // 이 코드는 서버에서만 실행됩니다.
        Debug.Log("전사: (서버) 도끼로 공격을 실행합니다! 공격 방향: " + LastReceivedInput.LookDirection);

        // 여기에 데미지 판정, 애니메이션 동기화(ClientRpc) 등을 추가
    }
}