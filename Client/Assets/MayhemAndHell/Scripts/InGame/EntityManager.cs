using System.Collections.Generic;
using MayhemCommon.MessageObjects.Views;
using UnityEngine;
using System.Collections;

public class EntityManager : MonoBehaviour
{
    private Dictionary<int, WorldObject> _entities;
    private Dictionary<string, Stack<WorldObject>> _freeObjects;

	void Start () {
        _entities = new Dictionary<int, WorldObject>();
        _freeObjects = new Dictionary<string, Stack<WorldObject>>();
	}

    public WorldObject InstantiateObject(ObjectView view)
    {
        var newObject = TryGetPrefab(view.Prefab);

        if (newObject == null)
        {
            var prefab = Resources.Load("Prefabs/" + view.Prefab);
            var obj = (GameObject)Object.Instantiate(prefab, new Vector3(), Quaternion.identity);
            newObject = obj.GetComponent<WorldObject>();
        }
        
        newObject.InitializeFromView(view);
        AddObject(newObject, view.Prefab);
        newObject.transform.parent = transform;

        Debug.Log(view.InstanceId);

        _entities.Add(view.InstanceId, newObject);

        return newObject;
    }

    public WorldObject TryGetPrefab(string prefab)
    {
        if (_freeObjects.ContainsKey(prefab))
        {
            var stack = _freeObjects[prefab];
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
        }
        return null;
    }

    public void DestroyObject(WorldObject obj)
    {
        _entities.Remove(obj.InstanceId);

        Stack<WorldObject> stack;
        if (_freeObjects.ContainsKey(obj.Prefab))
        {
            stack = _freeObjects[obj.Prefab];
        }
        else
        {
            stack = new Stack<WorldObject>();
        }

        stack.Push(obj);
    }

    public WorldObject GetObject(int id)
    {
        WorldObject temp;
        _entities.TryGetValue(id, out temp);
        return temp;
    }

    public void AddObject(WorldObject obj, string prefab)
    {
        if (!_entities.ContainsKey(obj.InstanceId))
        {
            _entities.Add(obj.InstanceId, obj);
        }
    }
}
