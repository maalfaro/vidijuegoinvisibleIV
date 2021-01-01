using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : PoolObject
{

    #region Variables

    private T prefab;
    private Transform poolParent;
    private List<T> pool = new List<T>();

    public int Count => pool.Count;

    #endregion

    #region Constructors

    public Pool(T prefab, Transform parent, int poolSize)
    {
        this.prefab = prefab;
        poolParent = parent;
        InitPool(poolSize);
    }

    #endregion

    #region Public methods

    public T GetPoolObject()
    {
        T poolObject = pool.Find(x => !x.InUse);

        if (poolObject == null)
        {
            return null;
        }

        poolObject.InUse = true;
        poolObject.GetPoolObject();
        return poolObject;
    }

    public void ReleasePoolObject(T poolObject)
    {
        poolObject.InUse = false;
        poolObject.ReleasePoolObject();
    }

    public void ClearPool()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            ReleasePoolObject(pool[i]);
        }
    }

    public T GetPoolObject(int index)
    {
        T poolObject = pool[index];

        if (poolObject == null)
        {
            return null;
        }

        poolObject.InUse = true;
        poolObject.GetPoolObject();
        return poolObject;
    }

    #endregion

    #region Private methods

    private void InitPool(int poolSize)
    {
        T newPoolObject;
        for (int i = 0; i < poolSize; i++)
        {
            newPoolObject = GameObject.Instantiate(prefab, poolParent).GetComponent<T>();
            newPoolObject.ReleasePoolObject();
            pool.Add(newPoolObject);
        }
    }

    #endregion

}

public abstract class PoolObject : MonoBehaviour
{

    #region Variables

    public bool InUse { get; set; }

    #endregion

    #region Abstract methods

    public abstract void GetPoolObject();
    public abstract void ReleasePoolObject();

    #endregion
}