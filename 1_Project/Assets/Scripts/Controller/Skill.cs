using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    private bool isLearned = false;

    public void LearnSkill()
    {
        if (isLearned)
        {
            Debug.Log("이미 배운 스킬입니다.");
        }

        isLearned = true;
    }
}
