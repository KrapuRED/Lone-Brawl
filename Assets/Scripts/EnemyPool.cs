using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class EnemyPool : MonoBehaviour
{
    // singleton
   public static EnemyPool Instance;

   [Header("Pool Settings")]
   public GameObject enemyPrefab;
   public int poolSize = 50;

   private Queue<GameObject> poolQueue = new Queue<GameObject>();

    // singleton setup
   private void Awake()
   {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
   }

    private void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            // set as a child
            obj.transform.SetParent(this.transform);
            // add child to the pool
            poolQueue.Enqueue(obj);

        }


    }

    public GameObject GetObject()
    {
        if(poolQueue.Count > 0)
        {
            GameObject obj = poolQueue.Dequeue();
            return obj;
        }
        else
        {
            Debug.LogWarning("Exceed standart pool size");

            GameObject extraObj = Instantiate(enemyPrefab);
            extraObj.transform.SetParent(this.transform);

            return extraObj;
        }
    }

    public void ReturnObject(GameObject enemy)
    {
        enemy.SetActive(false);
        // put back for later
        poolQueue.Enqueue(enemy);
    }

    

}
