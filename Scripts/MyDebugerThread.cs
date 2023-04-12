using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ThreadTask
{
    public class MyDebugerThread : MonoBehaviour
    {
        // main scene
        public Button btn1;
        public Button btn2;
        public Button btn3;
        public Button btn4;

        private Thread _thread = null;
        private object _logLock = null;
        private Queue<string> _queueLog = null;

        public void PrintDebugLog(string msg)
        {
            lock (_logLock)
            {
                _queueLog.Enqueue(msg);
            }
        }

        void Awake()
        {
            _logLock = new object();
            _queueLog = new Queue<string>();
            btn1.onClick.AddListener(() => PrintDebugLog("1 click"));
            btn2.onClick.AddListener(() => PrintDebugLog("2 click"));
            btn3.onClick.AddListener(() => PrintDebugLog("3 click"));
            btn4.onClick.AddListener(() => PrintDebugLog("4 click"));
        }

        void Start()
        {
            CloseThread();
            _thread = new Thread(PrintLog);
            _thread.Start();
            UnityDebugLog("=====Start PrintLog Thread=====");
        }

        void Update()
        {
            ThreadState state = _thread.ThreadState;
        }

        void OnDestroy()
        {
            CloseThread();
        }

        private void UnityDebugLog(string msg)
        {
            Debug.Log($"{msg} <color=#ffff00>[{Thread.CurrentThread.ManagedThreadId}]</color>");
        }

        private void PrintLog()
        {
            while (true)
            {
                if (_queueLog.Count <= 0)
                    continue;

                lock (_logLock)
                {
                    UnityDebugLog(_queueLog.Dequeue());
                }
            }
        }

        private void CloseThread()
        {
            if (_thread != null)
            {
                _thread.Abort();
                UnityDebugLog("=====Stop PrintLog Thread=====");
            }
        }
    }
}
