using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBob : MonoBehaviour
{
    private Transform Transform;

    [Tooltip("If anchor is self, it doesnt get anchored")]
    [SerializeField] private Transform Anchor;

    private bool _isAnchored;
    private Vector3 _anchorOffset;
    public bool LockXMovement, LockYMovement, LockZMovement;
    public float bobSpeed = 1.5f;
    public float bobHeight = 1f;
    private Vector3 _startPos;
    private Vector3 _MovemenEquations;
    private Vector3 _progression;
    private readonly Vector3 _speedIncrement = Vector3.one;

    public void Start()
    {
        _MovemenEquations = new Vector3(Mathf.Floor(Random.Range(0, 3)), Mathf.Floor(Random.Range(0, 3)), Mathf.Floor(Random.Range(0, 3)));
        Transform = GetComponent<Transform>();
        if (Anchor != null)
            _isAnchored = true;
        else
            _isAnchored = false;

        _startPos = transform.position;
        if (_isAnchored)
        {
            Transform.SetParent(Anchor);
            _startPos = transform.localPosition;
            _anchorOffset = Anchor.localPosition - _startPos;
        }
    }

    public void Update()
    {
        _progression += Time.deltaTime * _speedIncrement;
    }

    public void FixedUpdate()
    {
        if (_isAnchored)
            AnchoredBob();
        else
            bob();
    }

    public void AnchoredBob()
    {
        Vector3 TmpPos = Anchor.localPosition - _anchorOffset;
        float newY = TmpPos.y + TrigMotionEquations((int)_MovemenEquations.x, _progression.x, bobSpeed, bobHeight);
        float newX = TmpPos.x + TrigMotionEquations((int)_MovemenEquations.y, _progression.y, bobSpeed, bobHeight);
        float newZ = TmpPos.z + TrigMotionEquations((int)_MovemenEquations.z, _progression.z, bobSpeed, bobHeight);

        if (LockXMovement)
            newX = TmpPos.x;
        if (LockYMovement)
            newY = TmpPos.y;
        if (LockZMovement)
            newZ = TmpPos.z;
        Transform.localPosition = new Vector3(newX, newY, newZ);
    }

    public void bob()
    {
        float newY = _startPos.y + TrigMotionEquations((int)_MovemenEquations.x, _progression.x, bobSpeed, bobHeight);
        float newX = _startPos.x + TrigMotionEquations((int)_MovemenEquations.y, _progression.y, bobSpeed, bobHeight);
        float newZ = _startPos.z + TrigMotionEquations((int)_MovemenEquations.z, _progression.z, bobSpeed, bobHeight);

        if (LockXMovement)
            newX = _startPos.x;
        if (LockYMovement)
            newY = _startPos.y;
        if (LockZMovement)
            newZ = _startPos.z;
        Transform.position = new Vector3(newX, newY, newZ);
    }

    [Tooltip("Equations index starts at 0")]
    private float TrigMotionEquations(int equation, float progression, float frequency, float amplitude)
    {
        float result = 0f;
        switch (equation)
        {
            case 0:
                result = Mathf.Sin(1.8f * Mathf.Sin(Mathf.Cos(progression * 0.13f)) * Mathf.Cos((progression - 3f) * frequency)) * amplitude;
                break;

            case 1:
                result = Mathf.Sin((Mathf.Sin(progression * 0.8f) * 0.5f) * (Mathf.Cos(progression * 0.2f) * frequency)) * amplitude;
                break;

            case 2:
                result = (Mathf.Sin(progression * frequency)) * (Mathf.Sin((progression * 0.4f) * 0.3f)) * amplitude;
                break;
        }
        return result;
    }

    public void SetAnchor(Transform anchor)
    {
        Anchor = anchor;
        _isAnchored = true;
        _anchorOffset = Anchor.localPosition - _startPos;
    }
}