using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// 해당 상태를 시작할 때 1회 호출한다.
    /// </summary>
    void Enter();

    /// <summary>
    /// 해당 상태를 FixedUpdate할 때 고정된 프레임에 호출
    /// </summary>
    void PhysicsExecute();

    /// <summary>
    /// 해당 상태를 종료할 때 1회 호출
    /// </summary>
    void Exit();
}