using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : SingletonMonoBehaviour<ObjectPoolManager>
{
    private class Pool
    {
        private Queue<PoolObject> pooledObjs = new Queue<PoolObject>();
        private GameObject prefab;
        private Transform poolTr;
        private string poolName;

        public Pool(string poolName, GameObject prefab, Transform poolTr, int initialCount)
        {
            this.poolName = poolName;
            this.prefab = prefab;
            this.poolTr = poolTr;
            for (var index = 0; index < initialCount; index++)
            {
                AddToPool(NewObject());
            }
        }

        public void Clear()
        {
            ClearPooledObjects();
            pooledObjs = null;

            Destroy(prefab);
            prefab = null;

            Destroy(poolTr.gameObject);
            poolTr = null;
        }

        public void ClearPooledObjects()
        {
            while (pooledObjs.Count > 0)
            {
                var poolObj = pooledObjs.Dequeue();

                Destroy(poolObj.gameObject);
            }
        }

        PoolObject NewObject()
        {
            GameObject go = Instantiate(prefab);

            var poolObj = go.GetComponent<PoolObject>();

            if (poolObj == null)
                poolObj = go.AddComponent<PoolObject>();

            poolObj.poolName = poolName;

            return poolObj;
        }

        public void AddToPool(PoolObject poolObj)
        {
            if (poolObj.poolName != poolName)
            {
                Debug.LogError("Cannot add obj to other pool!");
                return;
            }

            if (poolObj.isPooled)
            {
                Debug.LogError("Cannot add already pooled object!");
                return;
            }

            poolObj.gameObject.SetActive(false);
            poolObj.transform.SetParent(poolTr, false);
            pooledObjs.Enqueue(poolObj);
            poolObj.isPooled = true;
        }

        public GameObject GetObject(Transform parent)
        {
            PoolObject poolObj = pooledObjs.Count > 0 ? pooledObjs.Dequeue() : NewObject();
            poolObj.isPooled = false;
            poolObj.transform.SetParent(parent, false);
            poolObj.gameObject.SetActive(true);
            
            return poolObj.gameObject;
        }
    }

    [SerializeField] private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

    protected override void Awake()
    {
        base.Awake();

        var pref = Resources.Load<ObjectPoolData>("ObjectPoolingData");

        //if (pref != null)
        //{
        //    for (var i = 0; i < pref.presets.Length; ++i)
        //    {
        //        var preset = pref.presets[i];

        //        if (string.IsNullOrWhiteSpace(preset.name))
        //        {
        //            preset.name = preset.prefab.name;
        //        }

        //        var poolTr = new GameObject($"{preset.name} Object Pool").transform;
                
        //        poolTr.SetParent(transform, false);
                
        //        pools.Add(preset.name, new Pool(preset.name, preset.prefab, poolTr, preset.initialCount));
        //    }
        //}
    }

    //public static T Spawn<T>(Transform parent, Vector3 position, Quaternion rotation) where T : Component
    //{
    //    // 1. ObjectPool에 있는지 확인하고 가져옴.

    //    // 2. 없다면 Resources.Load로 프리팹을 가져와서 Instantiate를 해주고 allPoolDictionary에 넣어준다.

    //    // ex) 
    //    // ObjectPool
    //    //    Enemy Pool(자식)
    //    //         Enemy (Enemy Pool의 자식)
    //    //    Bullet Pool (자식)
    //    //         Bullet (Bullet Pool의 자식) 
    //    // ... 
    //    // 이런형태로 하고싶어


    //    string prefabName = typeof(T).Name;
    //    GameObject prefab = Resources.Load<GameObject>(prefabName);
    //    if (prefab == null)
    //    {
    //        Debug.Assert(false, $"Prefab for {typeof(T).Name} does not exist.");
    //        return null;
    //    }

    //    if (!allPoolDictionary.ContainsKey(prefab))
    //    {
    //        //CreatePool(prefab, 1);
    //    }

    //    //Queue<GameObject> objectPool = allPoolDictionary[prefab];
    //    GameObject obj;

    //    if (objectPool.Count > 0)
    //    {
    //        obj = objectPool.Dequeue();
    //    }
    //    else
    //    {
    //        obj = Object.Instantiate(prefab);
    //    }

    //    obj.transform.SetParent(parent);
    //    obj.transform.position = position;
    //    obj.transform.rotation = rotation;
    //    obj.SetActive(true);

    //    return obj.GetComponent<T>();
    //}
}

/// <summary>
/// 스크립터블을 생성하고 초기에 여러개 생성 할 오브젝트를 등록해줄 것.
/// </summary>
[CreateAssetMenu(fileName = "ObjectPoolSetting", menuName = "Scriptable Object/ObjectPool", order = int.MaxValue)]
public class ObjectPoolData : ScriptableObject
{
    public List<PoolObject> poolObjects;

}