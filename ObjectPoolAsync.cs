using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Utils.ObjectPooling
{
  public abstract class ObjectPoolAsync<TObject> where TObject : MonoBehaviour, IPoolable<TObject>
  {
    private readonly List<TObject> _pooledObjects = new();
    private readonly Stack<TObject> _freeObjects = new();
    
    private readonly IAsyncObjectFactory<TObject> _objectFactory;
    private readonly Action<TObject> _onPullObject;
    private readonly Action<TObject> _onPushObject;

    protected ObjectPoolAsync(IAsyncObjectFactory<TObject> objectFactory) => 
      _objectFactory = objectFactory;

    protected ObjectPoolAsync(IAsyncObjectFactory<TObject> objectFactory, Action<TObject> onPullObject, Action<TObject> onPushObject)
    {
      _objectFactory = objectFactory;
      _onPullObject = onPullObject;
      _onPushObject = onPushObject;
    }

    public async UniTask<TObject> Pull() => 
      await PullInternal(Vector3.zero, Quaternion.identity, null);
    
    public async UniTask<TObject> Pull(Vector3 at, Quaternion rotation = default, Transform parent = null) => 
      await PullInternal(at, rotation, parent);

    private async UniTask<TObject> PullInternal(Vector3 at, Quaternion rotation, Transform parent)
    {
      TObject pulled;

      if (_freeObjects.Count > 0)
      {
        pulled = _freeObjects.Pop();
        Transform pulledTransform = pulled.transform;
        pulledTransform.position = at;
        pulledTransform.rotation = rotation;
        pulledTransform.SetParent(parent);
      }
      else
      {
        pulled = await _objectFactory.Create(at, rotation, parent);
        _pooledObjects.Add(pulled);
      }

      pulled.Initialize(returnAction: Push);
      _onPullObject?.Invoke(pulled);
    
      return pulled;
    }

    private void Push(TObject obj)
    {
      _freeObjects.Push(obj);
      _onPushObject?.Invoke(obj);
      obj.OnPush();
    }

    public void Cleanup()
    {
      foreach (TObject obj in _pooledObjects) 
        Object.Destroy(obj.gameObject);
    }
  }
}