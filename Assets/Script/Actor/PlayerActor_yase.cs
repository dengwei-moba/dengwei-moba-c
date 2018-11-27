using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Google.Protobuf;

public class GameState_Skill_1_yanse : ActorGameState
{
    private Actor mActor;

    public override ActorStateType StateType
    {
        get
        {
            return ActorStateType.Skill_1;
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
                animation.Play("skill2");
            }
        }
    }

    public override void Exit()
    {
        mActor = null;
    }

    public override void OnUpdate()
    {
        if (mActor != null && mActor.ActorObj != null)
        {
            mActor.Skill_1();
        }
    }
}

public class PlayerActor_yase : PlayerActor
{
    protected override void InitStateMachine()
    {
        mStateMachineDic[ActorStateType.Idle] = new GameState_Idle_Normal();
        mStateMachineDic[ActorStateType.Move] = new GameState_Move_Normal();
        mStateMachineDic[ActorStateType.Skill_1] = new GameState_Skill_1_yanse();
    }

    public override void Skill_1()  //超速移动
    {
        AllTSTransform.Translate(Angle * Speed * 2);
    }
}
