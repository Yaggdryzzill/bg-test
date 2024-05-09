
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pentagram : Interactable
{
    [SerializeField] private Soul soul;
    [SerializeField] private float soulMaxXOffset = 9f;
    [SerializeField] private float soulMinXOffset = -9f;
    [SerializeField] private float soulYOffset = 9f;
    [SerializeField] private Transform centerPoint;


    protected override void Awake()
    {
        base.Awake();
        SetListeners();
    }

    private void SetListeners()
    {
        if(soul)
            soul.onTargetReached.AddListener(SoulReachedCenter);
    }

    private void StartSoul()
    {
        float xOffset = Random.Range(soulMinXOffset, soulMaxXOffset);
        var position = centerPoint.position;
        soul.transform.position = new Vector3(position.x + xOffset, position.y + soulYOffset, 0f);
        soul.MoveToTarget(centerPoint);
    }

    private void SoulReachedCenter()
    {
        PlayerManager.Instance.AddSouls(1);
        if(_isInteracting)
            StartSoul();
    }

    public override void Interact(PlayerInteraction playerInteraction)
    {
        if(!_isInteractable) return;

        if (!_isInteracting)
        {
            _isInteracting = true;
            _playerInteraction = playerInteraction;
            BeginInteraction();
        }
        else
        {
            EndInteraction();
            _isInteracting = false;
        }
    }

    public override void ClosestInteractable(bool value)
    {
        if(!_isInteractable || _isInteracting) return;

        _interactionCanvas.SetActive(value);
    }

    protected override void EndInteraction()
    {
        _playerInteraction.ReleaseInteraction();
        onEndInteraction?.Invoke();
    }

    protected override void BeginInteraction()
    {
        _playerInteraction.LockInteraction(this);
        _interactionCanvas.SetActive(false);
        
        if(!soul.IsMoving)
            StartSoul();
        
        onBeginInteraction?.Invoke();
    }
}