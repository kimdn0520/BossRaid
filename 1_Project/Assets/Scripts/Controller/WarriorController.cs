using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : BaseHero
{
    public override HeroType HeroClass => HeroType.Warrior;
    public override int Defense { get; set; } = 5;
    public override int Shield { get; set; } = 0;
    public override float MoveSpeed { get; set; } = 5.0f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnNetworkSpawn()
    {

    }

    public override void MoveCharacter()
    {
        base.MoveCharacter();
    }

    public override void AddShield(int amount)
    {

    }

    public override void Attack(Vector2 direction)
    {

    }

    public override void TakeDamage(int amount)
    {

    }

    public override void UseSkill(int skillIndex, Vector2 direction)
    {
        
    }

    public override async UniTask DashAsync()
    {
        //float dashDistance = 5f;
        //Vector3 dashDirection = movementInput.normalized;
        //Vector3 startPosition = transform.position;
        //Vector3 targetPosition = startPosition + dashDirection * dashDistance;

        //float duration = 0.2f;  // Dash 지속 시간
        //float elapsedTime = 0f;

        //while (elapsedTime < duration)
        //{
        //    transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
        //    elapsedTime += Time.deltaTime;
        //    await UniTask.Yield();
        //}

        //transform.position = targetPosition;

        await UniTask.Yield();
    }
}
