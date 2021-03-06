﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class VirtualClient: MonoBehaviour,IVirtualClient
{
    GameObject m_clientAssets;
    List<Tank> m_Tanks=new List<Tank>();

    LockStepManager m_lockStepManager;

    Queue<IGeneralAction> m_generalActions=new Queue<IGeneralAction>();

    public int ID { get ; private set; }
    public bool BattleStart { get; set ; }

    public void Init(int clientId,GameObject clientAssets)
    {
        this.ID = clientId;
        this.m_clientAssets = clientAssets;
        LoadClientAssets();
        InitReadyBtn();
        this.m_lockStepManager = new LockStepManager();
    }


    public void InitPlayerTrans(Dictionary<int, PlayerTransFixData> playerInitTransDatas)
    {
        foreach (var clientId_playerTransFixData in playerInitTransDatas)
        {
            var tank = new Tank();
            var tankGo = Instantiate<GameObject>(VirtualManager.Instance.m_tankPrefab);
            tankGo.transform.SetParent(this.m_clientAssets.transform);
            tank.InitTank(clientId_playerTransFixData.Value, tankGo);
            m_Tanks.Add(tank);
        }
    }


    public void OnReceiveGeneralAction(IGeneralAction action)
    {
        m_generalActions.Enqueue(action);
    }

    public void OnReceiveLockStepAction(ILockStepAction action)
    {
        throw new NotImplementedException();
    }

    public void SendClientReady()
    {
        VirtualManager.Instance.SendReadyToServer(this.ID);
    }

    public void SendLockStepAction(ILockStepAction action)
    {
        throw new NotImplementedException();
    }

    void InitReadyBtn()
    {
        var readyBtnTrans=this.m_clientAssets.transform.FindDeepChild("ReadyBtn");
        readyBtnTrans.gameObject.GetComponent<Button>().onClick.
            AddListener(() => 
            {
                readyBtnTrans.gameObject.SetActive(false);
                SendClientReady();
            });
    }


    private void LoadClientAssets()
    {
        this.m_clientAssets.transform.SetParent(this.transform,false);
    }

    private void Update()
    {
        while (m_generalActions.Count > 0)
        {
            m_generalActions.Dequeue().Handle(this);
        }

        if (!this.BattleStart)
        {
            return;
        }
        else
        {
            this.m_lockStepManager.Update();
        }

        Debug.Log($"{this.ID} LockStep");
    }




    //private void Awake()
    //{
    //    Debug.Log("Virtual Client Started");
    //}


    //public int ID = 0;

    //public bool battleStart = false;
    //private LockStepManager m_lockStepManager;

    // public void Init()
    //{
    //    m_lockStepManager = new LockStepManager(this);
    //    VirtualManager.Instance.ConnectToServer(ID);
    //}

    //private void Update()
    //{
    //    while(m_generalActions.Count>0)
    //    {
    //        m_generalActions.Dequeue().ProcessAction(this);
    //    }

    //    if(battleStart)
    //    {
    //        m_lockStepManager.Update();
    //    }
    //}

    //public void OnReceiveGeneralAction(IAction action)
    //{
    //}

    //public void OnReceiveLockStepAction(int lockStepTurn,int playerid,IAction action)
    //{
    //    this.m_lockStepManager.ReceiveAction(lockStepTurn, playerid, action);
    //}

    //public void SendLockStepAction(int lockStepId, int playerid, IAction action)
    //{
    //    VirtualManager.Instance.SendToServerLockStepAction(lockStepId, ID, action);
    //}


    //public void ForwardPlayer(int id)
    //{
    //    if(id==this.ID)
    //    {
    //        player.transform.transform.position += player.transform.forward;
    //    }
    //    else
    //    {
    //       otherPlayers[0].transform.transform.position += player.transform.forward;
    //    }

    //}

    //GameObject player;
    //List<GameObject>otherPlayers=new List<GameObject>();
    //public void InitPlayer(Vector3 pos)
    //{
    //    player= GameObject.CreatePrimitive(PrimitiveType.Capsule);
    //    player.GetComponent<MeshRenderer>().material.color = Color.red;
    //    player.name = this.ID.ToString();
    //    player.transform.SetParent(this.transform);
    //    player.transform.position = pos;
    //    player.layer = this.ID+8;
    //}

    //public void InitOtherPlayer(int id,Vector3 pos)
    //{
    //   var otherPlayer= GameObject.CreatePrimitive(PrimitiveType.Capsule);
    //    otherPlayer.name = id.ToString();
    //    otherPlayer.transform.SetParent(this.transform);
    //    otherPlayer.transform.position = pos;
    //    otherPlayer.layer = this.ID+8;
    //    otherPlayers.Add(otherPlayer);
    //}

}
