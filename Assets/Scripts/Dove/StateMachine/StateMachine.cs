using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dove.Core
{
    public class StateMachine
    {
        public int State { get { return _state; } }

        private State[] states;
        private int _state;
        public int lastState;
        private State currentState { get { return states[_state]; } }

        public StateMachine(int stateCount)
        {
            if (stateCount > 1) 
            {
                states = new State[stateCount];
                for (int i = 0; i < stateCount; i++)
                {
                    states[i] = new State();
                }
            }
            else
            {
                Debug.LogError("State Count Should Be Larger Than 1");
            }

            CoreManager.Instance.FUpdate += Update;
            CoreManager.Instance.FFixedUpdate += FixedUpdate;
        }

        public void SetState(int stateNum, Action start, Action update, Action fixedupdate, Action end)
        {
            states[stateNum].Start = start;
            states[stateNum].Update = update;
            states[stateNum].FixedUpdate = fixedupdate;
            states[stateNum].End = end;
        }

        public void StartMachine()
        {
            _state = 0;
            lastState = 0;
            if (currentState.Start != null)
            {
                currentState.Start();
            }
        }

        public void ChangeState(int stateNum)
        {
            if (stateNum < states.Length && stateNum > -1)
            {
                if (stateNum != State) 
                {
                    if (currentState.End != null)
                    {
                        currentState.End();
                    }
                    lastState = _state;
                    _state = stateNum;
                    if (currentState.Start != null)
                    {
                        currentState.Start();
                    }
                }
            }
            else
            {
                Debug.LogError("StateIndexBeyond");
            }
        }

        private void Update()
        {
            if (currentState.Update != null) 
            {
                currentState.Update();
            }
        }
        private void FixedUpdate()
        {
            if (currentState.FixedUpdate != null)
            {
                currentState.FixedUpdate();
            }
        }


    }
    public class ActorStateMachine
    {
        public int State { get { return _state; } }

        private State[] states;
        private int _state;
        public int lastState;
        private State currentState { get { return states[_state]; } }

        public ActorStateMachine(int stateCount)
        {
            if (stateCount > 1)
            {
                states = new State[stateCount];
                for (int i = 0; i < stateCount; i++)
                {
                    states[i] = new State();
                }
            }
            else
            {
                Debug.LogError("State Count Should Be Larger Than 1");
            }

            CoreManager.Instance.FUpdate += Update;
            CoreManager.Instance.FFixedUpdate += FixedUpdate;
        }

        public void SetState(int stateNum, Action start, Action update, Action fixedupdate, Action end)
        {
            states[stateNum].Start = start;
            states[stateNum].Update = update;
            states[stateNum].FixedUpdate = fixedupdate;
            states[stateNum].End = end;
        }

        public void StartMachine()
        {
            _state = 0;
            lastState = 0;
            if (currentState.Start != null)
            {
                currentState.Start();
            }
        }

        public void ChangeState(int stateNum)
        {
            if (stateNum < states.Length && stateNum > -1)
            {
                if (stateNum != State)
                {
                    if (currentState.End != null)
                    {
                        currentState.End();
                    }
                    lastState = _state;
                    _state = stateNum;
                    if (currentState.Start != null)
                    {
                        currentState.Start();
                    }
                }
            }
            else
            {
                Debug.LogError("StateIndexBeyond");
            }
        }

        private void Update()
        {
            if (!Actor.Pausing)
            {
                if (currentState.Update != null)
                {
                    currentState.Update();
                }
            }
        }
        private void FixedUpdate()
        {
            if (!Actor.Pausing)
            {
                if (currentState.FixedUpdate != null)
                {
                    currentState.FixedUpdate();
                }
            }
        }
    }
    public class State
    {
        public Action Start;
        public Action Update;
        public Action FixedUpdate;
        public Action End;
    }
}

