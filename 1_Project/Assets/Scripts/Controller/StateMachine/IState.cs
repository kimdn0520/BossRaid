using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IState
{
    /// <summary>
    /// �ش� ���¸� ������ �� 1ȸ ȣ���Ѵ�.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// �ش� ���¸� Update�� �� �� �����ӿ� ȣ��
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// �ش� ���¸� FixedUpdate�� �� ������ �����ӿ� ȣ��
    /// </summary>
    public abstract void PhysicsExecute();

    /// <summary>
    /// �ش� ���¸� ������ �� 1ȸ ȣ��
    /// </summary>
    public abstract void Exit();
}