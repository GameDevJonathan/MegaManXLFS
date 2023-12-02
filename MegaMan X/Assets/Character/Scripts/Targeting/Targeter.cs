using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [SerializeField] private List<Target> targets = new List<Target>();
    [SerializeField] public Target CurrentTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       if(!other.TryGetComponent<Target>(out Target target)) { return; }
       targets.Add(target);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Remove(target);
    }

    public bool SelectTarget()
    {
        if(targets.Count == 0) { return false; }
        CurrentTarget = targets[0];
        return true;
    }

    public void Cancel()
    {
        CurrentTarget = null;
    }
}
