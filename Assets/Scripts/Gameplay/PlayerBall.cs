using System.Collections;
using Core.ServiceLocator;
using Gameplay.GameServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class PlayerBall : MonoBehaviour
{
 
    [Header("Size")]
    [SerializeField] private float _growRate = 0.25f;          // Player volume transferred to the shot per second.
    [SerializeField] private float _roadFactor = 2f;           // Road half-width = player radius * this value.
 
    
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _hopHeight = 0.5f;
    [SerializeField] private float _hopFrequency = 1.5f;       // Hops per second.
    [SerializeField] private float _shotSpeed = 25f;
 
    private enum State { Idle, Charging, Firing, Won, Lost }
    private State _state = State.Idle;
 
    private float _playerVol, _shotVol, _z, _hopT;
    private bool _moving;
    private Transform _shot;

    private GameProgressor _progressor;
    private LevelGenerator _level;
 
    private float Radius => LevelRules.VolToRadius(_playerVol);
 
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _progressor = Services.Get<GameProgressor>();
        _level = Services.Get<LevelGenerator>();
    }
 
    private void Update()
    {
        if (_state == State.Won || _state == State.Lost) return;
 
        if (_state == State.Idle)
        {
            if (Input.GetMouseButtonDown(0) && CanShoot()) BeginCharge();
            else Advance();
        }
        else if (_state == State.Charging)
        {
            Charge();
        }
        ApplyVisual();
    }

    public void StartGame()
    {
        _playerVol = LevelRules.RadiusToVol(_level.startRadius);
        _z = 0f;
        ApplyVisual();
    }
 
    private bool CanShoot()
    {
        float minVol = LevelRules.RadiusToVol(LevelRules.CriticalRadius);
        return _playerVol - LevelRules.RadiusToVol(LevelRules.MinShotRadius) > minVol;
    }
 
    // ---------- Forward movement ----------
    private void Advance()
    {
        int blocker = LevelRules.FindBlocker(_level.Data, _level.Dead, _z, Radius, out float stopZ);
        float targetZ = blocker >= 0 ? stopZ : _level.GoalZ - 0.5f;
 
        if (_level.GoalZ - _z <= _level.doorClearZone) _level.OpenDoor();
 
        if (_z < targetZ - 0.01f)
        {
            _z = Mathf.MoveTowards(_z, targetZ, _moveSpeed * Time.deltaTime);
            _moving = true;
            _hopT += Time.deltaTime;
        }
        else
        {
            _moving = false;
            if (blocker >= 0)
            {
                // A blocker remains and no usable shots are available.
                if (!CanShoot()) Finish(false);
            }
            else Finish(true);
        }
    }
 
    // ---------- Projectile charging ----------
    private void BeginCharge()
    {
        _state = State.Charging;
        _shotVol = 0f;
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().material.color = new Color(1f, 0.9f, 0.2f);
        _shot = go.transform;
    }
 
    private void Charge()
    {
        float dv = _growRate * Time.deltaTime;
        _playerVol -= dv;
        _shotVol += dv;
 
        // The player has reached the critical size threshold.
        bool lessResources = Radius <= LevelRules.CriticalRadius;
 
        float sr = LevelRules.VolToRadius(_shotVol);
        _shot.localScale = Vector3.one * sr * 2f;
        _shot.position = transform.position + Vector3.forward * (Radius + sr * 0.8f);
 
        if (Input.GetMouseButtonUp(0) || lessResources)
            StartCoroutine(FireRoutine(sr));
    }
 
    private IEnumerator FireRoutine(float shotR)
    {
        _state = State.Firing;
        _moving = false;
 
        Vector3 origin = new Vector3(0, 0, _z);
        int hit = LevelRules.FindFirstHit(_level.Data, _level.Dead, origin, shotR, out Vector3 hp);
        float targetZ = hit >= 0 ? hp.z : _level.GoalZ;
        Vector3 target = new Vector3(0, shotR, targetZ);
 
        while (_shot != null && Vector3.Distance(_shot.position, target) > 0.05f)
        {
            _shot.position = Vector3.MoveTowards(_shot.position, target, _shotSpeed * Time.deltaTime);
            yield return null;
        }
        if (_shot != null) Destroy(_shot.gameObject);
 
        if (hit >= 0) _level.InfectFrom(hit, shotR);
 
        yield return new WaitForSeconds(0.8f);   // Wait for chain explosions to finish.
        if (_state == State.Firing) _state = State.Idle;
    }
 
    // ---------- Visuals ----------
    public void ApplyVisual()
    {
        float r = Radius;
        float hop = _moving ? Mathf.Abs(Mathf.Sin(_hopT * _hopFrequency * Mathf.PI)) * _hopHeight : 0f;
        transform.localScale = Vector3.one * r * 2f;
        transform.position = new Vector3(0, r + hop, _z);
        _level.SetRoad(r * _roadFactor);
    }
 
    private void Finish(bool win)
    {
        _state = win ? State.Won : State.Lost;
        _progressor.GameFinished(win);
    }
}
}
