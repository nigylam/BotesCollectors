using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private float _scanSize;
    [SerializeField] private float _scanDuration;
    [SerializeField] private float _sphereDrawingRate = 1f;

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _particleSystemMain;
    private ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;

    private float _elapsedTime = 0;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemMain = _particleSystem.main;
        _sizeOverLifetime = _particleSystem.sizeOverLifetime;

        _particleSystemMain.startLifetime = _scanDuration;
        _particleSystemMain.startSize = _scanSize;
    }

    public void Scan()
    {
        if (_particleSystem.IsAlive() == false)
            return;

        float elapsed = _particleSystem.time;
        float normalized = Mathf.Clamp01(elapsed / _scanDuration);
        float currentSize = 1f;

        if (_sizeOverLifetime.enabled)
            currentSize = _sizeOverLifetime.size.Evaluate(normalized);

        float visualSize = _scanSize * currentSize;
        float halfSize = 0.5f;
        float radius = visualSize * halfSize;

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _sphereDrawingRate)
        {
            _elapsedTime = 0;
            DrawSphere(radius);
        }
    }

    private void DrawSphere(float radius)
    {
        foreach (var collider in Physics.OverlapSphere(transform.position, radius))
        {
            if (collider.TryGetComponent(out Resource resource))
                resource.Select();
        }
    }
}
