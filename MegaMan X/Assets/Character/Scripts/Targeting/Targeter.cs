using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private List<Target> targets = new List<Target>();
    [SerializeField] public Target CurrentTarget;
    [SerializeField] public int index = 0;
    [SerializeField] private bool didCycle;
    [SerializeField] private PlayerStateMachine stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"Target List Length: {targets.Count}");

        //if (CurrentTarget)
        //    Debug.Log($"CurrentTargetName: {CurrentTarget.transform.root.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Add(target);
        target.OnDestroyed += RemoveTarget;
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Remove(target);

        RemoveTarget(target);
    }

    private void RemoveTarget(Target target)
    {
        if (CurrentTarget == target)
        {
            targetGroup.RemoveMember(CurrentTarget.transform);
            CurrentTarget = null;
        }

        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }

    public bool SelectTarget()
    {


        if (targets.Count == 0) { return false; }

        Target closestTarget = null;
        float closestTargetDistance = Mathf.Infinity;

        foreach (Target target in targets)
        {
            Vector2 viewPos = mainCamera.WorldToViewportPoint(target.transform.position);

            if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
            {
                continue;
            }

            Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);

            if (toCenter.sqrMagnitude < closestTargetDistance)
            {
                closestTarget = target;
                closestTargetDistance = toCenter.sqrMagnitude;
            }
        }
        if (closestTarget == null) { return false; }

        CurrentTarget = closestTarget;

        if (CurrentTarget)
        {
            switch (CurrentTarget.type)
            {
                case Target.Type.small:
                    targetGroup.AddMember(CurrentTarget.transform, 1f, 2f);
                    break;
                case Target.Type.large:
                    targetGroup.AddMember(CurrentTarget.transform, .25f, 2f);
                    break;
            }

        }


        return true;
    }

    public void CycleTarget()
    {
        if (targets.Count == 0) { return; }
        if (CurrentTarget == null) { return; }

        //Debug.Log("Target Count: " + targets.Count);
        //Debug.Log("Current Target: " + CurrentTarget.name);

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].transform.position == CurrentTarget.transform.position)
            {
                index = i;
            }
        }


        if (stateMachine.InputReader.SelectionValue.x >= .9f)
        {

            if (index == 0 && didCycle == false)
            {
                Debug.Log("Running this block index >= targets.count");
                didCycle = true;
                index = targets.Count - 1;
                CurrentTarget = targets[index];

            }

            if (index <= targets.Count && didCycle == false)
            {
                didCycle = true;
                index -= 1;
                CurrentTarget = targets[index];
            }


        }

        if (stateMachine.InputReader.SelectionValue.x <= -.9f)
        {
            if (index >= targets.Count - 1 && didCycle == false)
            {
                Debug.Log($"Index: {index}");
                Debug.Log($"Target.Count: {targets.Count}");
                Debug.Log("Running this block index >= targets.count");
                didCycle = true;
                index = 0;
                CurrentTarget = targets[index];

            }

            if (index < targets.Count && didCycle == false)
            {
                didCycle = true;
                index += 1;
                CurrentTarget = targets[index];
            }

        }

        if (stateMachine.InputReader.SelectionValue.x == 0 && didCycle)
            didCycle = false;

    }

    public void Cancel()
    {
        if (CurrentTarget == null) { return; }
        targetGroup.RemoveMember(CurrentTarget.transform);
        CurrentTarget = null;
    }
}
