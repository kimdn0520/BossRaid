using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSwitcher : MonoBehaviour
{
    [SerializeField] BoxCollider2D layerCollider;
    [SerializeField] SpriteRenderer[] renderers;

    public Common.SLayerDirection direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("SLayer")) return;

        var slayer = other.GetComponent<SLayer>();

        if (slayer.direction == Common.SLayerDirection.South && other.transform.position.y > transform.position.y) SetLayerAndSortingLayer(slayer.layerUpper, slayer.sortingLayerUpper);
        else
        if (slayer.direction == Common.SLayerDirection.West && other.transform.position.x > transform.position.x) SetLayerAndSortingLayer(slayer.layerUpper, slayer.sortingLayerUpper);
        else
        if (slayer.direction == Common.SLayerDirection.East && other.transform.position.x < transform.position.x) SetLayerAndSortingLayer(slayer.layerUpper, slayer.sortingLayerUpper);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("SLayer")) return;

        var slayer = other.GetComponent<SLayer>();

        if (slayer.direction == Common.SLayerDirection.South && other.transform.position.y > transform.position.y) SetLayerAndSortingLayer(slayer.layerLower, slayer.sortingLayerLower);
        else
        if (slayer.direction == Common.SLayerDirection.West && other.transform.position.x > transform.position.x) SetLayerAndSortingLayer(slayer.layerLower, slayer.sortingLayerLower);
        else
        if (slayer.direction == Common.SLayerDirection.East && other.transform.position.x < transform.position.x) SetLayerAndSortingLayer(slayer.layerLower, slayer.sortingLayerLower);
    }

    private void SetLayerAndSortingLayer(string layer, string sortingLayer)
    {
        layerCollider.gameObject.layer = LayerMask.NameToLayer(layer);

        foreach (SpriteRenderer sr in renderers)
        {
            sr.sortingLayerName = sortingLayer;
        }
    }
}
