using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LogExtraction
{
    public class LogExtractionTool : MonoBehaviour
    {
        public bool SaveLogsOnPause { get; private set; } = true;
        private StringBuilder _sb = new();

        public void Setup(bool enableSaveLogsOnPause = true)
        {
            SaveLogsOnPause = enableSaveLogsOnPause;
        }

        public void SendLogs()
        {
            DataSaver.SaveData(_sb.ToString(), "savelog");
            new NativeShare().AddFile(DataSaver.GetFilePath("savelog"))
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();
        }

        //Called when there is an exception
        void LogCallback(string condition, string stackTrace, LogType type)
        {
            _sb.Append($"{type}\n");
            _sb.Append($"{DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")}\n");
            _sb.Append($"{condition}\n");
            _sb.Append($"{stackTrace}");
            _sb.Append($"\n");
        }

        void Awake()
        {
            //Subscribe to Log Event
            Application.logMessageReceived += LogCallback;
        }

        void OnDestroy()
        {
            //Un-Subscribe from Log Event
            Application.logMessageReceived -= LogCallback;
        }

        //Save log  when focous is lost
        void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                //Save
                if (SaveLogsOnPause)
                    DataSaver.SaveData(_sb.ToString(), "savelog");
            }
        }

        //Save log on exit
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                //Save
                if (SaveLogsOnPause)
                    DataSaver.SaveData(_sb.ToString(), "savelog");
            }
        }
    }
}