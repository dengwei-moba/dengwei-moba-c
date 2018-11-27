using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActorStateType
{
    Idle,
    Move,
    Skill_1,
    Skill_2,
    Skill_3,
    Skill_4,
}

public abstract class ActorGameState
{
    public abstract ActorStateType StateType { get; }
    public abstract void TryTransState(ActorStateType tStateType);
    public abstract void Enter(params object[] param);
    public abstract void OnUpdate();
    public abstract void Exit();
}
//====================================================================================================
//只是提供一些基础的通用的ActorGameState
public class GameState_Idle_Normal : ActorGameState
{
    private Actor mActor;

    public override ActorStateType StateType
    {
        get
        {
            return ActorStateType.Idle;
        }
    }

    public override void TryTransState(ActorStateType tStateType)
    {

    }

    public override void Enter(params object[] param)
    {
        mActor = param[0] as Actor;
        if (mActor != null && mActor.ActorObj != null)
        {
            Animation animation = mActor.ActorObj.GetComponent<Animation>();
            if (animation != null)
            {
                animation.wrapMode = WrapMode.Loop;
                animation.Play("idle");
            }
        }
    }

    public override void Exit()
    {
        mActor = null;
    }

    public override void OnUpdate()
    {

    }
}

public class GameState_Move_Normal : ActorGameState
{
    private Actor mActor;

    public override ActorStateType StateType
    {
        get
        {
            return ActorStateType.Move;
        }
    }

    public override void TryTransState(ActorStateType tStateType)
    {

    }

    public override void Enter(params object[] param)
    {
        mActor = param[0] as Actor;
        if (mActor != null && mActor.ActorObj != null)
        {
            Animation animation = mActor.ActorObj.GetComponent<Animation>();
            if (animation != null)
            {
                animation.wrapMode = WrapMode.Loop;
                animation.Play("run");
            }
            mActor.IsMove = true;
        }
    }

    public override void Exit()
    {
        mActor.IsMove = false;
        mActor = null;
    }

    public override void OnUpdate()
    {
        if (mActor != null && mActor.ActorObj != null)
        {
            mActor.Move();
        }
    }
}