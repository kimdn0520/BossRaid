using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PoolManager : SingletonMonoBehaviour<PoolManager>
{
    private static PoolManager _instance;

    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<PoolManager>();

                // 씬에 없다면, 새로 생성합니다. (기존 로직 유지)
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PoolManager");
                    _instance = obj.AddComponent<PoolManager>();
                }
            }

            return _instance;
        }
    }

    private readonly Dictionary<string, PoolContainer> pools = new Dictionary<string, PoolContainer>();

    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized) return;

        GameObject containerMaster = new GameObject("PoolContainer_Master");
        DontDestroyOnLoad(containerMaster);

        var poolSO = Resources.Load<PoolSO>("PoolSO");

        if (poolSO != null)
        {
            foreach (var preset in poolSO.presets)
            {
                // 이름이 비어있으면 프리팹 이름으로 대체
                string poolName = string.IsNullOrWhiteSpace(preset.name) ? preset.prefab.name : preset.name;

                if (pools.ContainsKey(poolName))
                {
                    Debug.LogWarning($"'{poolName}' 이름을 가진 풀컨테이너가 이미 존재합니다. 건너뜁니다.");
                    continue;
                }

                Transform containerTr = new GameObject($"{poolName} Container").transform;
                containerTr.SetParent(containerMaster.transform, false);

                pools[poolName] = new PoolContainer(poolName, preset.prefab, preset.initialCount, containerTr);
            }
        }

        isInitialized = true;
    }

    public T Get<T>(string poolName, Transform parent = null, Vector3? position = null, Quaternion? rotation = null) where T : Component
    {
        GameObject obj = Get(poolName, parent, position, rotation);
        if (obj == null) return null;

        if (obj.TryGetComponent<T>(out T component))
        {
            return component;
        }
        else
        {
            Debug.LogError($"'{poolName}' 풀의 프리팹에 '{typeof(T)}' 컴포넌트가 없습니다.");
            Return(obj);
            return null;
        }
    }

    public GameObject Get(string poolName, Transform parent = null, Vector3? position = null, Quaternion? rotation = null)
    {
        if (!isInitialized)
        {
            Debug.LogError("PoolManager가 초기화되지 않았습니다.");
            return null;
        }

        if (!pools.ContainsKey(poolName))
        {
            Debug.LogError($"'{poolName}' 이름을 가진 풀을 찾을 수 없습니다.");
            return null;
        }

        Vector3 finalPos = position ?? Vector3.zero;
        Quaternion finalRot = rotation ?? Quaternion.identity;

        GameObject obj = pools[poolName].Get();
        obj.transform.SetParent(parent, false);
        obj.transform.SetPositionAndRotation(finalPos, finalRot);

        return obj;
    }

    public void Return(GameObject obj)
    {
        // PoolObject 컴포넌트를 통해 어떤 풀에 속해있는지 확인합니다.
        if (!obj.TryGetComponent<PoolObject>(out var poolObj))
        {
            Debug.LogError($"'{obj.name}'에는 PoolObject 컴포넌트가 없어 풀로 반환할 수 없습니다. 즉시 파괴합니다.");
            Destroy(obj);
            return;
        }

        if (!pools.ContainsKey(poolObj.poolName))
        {
            Debug.LogError($"'{poolObj.poolName}' 풀을 찾을 수 없어 '{obj.name}'을 반환할 수 없습니다. 즉시 파괴합니다.");
            Destroy(obj);
            return;
        }

        pools[poolObj.poolName].Return(obj);
    }
}
