using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Dove.Core
{
    public class CoreManager : MonoBehaviour
    {
        public static CoreManager Instance { get { return _instance; } }
        private static CoreManager _instance;

        public bool pausing { get { return Actor.Pausing; } }
        private bool systemPausing = false;
        public bool sysPausing { get { return systemPausing; } }

        public Action FAwake;
        public Action FStart;
        public Action FUpdate;
        public Action FFixedUpdate;

        public Action FPAwake;
        public Action FPStart;
        public Action FPUpdate;
        public Action FPFixedUpdate;


        #region Mono
        private void Awake()
        {
            _instance = this;
            Time.timeScale = 1;
            if (FAwake != null)
            {
                FAwake();
            }
            if (FPAwake != null && pausing)
            {
                FPAwake();
            }
        }
        private void Start()
        {
            if (FStart != null)
            {
                FStart();
            }
            if (FPStart != null && pausing)
            {
                FPStart();
            }
        }
        private void Update()
        {
            if (FUpdate != null)
            {
                FUpdate();
            }
            if (FPUpdate != null && pausing)
            {
                FPUpdate();
            }
        }
        private void FixedUpdate()
        {
            if (FFixedUpdate != null)
            {
                FFixedUpdate();
            }
            if (FPFixedUpdate != null && pausing)
            {
                FPFixedUpdate();
            }
        }
        #endregion

        public void PauseGame()
        {
            systemPausing = true;
            Actor.Pausing = true;

        }
        public void ResumeGame()
        {
            systemPausing = false;
            Actor.Pausing = false;
        }

        public void StrikePause(float waitTime,float pauseTime)
        {
            if (!systemPausing)
            {
                StartCoroutine(PauseStriker(waitTime, pauseTime));
            }
        }

        private IEnumerator PauseStriker(float waitTime, float time)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Actor.Pausing = true;
            yield return new WaitForSecondsRealtime(time);
            if (!systemPausing)
            {
                Actor.Pausing = false;
            }
        }
    }




}

