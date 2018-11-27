using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Google.Protobuf;
using TrueSync;

public class PlayerActor : Actor
{
    ETCJoystick joy;
    ETCButton button_1;
    ETCButton button_2;
    ETCButton button_3;
    ETCButton button_4;
    public GameObject[] WillUsedPrefabs; 

    protected override void InitStateMachine()
    {
        mStateMachineDic[ActorStateType.Idle] = new GameState_Idle_Normal();
        mStateMachineDic[ActorStateType.Move] = new GameState_Move_Normal();
    }

    protected override void InitCurState()
    {
        CurState = mStateMachineDic[ActorStateType.Idle];
        CurState.Enter(this);
    }

    protected override void InitStateTransLimit()
    {

    }

    protected override void InitWillUsedPrefabs()
    {

    }
    //==========================处理网络数据开始=============================================================
    public override void PlayerInputHandle(PB_ClientInput input)
    {
        InputType iInputType = input.InputType;
        switch (iInputType)
        {
            case InputType.MoveStart:
                PlayerInputHandle_MoveStart();
                break;
            case InputType.MoveAngle:
                PlayerInputHandle_MoveAngle(input.AngleX, input.AngleY);
                break;
            case InputType.MoveEnd:
                PlayerInputHandle_MoveEnd();
                break;
            case InputType.KeyUp:
                PlayerInputHandle_KeyUp(input.Key);
                break;
            case InputType.KeyDown:
                PlayerInputHandle_KeyDown(input.Key);
                break;
            case InputType.KeyAngle:
                PlayerInputHandle_KeyAngle(input.Key);
                break;
            case InputType.ClickXy:
                PlayerInputHandle_ClickXY(input.PosX, input.PosY);
                break;
        }
    }

    public override void PlayerInputHandle_MoveStart()
    {
        this.TransState(ActorStateType.Move);
    }

    public override void PlayerInputHandle_MoveAngle(int inputAngleX, int inputAngleY)
    {
        TSVector mTSVector = new TSVector((FP)inputAngleX / 1000, FP.Zero, (FP)inputAngleY / 1000);
        //TSVector mTSVector2 = new TSVector(-(FP)inputAngleY / 1000, FP.Zero, (FP)inputAngleX / 1000);//(x,y)的法向量就是(-y,x)
        this.Angle = mTSVector;
        //Debug.LogErrorFormat("PlayerInputHandle_MoveAngle======>mTSVector={0}", mTSVector.ToString());
        //RotateTSTransform.LookAt(this.Angle);
        //RotateTSTransform.Rotate(this.Angle);
        RotateTSTransform.rotation = TSQuaternion.LookRotation(mTSVector);
    }

    public override void PlayerInputHandle_MoveEnd()
    {
        this.TransState(ActorStateType.Idle);
    }

    public override void PlayerInputHandle_KeyUp(int inputKey)
    {
        switch (inputKey)
        {
            case 1:
                this.TransState(ActorStateType.Idle);
                break;
            case 2:
                this.TransState(ActorStateType.Idle);
                break;
            case 3:
                this.TransState(ActorStateType.Idle);
                break;
            case 4:
                this.TransState(ActorStateType.Idle);
                break;
        }
    }

    public override void PlayerInputHandle_KeyDown(int inputKey)
    {
        switch (inputKey)
        {
            case 1:
                this.TransState(ActorStateType.Skill_1);
                break;
            case 2:
                this.TransState(ActorStateType.Skill_2);
                break;
            case 3:
                this.TransState(ActorStateType.Skill_3);
                break;
            case 4:
                this.TransState(ActorStateType.Skill_4);
                break;
        }
    }

    public override void PlayerInputHandle_KeyAngle(int inputKey)
    {
        
    }

    public override void PlayerInputHandle_ClickXY(int inputPosX, int inputPosY)
    {
        
    }
    //==========================处理网络数据结束=============================================================
    //==========================处理摇杆等本地操作开始=======================================================
    void Start()
    {
        Camera mCamera = transform.Find("camera").GetComponent<Camera>();
        if (this.IsETCControl == false) 
        {
            mCamera.enabled = false;
            return;
        }
        mCamera.enabled = true;
        joy = GameObject.FindObjectOfType<ETCJoystick>();
        button_1 = GameObject.Find("ETCButton_1").GetComponent<ETCButton>();
        button_2 = GameObject.Find("ETCButton_2").GetComponent<ETCButton>();
        button_3 = GameObject.Find("ETCButton_3").GetComponent<ETCButton>();
        button_4 = GameObject.Find("ETCButton_4").GetComponent<ETCButton>();
        if (joy != null)
        {
            joy.onMoveStart.AddListener(StartMoveCallBack);
            joy.onMove.AddListener(MoveCallBack);
            joy.onMoveEnd.AddListener(EndMoveCallBack);
        }
        if (button_1 != null)
        {
            button_1.onUp.AddListener(onUp_Skill_1);
            button_1.onDown.AddListener(onDown_Skill_1);
        }
        if (button_2 != null)
        {
            button_2.onUp.AddListener(onUp_Skill_2);
            button_2.onDown.AddListener(onDown_Skill_2);
        }
        if (button_3 != null)
        {
            button_3.onUp.AddListener(onUp_Skill_3);
            button_3.onDown.AddListener(onDown_Skill_3);
        }
        if (button_4 != null)
        {
            button_4.onUp.AddListener(onUp_Skill_4);
            button_4.onDown.AddListener(onDown_Skill_4);
        }
                
    }

