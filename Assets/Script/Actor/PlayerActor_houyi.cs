using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Google.Protobuf;
using TrueSync;

public class GameState_Skill_1_houyi : ActorGameState
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
            mActor.TransState(ActorStateType.Idle);
        }
    }
}

public class PlayerActor_houyi : PlayerActor
{
    public static FixedPointF mRenderFrameRate = new FixedPointF(1000, 1000);

    protected override void InitStateMachine()
    {
        mStateMachineDic[ActorStateType.Idle] = new GameState_Idle_Normal();
        mStateMachineDic[ActorStateType.Move] = new GameState_Move_Normal();
        mStateMachineDic[ActorStateType.Skill_1] = new GameState_Skill_1_houyi();
    }

    protected override void InitWillUsedPrefabs()
    {
        WillUsedPrefabs = new GameObject[3];
        WillUsedPrefabs[0] = _AssetManager.GetGameObject("prefab/effect/bullet/houyibullet_prefab");
    }

    public override void Skill_1()  //闪现
    {
        TrueSyncManager.SyncedInstantiate(WillUsedPrefabs[0], AllTSTransform.position, RotateTSTransform.rotation);
        //for (int i = 0; i <= 50; i++)
        //{
        //    AllTSTransform.Translate(Angle * Speed);
        //}
        AllTSTransform.Translate(Angle * Speed * 50);
    }
}
