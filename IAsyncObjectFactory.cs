using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Utils.ObjectPooling
{
  public interface IAsyncObjectFactory<TObject>
  {
    UniTask<TObject> Create(Vector3 at, Quaternion rotation, Transform parent = null);
  }
}