    void StartMoveCallBack()
    {
        _UdpSendManager.SendStartMove();
    }

    void MoveCallBack(Vector2 tVec2)
    {
        TSVector2 mTSVector2 = new TSVector2((FP)(tVec2.x - 0.001f), (FP)(tVec2.y - 0.001f)).normalized;//这里减0.001f是因为这里精度有问题,会传来一个极其微小大于1的浮点数,后面使用时候导致FP Acos()中算出来的1值(4294967297)大于FP.ONE值(4294967296)
        FP anglenew = TSVector2.Angle(mTSVector2, TSVector2.up);
        FP angle = TSQuaternion.Angle(AllTSTransform.rotation, RotateTSTransform.rotation);
        //FP iTanDeg = TSMath.Atan2((FP)tVec2.y, (FP)tVec2.x) * FP.Rad2Deg;
        //Debug.LogErrorFormat("MoveCallBack===12=======>{0},{1},{2},{3},iTanDeg={4}", mTSVector2.x, mTSVector2.y, mTSVector2.ToString(), anglenew, iTanDeg);
        if (mTSVector2.x < 0 && (360 - anglenew) < 183) anglenew = 360 - anglenew; //这里让他们夹角在0-183范围内,刚好与下面的0-180错开3(灵敏度5的一半),这样就少算了一次RotateTransform.rotation.eulerAngles的值
        //if (mTSVector2.y < 0) anglenew = 360 - anglenew;
        //if (RotateTransform.rotation.eulerAngles.x < 0) angle = 360 - angle;
        //Debug.LogErrorFormat("MoveCallBack==2====>{0},{1},ToString={2}", anglenew, angle, RotateTSTransform.rotation.ToString());
        if (TSMath.Abs(angle - anglenew) > 5)
        {
            int x = FP.ToInt(mTSVector2.x * 1000);
            int y = FP.ToInt(mTSVector2.y * 1000);
            //Debug.LogErrorFormat("MoveCallBack=3==>{0},{1}", x, y);
            _UdpSendManager.SendChangeAngle(x,y);
        }
    }

    void EndMoveCallBack()
    {
        _UdpSendManager.SendEndMove();
    }

    void onUp_Skill_1()
    {
        _UdpSendManager.SendInputSkill(1, InputType.KeyUp);
    }
    void onDown_Skill_1()
    {
        _UdpSendManager.SendInputSkill(1, InputType.KeyDown);
    }
    
    void onUp_Skill_2()
    {
        _UdpSendManager.SendInputSkill(2, InputType.KeyUp);
    }
    void onDown_Skill_2()
    {
        _UdpSendManager.SendInputSkill(2, InputType.KeyDown);
    }

    void onUp_Skill_3()
    {
        _UdpSendManager.SendInputSkill(3, InputType.KeyUp);
    }
    void onDown_Skill_3()
    {
        _UdpSendManager.SendInputSkill(3, InputType.KeyDown);
    }

    void onUp_Skill_4()
    {
        _UdpSendManager.SendInputSkill(4, InputType.KeyUp);
    }
    void onDown_Skill_4()
    {
        _UdpSendManager.SendInputSkill(4, InputType.KeyDown);
    }


    void OnDestroy()
    {
        if (joy != null)
        {
            joy.onMoveStart.RemoveListener(StartMoveCallBack);
            joy.onMove.RemoveListener(MoveCallBack);
            joy.onMoveEnd.RemoveListener(EndMoveCallBack);
        }
        if (button_1 != null)
        {
            button_1.onUp.RemoveListener(onUp_Skill_1);
            button_1.onUp.RemoveListener(onDown_Skill_1);
        }
        if (button_2 != null)
        {
            button_2.onUp.RemoveListener(onUp_Skill_2);
            button_2.onUp.RemoveListener(onDown_Skill_2);
        }
        if (button_3 != null)
        {
            button_3.onUp.RemoveListener(onUp_Skill_3);
            button_3.onUp.RemoveListener(onDown_Skill_3);
        }
        if (button_4 != null)
        {
            button_4.onUp.RemoveListener(onUp_Skill_4);
            button_4.onUp.RemoveListener(onDown_Skill_4);
        }
    }
    //==========================处理摇杆等本地操作结束=======================================================
}
