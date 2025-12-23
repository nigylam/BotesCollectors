using UnityEngine;
using UnityEngine.Pool;

public class FlagSpawner : MonoBehaviour
{
    [SerializeField] private Flag _flagPrefab;

    private ObjectPool<Flag> _pool;
    private int _poolCapacity = 5;
    private int _poolMaxSize = 10;

    private void Awake()
    {
        _pool = new ObjectPool<Flag>
            (
            createFunc: () => Instantiate(_flagPrefab),
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
            );
    }

    public Flag Spawn(Vector3 position, Transform parrent)
    {
        Vector3 spawnPosition = new Vector3(position.x, _flagPrefab.transform.position.y, position.z);
        Flag flag = _pool.Get();
        flag.transform.position = spawnPosition;
        flag.transform.parent = parrent;

        return flag;
    }

    public void Release(Flag flag) => _pool.Release(flag);
}
