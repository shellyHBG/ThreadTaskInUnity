using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTask
{
    public sealed class MyLogTask
    {
        public static MyLogTask Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MyLogTask();
                    _instance.InitLoggerTask();
                }
                return _instance;
            }
        }
        private static MyLogTask _instance;

        private object _logLock = null;
        private Queue<string> _queueLog = null;
        private CancellationTokenSource _tokenSource = null;
        private IMyLogger _mylogger = null;
        private bool _bDispose = false;

        public static void Log(string msg)
        {
            Instance.PrintLogEnqueue(msg);
        }

        public static void LogError(string msg)
        {
            Instance.PrintLogEnqueue(msg);
        }

        public static void LogWarning(string msg)
        {
            Instance.PrintLogEnqueue(msg);
        }

        public static void Dispose()
        {
            if (_instance == null)
                return;

            if (!_instance._bDispose)
            {
                _instance._bDispose = true;
                _instance.CancelTaskPrintLog();
                _instance = null;
            }
        }

        private void InitLoggerTask()
        {
            _logLock = new object();
            _queueLog = new Queue<string>();
            _mylogger = FactoryMyLogger.Create();

            CreateTaskPrintLog();
        }

        private async void CreateTaskPrintLog()
        {
            if (_tokenSource != null)
                return;

            _tokenSource = new CancellationTokenSource();
            _mylogger.Log("=====Start PrintLog Task=====");

            try
            {
                await Task.Run(() => ProcessPrintLog(_tokenSource.Token));
            }
            catch (OperationCanceledException ex)
            {
                _mylogger.LogError("=====Cancel PrintLog Task=====");
            }
            finally
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }

            _mylogger.Log("=====End PrintLog Task=====");
        }

        private void ProcessPrintLog(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();   // throw exception

                if (_queueLog.Count <= 0)
                    continue;

                lock (_logLock)
                {
                    _mylogger.Log(_queueLog.Dequeue());
                }
            }
        }

        private void CancelTaskPrintLog()
        {
            if (_tokenSource == null)
                return;

            _tokenSource.Cancel();
        }

        /// <summary>
        /// Queue log
        /// </summary>
        private void PrintLogEnqueue(string msg)
        {
            lock (_logLock)
            {
                _queueLog.Enqueue(msg);
            }
        }
    }
}
