using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dove;
using Dove.Core;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField]
    private Rope rope;
    [SerializeField]
    private float diePrepareTime = 0.5f;

    public int score = 0;

    public StateMachine GameStateMachine { get; private set; }
    private const int ST_Menu= 0;
    private const int ST_StartPrepare = 1;
    private const int ST_Running = 2;
    private const int ST_Pausing = 3;
    private const int ST_DeadPrepare = 4;
    private const int ST_Dead = 5;

    private WaitUntil diePrepareWaiter;

    private void Awake()
    {
        _instance = this;
        //订阅事件
        SubscribeEvents();
        //设置游戏状态机
        GameStateMachine = new StateMachine(6);
        GameStateMachine.SetState(ST_Menu, STMFMenuStart, null, null, STMFMenuEnd);
        GameStateMachine.SetState(ST_StartPrepare, STMFStartPrepareStart, null, null, STMFStartPrepareEnd);
        GameStateMachine.SetState(ST_Running, STMFRunningStart, null, null, null);
        GameStateMachine.SetState(ST_Pausing, STMFPauseStart, null, null, STMFPauseEnd);
        GameStateMachine.SetState(ST_DeadPrepare, STMFDeadPrepareStart, null, null, null);
        GameStateMachine.SetState(ST_Dead, STMFDeadStart, null, null, STMFDeadEnd);

        diePrepareWaiter = new WaitUntil(()=>rope.Recovered);
    }



    private void Start()
    {
        //读入数据
        GlobalVar.records = Saver.Instance.GetData();

        UIManager.OpenUI("UICStatic");
        GameStateMachine.StartMachine();

    }

    private void SubscribeEvents()
    {
        EventManager.Instance.SubscribeEvent("RopeHitPlayer",OnRopeHitPlayer);
        EventManager.Instance.SubscribeEvent("RopeOneRound", () => {
            if (GameStateMachine.State == ST_Running) 
            {
                EventManager.Instance.SendEvent("ScoreGet");
                if (score == 99 || score == 59|| score == 39|| score == 19|| score == 9) 
                {
                    DifficultyIncrease();
                }
                score++;
            }
        });
        EventManager.Instance.SubscribeEvent("StartGame",OnStartGame);
        EventManager.Instance.SubscribeEvent("PauseGame", OnPauseGame);
        EventManager.Instance.SubscribeEvent("ResumeGame", OnResumeGame);
        EventManager.Instance.SubscribeEvent("RestartGame", OnRestartGame);
        EventManager.Instance.SubscribeEvent("BackToMenu", OnBackToMenu);
    }

    private void DifficultyIncrease()
    {
        if (GlobalVar.difficulty < Difficulty.MaxDifficulty) 
        {
            GlobalVar.difficulty++;
        }
        EventManager.Instance.SendEvent("DifficultyIncrease");
    }

    #region STMF
    private void STMFMenuStart()
    {
        rope.ResetRope();
        score = 0;

        System.GC.Collect();

        if (GameStateMachine.lastState == ST_Pausing)
        {
            UIManager.CloseUI("UICPlaying");
        }
        ActDrop.Instance.Blur();
        UIManager.OpenUI("UICStartMenu");
    }
    private void STMFMenuEnd()
    {
        ActDrop.Instance.Clear();
        UIManager.CloseUI("UICStartMenu");
    }

    private void STMFStartPrepareStart()
    {
        score = 0;
        GlobalVar.difficulty = 0;
        ActDrop.Instance.ActIn();
        StartCoroutine(StartPrepareWait());
    }
    private IEnumerator StartPrepareWait()
    {
        yield return new WaitUntil(() => ActDrop.Instance.In);
        GameStateMachine.ChangeState(ST_Running);
    }
    private void STMFStartPrepareEnd()
    {
        ActDrop.Instance.ActOut();
    }

    private void STMFRunningStart()
    {
        ActDrop.Instance.Clear();

        EventManager.Instance.SendEvent("GameStart");

        UIManager.OpenUI("UICPlaying");
    }

    private void STMFPauseStart()
    {
        ActDrop.Instance.Blur();
        CoreManager.Instance.PauseGame();
        EventManager.Instance.SendEvent("ScoreTextOut");
    }
    private void STMFPauseEnd()
    {
        CoreManager.Instance.ResumeGame();
        EventManager.Instance.SendEvent("ScoreTextBack");
    }

    private void STMFDeadPrepareStart()
    {
        UIManager.CloseUI("UICPlaying");
        EventManager.Instance.SendEvent("ScoreTextOut");

        StartCoroutine(DiePrepare());
    }
    
    private IEnumerator DiePrepare()
    {
        ActDrop.Instance.Blur();
        GlobalVar.diePreparing = true;
        yield return diePrepareWaiter;
        if (GameStateMachine.State == ST_DeadPrepare) 
        {
            GameStateMachine.ChangeState(ST_Dead);
        }
    }
    private void STMFDeadStart()
    {
        Saver.Instance.AddData(score);
        GlobalVar.records = Saver.Instance.data;

        System.GC.Collect();

        UIManager.OpenUI("UICDead");
        EventManager.Instance.SendEvent("GameDie");
        GlobalVar.diePreparing = false;
    }
    private void STMFDeadEnd()
    {
        UIManager.CloseUI("UICDead");
        EventManager.Instance.SendEvent("ScoreTextBack");
        score = 0;
    }
    #endregion


    private void Update()
    {
        GlobalVar.score = score;
    }


    #region 事件响应
    private void OnStartGame()
    {
        if (GameStateMachine.State == ST_Menu)
        {
            GameStateMachine.ChangeState(ST_StartPrepare);
        }
    }

    private void OnPauseGame()
    {
        if (GameStateMachine.State == ST_Running) 
        {
            GameStateMachine.ChangeState(ST_Pausing);
        }
    }
    private void OnResumeGame()
    {
        if (GameStateMachine.State == ST_Pausing)
        {
            GameStateMachine.ChangeState(ST_Running);
        }
    }
    private void OnRestartGame()
    {
        if (GameStateMachine.State == ST_Dead || GameStateMachine.State == ST_Pausing) 
        {
            GameStateMachine.ChangeState(ST_StartPrepare);
        }
    }
    private void OnBackToMenu()
    {
        if (GameStateMachine.State == ST_Dead || GameStateMachine.State == ST_Pausing) 
        {
            GameStateMachine.ChangeState(ST_Menu);
        }
    }
    private void OnRopeHitPlayer()
    {
        if (GameStateMachine.State == ST_Running) 
        {
            CoreManager.Instance.StrikePause(0, 0.14f);
            GameStateMachine.ChangeState(ST_DeadPrepare);
            EventManager.Instance.SendEvent("ShakeCamera");
        }
    }

    #endregion

}
