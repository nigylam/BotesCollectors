using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class Scanner : MonoBehaviour
{
    [SerializeField] private float _scanSize;
    [SerializeField] private float _scanDuration;
    [SerializeField] private float _sphereDrawingRate = 1f;
    [SerializeField] private Button _scanButton;

    private float _effectDuration;
    private float _deltaDuration = 1f;

    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _particleSystemMain;
    private ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;

    private float _elapsedTime = 0;

    public event Action<Resource> ResourceFound;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemMain = _particleSystem.main;
        _sizeOverLifetime = _particleSystem.sizeOverLifetime;

        _effectDuration = _scanDuration + _deltaDuration;
        _particleSystemMain.duration = _effectDuration;

        ParticleSystem.MinMaxCurve startSize = _particleSystemMain.startSize;
        startSize.constantMax = _scanSize;
    }

    private void OnEnable()
    {
        _scanButton.onClick.AddListener(StartScan);
    }

    private void OnDisable()
    {
        _scanButton.onClick.RemoveListener(StartScan);
    }

    public void Scan()
    {
        if (_particleSystem.IsAlive() == false || _particleSystem.time > _scanDuration)
            return;

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _sphereDrawingRate)
        {
            float elapsed = _particleSystem.time;
            float normalized = Mathf.Clamp01(elapsed / _scanDuration);
            float currentSize = 1f;

            if (_sizeOverLifetime.enabled)
                currentSize = _sizeOverLifetime.size.Evaluate(normalized);

            float visualSize = _scanSize * currentSize;
            float halfSize = 0.5f;
            float radius = visualSize * halfSize;
            _elapsedTime = 0;
            SelectResources(radius);
        }
    }

    private void SelectResources(float radius)
    {
        foreach (var collider in Physics.OverlapSphere(transform.position, radius))
        {
            if (collider.TryGetComponent(out Resource resource))
                ResourceFound?.Invoke(resource);
        }
    }

    private void StartScan()
    {
        _particleSystem.Play();
    }
}
