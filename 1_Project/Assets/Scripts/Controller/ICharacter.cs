using Unity.Netcode;
using UnityEngine;

public interface ICharacter
{
    void Attack();
    void UseSkill(int skillIndex);
}
