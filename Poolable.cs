using System;
using UnityEngine;

namespace CodeBase.Utils.ObjectPooling
{
  public class Poolable : MonoBehaviour, IPoolable<Poolable>
  {
    private Action<Poolable> _returnAction;

    public void Initialize(Action<Poolable> returnAction) => 
      _returnAction = returnAction;

    public void ReturnToPool() => 
      _returnAction?.Invoke(this);

    public virtual void OnPull()
    {
    }

    public virtual void OnPush()
    {
    }
  }
}