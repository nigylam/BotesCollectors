using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class Scanner : MonoBehaviour, IColorable
{
    [SerializeField] private float _scanDuration;
    [SerializeField] private float _scanFrequencyRate = 1f;
    [SerializeField] private float _deltaDuration;
    [SerializeField] private float _scanSize;

    private bool _isScanActive = false;
    private float _scanTimer;
    private float _elapsedTime = 0;
    private ParticleSystem _particleSystem;
    private ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;
    private Button _scanButton;

    public event Action<Resource> ResourceFound;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule particleSystemMain = _particleSystem.main;
        _sizeOverLifetime = _particleSystem.sizeOverLifetime;
        particleSystemMain.duration = _scanDuration;
        ParticleSystem.MinMaxCurve startSize = particleSystemMain.startSize;
        startSize.constantMax = _scanSize;
        ParticleSystem.MinMaxCurve lifetime = particleSystemMain.startLifetime;
        lifetime.curveMultiplier = _scanDuration + _deltaDuration;
        particleSystemMain.startLifetime = lifetime;
    }

    private void OnEnable()
    {
        if (_scanButton == null)
            return;

        _scanButton.onClick.AddListener(StartScan);
    }

    private void OnDisable()
    {
        _scanButton.onClick.RemoveListener(StartScan);
    }

    public void Initialize(Button scanButton)
    {
        _scanButton = scanButton;
        _scanButton.onClick.AddListener(StartScan);
    }

    public void Scan()
    {
        if (_isScanActive == false)
            return;

        _scanTimer += Time.deltaTime;
        _elapsedTime += Time.deltaTime;
        float normalized = Mathf.Clamp01(_scanTimer / _scanDuration);

        if (_elapsedTime >= _scanFrequencyRate)
        {
            float currentSize = 1f;

            if (_sizeOverLifetime.enabled)
                currentSize = _sizeOverLifetime.size.Evaluate(normalized);

            float visualSize = _scanSize * currentSize;
            float halfSize = 0.5f;
            float radius = visualSize * halfSize;
            _elapsedTime = 0;
            SelectResources(radius);
        }

        if (_scanTimer >= _scanDuration)
            _isScanActive = false;
    }

    public void ChangeColor(Color color)
    {
        ParticleSystem.MainModule main = _particleSystem.main;
        main.startColor = color;
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
        _scanTimer = 0f;
        _isScanActive = true;
        _particleSystem.Play();
    }
}
