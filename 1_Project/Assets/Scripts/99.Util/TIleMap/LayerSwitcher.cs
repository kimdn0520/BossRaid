using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitcher : MonoBehaviour
{
    public enum Direction
    {
        North,
        South,
        West,
        East
    }

    // Stair 방향 설정
    public Direction direction;

    [Space]
    public string layerUpper;
    public string sortingLayerUpper;
    
    [Space]
    public string layerLower;
    public string sortingLayerLower;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // position 말고 좋은게 없을까? collider 자체로?..
        if (direction == Direction.South && other.transform.position.y < transform.position.y) SetLayerAndSortingLayer(other.gameObject, layerUpper, sortingLayerUpper);
        else
        if (direction == Direction.West && other.transform.position.x < transform.position.x) SetLayerAndSortingLayer(other.gameObject, layerUpper, sortingLayerUpper);
        else
        if (direction == Direction.East && other.transform.position.x > transform.position.x) SetLayerAndSortingLayer(other.gameObject, layerUpper, sortingLayerUpper);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (direction == Direction.South && other.transform.position.y < transform.position.y) SetLayerAndSortingLayer(other.gameObject, layerLower, sortingLayerLower);
        else
        if (direction == Direction.West && other.transform.position.x < transform.position.x) SetLayerAndSortingLayer(other.gameObject, layerLower, sortingLayerLower);
        else
        if (direction == Direction.East && other.transform.position.x > transform.position.x) SetLayerAndSortingLayer(other.gameObject, layerLower, sortingLayerLower);
    }

    private void SetLayerAndSortingLayer(GameObject target, string layer, string sortingLayer)
    {
        // 1. 대상의 물리 레이어를 변경합니다. (충돌 처리용)
        target.layer = LayerMask.NameToLayer(layer);

        // 2. 대상과 그 모든 자식들에 있는 SpriteRenderer를 찾아서 정렬 레이어를 변경합니다. (보이는 순서용)
        SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        
        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingLayerName = sortingLayer;
        }
    }
}
