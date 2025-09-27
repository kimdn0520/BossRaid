using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreServeGameObject : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
