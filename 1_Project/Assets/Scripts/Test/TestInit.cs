using UnityEngine;

public class TestInit : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private HandRankDataSO handRankData;

    private void Awake()
    {
        SpriteManager.Instance.Initialize();

        PoolManager.Instance.Initialize();

        PokerEvaluator.Initialize(handRankData);
    }
}