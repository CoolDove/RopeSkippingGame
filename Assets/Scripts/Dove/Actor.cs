using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dove.Core
{
    public abstract class Actor : MonoBehaviour
    {
        [HideInInspector]
        public static bool Pausing {
            get
            {
                return _pausing;
            }
            set
            {
                if (_pausing != value) 
                {
                    _pausing = value;
                }
            }
        }

        private static bool _pausing = false;


        void Update()
        {
            if (!Pausing)
            {
                PUpdate();
            }
        }

        private void FixedUpdate()
        {
            if (!Pausing)
            {
                PFixedUpdate();
            }
        }

        protected abstract void PFixedUpdate();

        protected abstract void PUpdate();

    }
}

