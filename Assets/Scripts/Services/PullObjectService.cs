using System.Collections.Generic;
using UnityEngine;

public class PullObjectService : MonoBehaviour
{
    [SerializeField]
    private List<Object> _pullObjects = new();

    public T SpawnObject<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
    {
        foreach (var pulledObject in _pullObjects)
        {
            if (pulledObject.GetType() == prefab.GetType() && (pulledObject as MonoBehaviour).gameObject.activeInHierarchy == false)
            {
                (pulledObject as MonoBehaviour).transform.SetPositionAndRotation(position, rotation);
                (pulledObject as MonoBehaviour).
                    gameObject.SetActive(true);
                return pulledObject as T;
            }
        }
        var spawnedObject = Instantiate(prefab, position, rotation);
        _pullObjects.Add(spawnedObject);
        return spawnedObject;
    }
}
