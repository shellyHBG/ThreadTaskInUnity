using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ShellyQQQ
{
    public class MyDebuggerTask : MonoBehaviour
    {
        // main scene
        public Button btn1;
        public Button btn2;
        public Button btn3;
        public Button btn4;
        public Button btnCancel;
        public Button btnStart;

        private object _logLock = null;
        private Queue<string> _queueLog = null;
        private CancellationTokenSource _tokenSource = null;

        void Awake()
        {
            _logLock = new object();
            _queueLog = new Queue<string>();

            //btn1.onClick.AddListener(() => PrintDebugLog1("1 click"));
            //btn2.onClick.AddListener(() => PrintDebugLog1("2 click"));
            //btn3.onClick.AddListener(() => PrintDebugLog1("3 click"));
            //btn4.onClick.AddListener(() => PrintDebugLog1("4 click"));

            btn1.onClick.AddListener(() => PrintDebugLog2("1 click"));
            btn2.onClick.AddListener(() => PrintDebugLog2("2 click"));
            btn3.onClick.AddListener(() => PrintDebugLog2("3 click"));
            btn4.onClick.AddListener(() => PrintDebugLog2("4 click"));
            btnCancel.onClick.AddListener(CancelPrintLog);
            btnStart.onClick.AddListener(CreateTaskPrintLog);
        }

        void Start()
        {
            CreateTaskPrintLog();
        }

        void OnDestroy()
        {
            CancelPrintLog();
        }

        /// <summary>
        /// Simple log task
        /// </summary>
        public async void PrintDebugLog1(string msg)
        {
            await Task.Run(()=> UnityDebugLog(msg));
        }

        /// <summary>
        /// Queue log Task
        /// </summary>
        public void PrintDebugLog2(string msg)
        {
            lock (_logLock)
            {
                _queueLog.Enqueue(msg);
            }
        }

        private void UnityDebugLog(string msg)
        {
            Debug.Log($"{msg} <color=#ffff00>[{Thread.CurrentThread.ManagedThreadId}]</color>");
        }

        private void UnityDebugErrorLog(string msg)
        {
            Debug.LogError($"{msg} <color=#ffff00>[{Thread.CurrentThread.ManagedThreadId}]</color>");
        }

        private void ProcessPrintLog(CancellationToken token)
        {
            while(true)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();   // throw exception

                if (_queueLog.Count <= 0)
                    continue;

                lock (_logLock)
                {
                    UnityDebugLog(_queueLog.Dequeue());
                }
            }
        }

        private async void CreateTaskPrintLog()
        {
            if (_tokenSource != null)
                return;

            _tokenSource = new CancellationTokenSource();
            UnityDebugLog("=====Start PrintLog Task=====");

            try
            {
                await Task.Run(() => ProcessPrintLog(_tokenSource.Token));
            }
            catch (OperationCanceledException ex)
            {
                UnityDebugErrorLog("=====Cancel PrintLog Task=====");
            }
            finally
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }

            UnityDebugLog("=====End PrintLog Task=====");
        }

        private void CancelPrintLog()
        {
            if (_tokenSource == null)
                return;

            _tokenSource.Cancel();
        }
    }
}
