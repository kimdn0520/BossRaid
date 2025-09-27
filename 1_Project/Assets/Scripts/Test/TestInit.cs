using UnityEngine;

public class TestInit : MonoBehaviour
{
    private void Awake()
    {
        SpriteManager.Instance.Initialize();

        PoolManager.Instance.Initialize();
    }
}