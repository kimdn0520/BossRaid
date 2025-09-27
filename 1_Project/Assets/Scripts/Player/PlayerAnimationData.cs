using UnityEngine;

public class PlayerAnimationData
{
    public int IsMoving { get; private set; }

    public void Initialize()
    {
        IsMoving = Animator.StringToHash("isMoving");
    }
}