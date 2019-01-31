using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using Google.Protobuf;
using TrueSync;

//用来控制整个游戏进程,怎么玩,游戏规则等
public class GameProcessManager_Dota : TrueSyncBehaviour
{
	ETCButton FightEndBtn;		//模拟游戏结束的按钮
	public GameObject[] WillUsedPrefabs; 

	void Awake()
	{
		FightEndBtn = GameObject.Find("FightEndBtn").GetComponent<ETCButton>();
		FightEndBtn.onUp.AddListener(onUp_FightEnd);
		InitWillUsedPrefabs();
	}

	protected void InitWillUsedPrefabs()
	{
		WillUsedPrefabs = new GameObject[3];
		WillUsedPrefabs[0] = _AssetManager.GetGameObject("prefab/monsters/yeguai_prefab");
		WillUsedPrefabs[0].SetActive(false);
	}
	void onUp_FightEnd()
	{
		_UdpSendManager.SendFightEnd();
		Debug.Log("模拟游戏结束");
	}

	public override void OnSyncedStart()
	{
		Debug.LogErrorFormat("GameProcessManager_Dota====>OnSyncedStart");
		MyTimerDriver.Instance.SetTimer(60, "StartGenerateYeGuai", StartGenerateYeGuai);//new Action(StartGenerateYeGuai)
	}

	private void StartGenerateYeGuai() {
		WillUsedPrefabs[0].SetActive(true);//如果带了刚体等组件的,需要先激活再实例化,不然这些组件无法注册到系统中(子弹这种只有trigger的,可以不先激活)
		YeGuaiAI mYeGuaiAI = WillUsedPrefabs[0].gameObject.GetComponent<YeGuaiAI>();
		Debug.LogErrorFormat("StartGenerateYeGuai====>{0}", TrueSyncManager.Ticks);
		mYeGuaiAI.mBackPosition = new TSVector(3, 0, 3);
		GameObject yeguai_prefab1 = TrueSyncManager.SyncedInstantiate(WillUsedPrefabs[0], new TSVector(3, 0, 3), TSQuaternion.identity);
		mYeGuaiAI.mBackPosition = new TSVector(-3, 0, -3);
		GameObject yeguai_prefab2 = TrueSyncManager.SyncedInstantiate(WillUsedPrefabs[0], new TSVector(-3, 0, -3), TSQuaternion.identity);
		mYeGuaiAI.mBackPosition = new TSVector(-3, 0, 3);
		GameObject yeguai_prefab3 = TrueSyncManager.SyncedInstantiate(WillUsedPrefabs[0], new TSVector(-3, 0, 3), TSQuaternion.identity);
		mYeGuaiAI.mBackPosition = new TSVector(3, 0, -3);
		GameObject yeguai_prefab4 = TrueSyncManager.SyncedInstantiate(WillUsedPrefabs[0], new TSVector(3, 0, -3), TSQuaternion.identity);
		WillUsedPrefabs[0].SetActive(false);
	}

	public override void OnSyncedUpdate()
	{
		//Debug.LogErrorFormat("====>Ticks={0},LastSafeTick={1},Time={2},DeltaTime={3}", TrueSyncManager.Ticks, TrueSyncManager.LastSafeTick, TrueSyncManager.Time, TrueSyncManager.DeltaTime);
    }

}