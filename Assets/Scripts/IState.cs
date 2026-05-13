using UnityEngine;

public interface IState
{
    void Enter();
    void UpdateLogic();
    void UpdatePhysics();
    void Exit();
    
}