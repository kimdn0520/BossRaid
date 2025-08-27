using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// �ش� ���¸� ������ �� 1ȸ ȣ���Ѵ�.
    /// </summary>
    void Enter();

    /// <summary>
    /// �ش� ���¸� FixedUpdate�� �� ������ �����ӿ� ȣ��
    /// </summary>
    void PhysicsExecute();

    /// <summary>
    /// �ش� ���¸� ������ �� 1ȸ ȣ��
    /// </summary>
    void Exit();
}