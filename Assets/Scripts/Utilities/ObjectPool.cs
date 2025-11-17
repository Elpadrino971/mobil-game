using UnityEngine;
using System.Collections.Generic;

namespace PrisonIsland.Utilities
{
    /// <summary>
    /// Generic object pooling system for performance optimization
    /// Use this for frequently instantiated/destroyed objects
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

        [Header("Pool Settings")]
        [SerializeField] private Transform poolParent;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (poolParent == null)
            {
                poolParent = transform;
            }
        }

        /// <summary>
        /// Create a new pool for a specific prefab
        /// </summary>
        public void CreatePool(string poolName, GameObject prefab, int initialSize = 10)
        {
            if (poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool '{poolName}' already exists!");
                return;
            }

            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab, poolParent);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary.Add(poolName, objectQueue);
            prefabDictionary.Add(poolName, prefab);
        }

        /// <summary>
        /// Spawn an object from the pool
        /// </summary>
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolName))
            {
                Debug.LogError($"Pool '{poolName}' doesn't exist! Create it first with CreatePool()");
                return null;
            }

            GameObject obj;

            // If pool is empty, create new instance
            if (poolDictionary[poolName].Count == 0)
            {
                obj = Instantiate(prefabDictionary[poolName], poolParent);
            }
            else
            {
                obj = poolDictionary[poolName].Dequeue();
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            return obj;
        }

        /// <summary>
        /// Spawn an object from the pool (overload)
        /// </summary>
        public GameObject Spawn(string poolName, Vector3 position)
        {
            return Spawn(poolName, position, Quaternion.identity);
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        public void Despawn(string poolName, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool '{poolName}' doesn't exist! Destroying object instead.");
                Destroy(obj);
                return;
            }

            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            poolDictionary[poolName].Enqueue(obj);
        }

        /// <summary>
        /// Despawn after delay
        /// </summary>
        public void DespawnAfterDelay(string poolName, GameObject obj, float delay)
        {
            StartCoroutine(DespawnCoroutine(poolName, obj, delay));
        }

        private System.Collections.IEnumerator DespawnCoroutine(string poolName, GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(poolName, obj);
        }

        /// <summary>
        /// Clear a specific pool
        /// </summary>
        public void ClearPool(string poolName)
        {
            if (!poolDictionary.ContainsKey(poolName))
                return;

            Queue<GameObject> queue = poolDictionary[poolName];
            while (queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();
                Destroy(obj);
            }

            poolDictionary.Remove(poolName);
            prefabDictionary.Remove(poolName);
        }

        /// <summary>
        /// Clear all pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in poolDictionary.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    Destroy(obj);
                }
            }

            poolDictionary.Clear();
            prefabDictionary.Clear();
        }
    }
}
