using System;

namespace CodeBase.Utils.ObjectPooling
{
  public interface IPoolable<TObject>
  {
    void Initialize(Action<TObject> returnAction);
    void ReturnToPool();
    void OnPull();
    void OnPush();
  }
}