using System.Collections.Generic;
using UnityEngine;

public class PoolMonoBehaviour<T> where T : MonoBehaviour
{
    private List<T> _pool;
    private Transform _container;
    private T _prefab;

    public PoolMonoBehaviour(T prefab, Transform container, int objectAmount)
    {
        _prefab = prefab;
        _container = container;

        PoolCreate(objectAmount);
    }

    public T GetObject(bool state = false)
    {
        if (HasFreeObject(out T freeObject, state)) { return freeObject; }
        else { return CreateObject(state); }
    }

    private bool HasFreeObject(out T freeObject, bool state)
    {
        freeObject = null;

        foreach(var poolObject in _pool)
        {
            if (!poolObject.gameObject.activeInHierarchy)
            {
                freeObject = poolObject;
                freeObject.gameObject.SetActive(state);

                return true;
            }
        }

        return false;
    }

    private void PoolCreate(int objectAmount)
    {
        _pool = new List<T>();

        for(int i = 0; i < objectAmount; i++)
        {
            CreateObject(); 
        }
    }

    private T CreateObject(bool defaultState = false)
    {
        var poolObject = Object.Instantiate(_prefab, _container.position, Quaternion.identity);

        poolObject.gameObject.SetActive(defaultState);
        _pool.Add(poolObject);

        return poolObject;
    }
}