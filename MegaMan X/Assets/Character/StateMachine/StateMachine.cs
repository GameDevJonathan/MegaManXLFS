using Cinemachine;
using System.Collections;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State currentState;
    private void Update()
    {
        Debug.Log(currentState);
        currentState?.Tick(Time.deltaTime);
    }

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }


}
