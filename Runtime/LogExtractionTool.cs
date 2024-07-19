using System;
using System.Text;
using UnityEngine;

namespace LogExtraction
{
    public class LogExtractionTool : MonoBehaviour
    {
        public bool SaveLogsOnPause { get; private set; } = true;
        private StringBuilder _sb = new();

        private LogStreaming _logStream;

        public void Setup(bool enableSaveLogsOnPause = true)
        {
            SaveLogsOnPause = enableSaveLogsOnPause;
        }

        public void SendLogs()
        {
            new NativeShare().AddFile(_logStream.CurrentSessionLogsPath)
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();
        }

        public byte[] GetCurrentSessionLogsBytes()
        {
            return _logStream.GetCurrentSessionLogsBytes();
        }

        public byte[] GetPreviousSessionLogsBytes()
        {
            return _logStream.GetPreviousSessionLogsBytes();
        }

        void Awake()
        {
            _logStream = new LogStreaming();
            
            //Subscribe to Log Event
            Application.logMessageReceived += _logStream.AppendCurrentLog;
        }

        void OnDestroy()
        {
            //Un-Subscribe from Log Event
            Application.logMessageReceived -= _logStream.AppendCurrentLog;
            
            _logStream.Dispose();
        }

        //Save log  when focus is lost
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                // flush buffered logs to file on application out of focus
                _logStream.Flush();
            }
        }

        //Save log on exit
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // flush buffered logs to file on application pause
                _logStream.Flush();
            }
        }
    }
}