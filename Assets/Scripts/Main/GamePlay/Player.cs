using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dove.Core;

public class Player : Actor
{
    [Header("Jump")]
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float defSpeed;
    [SerializeField][Range(0, 400)]
    private float speedDecreaseHolding;
    [SerializeField][Range(0, 400)]
    private float speedDecrease;
    [SerializeField]
    private float PJumpColdDown = 0.1f;

    private float speed;
    private bool holding;
    private float jumpTime = 0f;
    private float jumpColdDown = 0f;

    private Vector3 defPosition;

    private int touching;

    private bool cmdDown;
    private bool cmd;
    private bool cmdUp;
    
    private bool onGround { get { return transform.position.y == defPosition.y; } }

    private ActorStateMachine stm;
    private const int ST_Idle = 0;
    private const int ST_Jump = 1;
    private const int ST_Fall = 2;


    private Animator animator;

    private void Start()
    {
        defPosition = transform.position;

        animator = GetComponentInChildren<Animator>();

        stm = new ActorStateMachine(3);
        stm.SetState(ST_Idle, null, STMF_IdleUpdate, null, null);
        stm.SetState(ST_Jump, STMF_JumpStart, STMF_JumpUpdate, null, STMF_JumpEnd);
        stm.SetState(ST_Fall, STMF_FallStart, STMF_FallUpdate, null, STMF_FallEnd);
        stm.StartMachine();
        EventManager.Instance.SubscribeEvent("StartPrepare", () => transform.position = defPosition);
    }


    override protected void PUpdate()
    {
        cmdDown = MyInput.Instance.CommandDown;
        cmd = MyInput.Instance.CommandHold;
        cmdUp = MyInput.Instance.CommandRelease;

        animator.SetFloat("JumpToFall", 1 - (Mathf.Clamp(speed, 0, jumpSpeed) / jumpSpeed));
    }

    protected override void PFixedUpdate()
    {
    }

    #region STMF
    private void STMF_IdleUpdate()
    {
        if (!GlobalVar.diePreparing) 
        {
            if (jumpColdDown <= PJumpColdDown)
            {
                jumpColdDown += Time.deltaTime;
            }

            if (jumpColdDown >= PJumpColdDown && (cmdDown || cmd))
            {
                stm.ChangeState(ST_Jump);
            }
        }
    }

    private void STMF_JumpStart()
    {
        animator.SetBool("Land", false);
        animator.SetBool("Jump", true);
        speed = jumpSpeed;
        jumpTime = 0f;
    }
    private void STMF_JumpUpdate()
    {
        float delt = transform.position.y + speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, delt, transform.position.z);
        speed -= Time.deltaTime * (cmd ? speedDecreaseHolding : speedDecrease);
        if (speed < 0)    
        {
            stm.ChangeState(ST_Fall);
        }
    }
    private void STMF_JumpEnd()
    {
        animator.SetBool("Jump", false);
    }

    private void STMF_FallStart()
    {
        //speed = 0.5f * defSpeed;
    }
    private void STMF_FallUpdate()
    {

        float delt = transform.position.y + speed * Time.deltaTime;
        if (delt < defPosition.y) 
        {
            delt = defPosition.y;
        }
        
        transform.position = new Vector3(transform.position.x, delt, transform.position.z);

        if (speed != defSpeed) 
        {
            speed -= Time.deltaTime * speedDecrease;
            speed = speed < defSpeed ? defSpeed : speed;
        }
        
        if (delt == defPosition.y) 
        { 
            stm.ChangeState(ST_Idle);
        }
    }
    private void STMF_FallEnd()
    {
        jumpTime = 0;
        animator.SetBool("Land", true);
        jumpColdDown = 0f;
    }

    #endregion



}
