using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    /// <summary>
    /// 해당 상태를 시작할 때 1회 호출한다.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// 해당 상태를 Update할 때 매 프레임에 호출
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// 해당 상태를 FixedUpdate할 때 고정된 프레임에 호출
    /// </summary>
    public abstract void PhysicsExecute();

    /// <summary>
    /// 해당 상태를 종료할 때 1회 호출
    /// </summary>
    public abstract void Exit();
}