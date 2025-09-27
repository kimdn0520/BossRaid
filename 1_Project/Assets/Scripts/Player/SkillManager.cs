using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private Dictionary<KeyCode, ISkill> _equippedSkills = new Dictionary<KeyCode, ISkill>();
    private List<ISkill> _allSkills = new List<ISkill>(); // 쿨타임 계산을 위한 모든 스킬 리스트

    void Update()
    {
        // 매 프레임 모든 스킬의 쿨타임을 돌려줍니다.
        foreach (var skill in _allSkills)
        {
            skill.Tick();
        }
    }

    /// <summary>
    /// 특정 키에 스킬을 장착(교체)하는 함수
    /// </summary>
    public void EquipSkill(KeyCode key, ISkill skill)
    {
        // 기존에 있던 스킬은 쿨타임 계산 리스트에서 제거
        if (_equippedSkills.ContainsKey(key) && _equippedSkills[key] != null)
        {
            _allSkills.Remove(_equippedSkills[key]);
        }

        Debug.Log($"{key}에 {skill.GetType().Name} 스킬 장착!");
        _equippedSkills[key] = skill;
        _allSkills.Add(skill);
    }

    /// <summary>
    /// 스킬 사용을 시도하는 함수
    /// </summary>
    public void TryUseSkill(KeyCode key)
    {
        if (_equippedSkills.TryGetValue(key, out ISkill skill) && skill.IsReady)
        {
            Debug.Log($"{key} 키의 {skill.GetType().Name} 스킬 사용!");
            skill.Use(gameObject); // gameObject는 이 컴포넌트가 붙어있는 플레이어 자신
        }
        else
        {
            Debug.Log($"{key} 스킬을 사용할 수 없습니다. (쿨타임이거나 장착되지 않음)");
        }
    }
}