using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TrueSync;

//用来配置buff属性(还没有做读取导表,面板上直接配置)
public class BuffBaseManager : MonoBehaviour {

    private static BuffBaseManager _instance;
    public static BuffBaseManager Instance
    {
        get { return _instance; }
    }

    public List<BuffBase> mBuffBaseList = new List<BuffBase>();

    void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// 执行buff
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="buffID"></param>
    public void DoBuff(Actor actor,int buffID)
    {
        DoBuff(actor,GetBuffBase(buffID));
    }

    /// <summary>
    /// 执行buff
    /// </summary>
    /// <param name="mActor"></param>
    /// <param name="buffBase"></param>
    public void DoBuff(Actor mActor, BuffBase buffBase)
    {
        if (buffBase == null) return;
        ActorBuff db = null;

        switch (buffBase.buffEffectType)
        {
            case BuffEffectType.AddHp: //增加血量
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        mActor.mActorAttr.Hp += buffBase.Num;
                    });

                }
                break;
            case BuffEffectType.AddMaxHp: //增加最大血量
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        mActor.mActorAttr.HpMax += buffBase.Num;
                    }, delegate {
                        mActor.mActorAttr.HpMax += buffBase.Num;
                    }, delegate {
                        mActor.mActorAttr.HpMax -= buffBase.Num;
                    });
                }
                break;
            case BuffEffectType.SubHp: //减少血量
                if (!IsAdd(mActor, buffBase))
                { 
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        mActor.mActorAttr.Hp -= buffBase.Num;
                    });
                }
                break;
            case BuffEffectType.SubMaxHp: //减少最大血量
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        mActor.mActorAttr.HpMax -= buffBase.Num;
                    }, delegate
                    {
                        mActor.mActorAttr.HpMax -= buffBase.Num;
                    }, delegate
                    {
                        mActor.mActorAttr.HpMax += buffBase.Num;
                    });
                }
                break;
            case BuffEffectType.AddSpeed:
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, null);
                    db.OnStart = delegate
                    {
                        mActor.Speed += buffBase.Num;
                    };
                    db.OnFinsh = delegate
                    {
                        mActor.Speed -= buffBase.Num;
                    };
                }
                break;
            case BuffEffectType.AddDamageFloated: //浮空
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        //if (actor.ActorState != ActorState.DamageRise)
                        //    actor.ActorAttr.DamageRiseAbility = buff.Num;
                        //actor.SetDamageRiseState();
                    });
                }
                break;
            case BuffEffectType.AddFloated:
                if (!IsAdd(mActor, buffBase))
                {
                    db = ActorBuff.Create(buffBase, delegate
                    {
                        //Vector3 moveDir = Vector3.up;
                        //moveDir *= buff.Num;
                        //actor.CharacterController.Move(moveDir*Time.deltaTime);
                    });
                }
                 break;
        }
        if (db != null)
            mActor.mActorBuffManager.AddBuff(db);
    }

    /// <summary>
    /// 玩家是否已经有此buff
    /// </summary>
    /// <param name="mActor"></param>
    /// <param name="buffBase"></param>
    /// <returns></returns>
    private bool IsAdd(Actor mActor,BuffBase buffBase)
    {
        ActorBuff oldBuff = mActor.mActorBuffManager.GetBuffByBaseID(buffBase.BuffID);
        if (oldBuff != null)
        {
            switch (buffBase.buffOverlapType)
            {
                case BuffOverlapType.ResetTime:
                    oldBuff.ResetTime();
                    break;
                case BuffOverlapType.AddLayer:
                    oldBuff.AddLayer();
                    break;
                case BuffOverlapType.AddTime:
                    oldBuff.ChangeTotalFrame(oldBuff.GetTotalFrame + buffBase.TotalFrame);
                    break;
                default:
                    break;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取配置数据
    /// </summary>
    /// <param name="buffID"></param>
    /// <returns></returns>
    public BuffBase GetBuffBase(int buffID)
    {
        for (int i = 0; i < mBuffBaseList.Count; i++)
        {
            if (mBuffBaseList[i].BuffID == buffID)
                return mBuffBaseList[i];
        }
        return null;
    }
}

/// <summary>
/// buff效果类型
/// </summary>
public enum BuffEffectType
{
    /// <summary>
    /// 恢复HP
    /// </summary>
    AddHp,
    /// <summary>
    /// 增加最大血量
    /// </summary>
    AddMaxHp,
    /// <summary>
    /// 减血
    /// </summary>
    SubHp,
    /// <summary>
    /// 减最大生命值
    /// </summary>
    SubMaxHp,
    /// <summary>
    /// 加速
    /// </summary>
    AddSpeed,

    /// <summary>
    /// 眩晕
    /// </summary>
    AddVertigo,
    /// <summary>
    /// 被击浮空
    /// </summary>
    AddFloated,
    /// <summary>
    /// 被击浮空
    /// </summary>
    AddDamageFloated,
}

/// <summary>
/// 叠加类型
/// </summary>
public enum BuffOverlapType
{
    None,
    /// <summary>
    /// 增加时间
    /// </summary>
    AddTime,
    /// <summary>
    /// 堆叠层数
    /// </summary>
    AddLayer,
    /// <summary>
    /// 重置时间
    /// </summary>
    ResetTime,
}

/// <summary>
/// 关闭类型
/// </summary>
public enum BuffShutDownType
{
    /// <summary>
    /// 关闭所有
    /// </summary>
    All,
    /// <summary>
    /// 单层关闭
    /// </summary>
    Layer,
}

/// <summary>
/// 执行时重复类型
/// </summary>
public enum BuffRepeatType
{
    /// <summary>
    /// 一次
    /// </summary>
    Once,
    /// <summary>
    /// 每次
    /// </summary>
    Loop,
}

[System.Serializable]
public class BuffBase       //配置类，这个类用来实现加载buff的配置表信息
{
    /// <summary>
    /// BuffID
    /// </summary>
    public int BuffID;
    /// <summary>
    /// Buff效果类型
    /// </summary>
    public BuffEffectType buffEffectType;
    /// <summary>
    /// 执行时重复类型
    /// </summary>
    public BuffRepeatType buffRepeatType = BuffRepeatType.Loop;
    /// <summary>
    /// 叠加类型
    /// </summary>
    public BuffOverlapType buffOverlapType = BuffOverlapType.AddLayer;
    /// <summary>
    /// 消除类型
    /// </summary>
    public BuffShutDownType buffShutDownType = BuffShutDownType.All;
    /// <summary>
    /// 如果是堆叠层数，表示最大层数，如果是时间，表示最大时间
    /// </summary>
    public int MaxLimit = 0;
    /// <summary>
    /// 总共持续时间(单位:帧)
    /// </summary>
    public int TotalFrame = 0;
    /// <summary>
    /// 调用间隔时间(单位:帧)
    /// </summary>
    public int CallIntervalFrame = 1;
    /// <summary>
    /// 执行数值 比如加血就是每次加多少
    /// </summary>
    public FP Num;
}
