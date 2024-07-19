using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LogExtraction
{
    public class LogStreaming : IDisposable
    {
        private const string CURRENT_SESSION_LOGS_FILENAME = "logs.txt";
        private const string PREVIOUS_SESSION_LOGS_FILENAME = "previous-session-logs.txt";
        private const int LENGTH_OF_LOG_MESSAGE_TO_WRITE_TO_FILE = 2000;

        private FileStream CurrentSessionLogs { get; set; }
        private FileStream PreviousSessionLogs { get; set; }
        private StreamWriter CurrentSessionLogsWriter { get; set; }

        private readonly string _currentSessionLogsPath;
        public string CurrentSessionLogsPath => _currentSessionLogsPath;
        private readonly string _previousSessionLogsPath;
        private readonly StringBuilder _sb;

        public LogStreaming()
        {
            _sb = new StringBuilder();
            
            _currentSessionLogsPath = GetFilePath(CURRENT_SESSION_LOGS_FILENAME);
            _previousSessionLogsPath = GetFilePath(PREVIOUS_SESSION_LOGS_FILENAME);
            
            // make sure that the directory for logs files exists
            var directoryPath = GetDirectoryPath();
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }
            
            CurrentSessionLogs = new FileStream(_currentSessionLogsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            
            // if there is a current logs file that is not empty:
            if (CurrentSessionLogs.Length > 0)
            {
                // 0. dispose stream to current logs file
                CurrentSessionLogs.Dispose();
                // 1. remove previous logs file
                File.Delete(_previousSessionLogsPath);
                // 2. rename current logs file to previous logs file
                File.Move(_currentSessionLogsPath, _previousSessionLogsPath);
                // 3. create new current logs file
                CurrentSessionLogs = new FileStream(_currentSessionLogsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }

            PreviousSessionLogs = new FileStream(_previousSessionLogsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            CurrentSessionLogsWriter = new StreamWriter(CurrentSessionLogs);
        }

        public void AppendCurrentLog(string condition, string stackTrace, LogType type)
        {
            var timestamp = DateTime.Now;
            lock (_sb)
            {
                _sb.Append($"{type}\n");
                _sb.Append($"{timestamp:yyyy-MM-ddTHH:mm:sszzz}\n");

                if (condition.Length > LENGTH_OF_LOG_MESSAGE_TO_WRITE_TO_FILE)
                {
                    _sb.Append($"{condition.Substring(0, Math.Min(condition.Length, LENGTH_OF_LOG_MESSAGE_TO_WRITE_TO_FILE))}\n");
                    _sb.Append("<log message was truncated>\n");
                }
                else
                {
                    _sb.Append($"{condition}");
                }
                _sb.Append($"{stackTrace}");
                _sb.Append($"\n");
                
                CurrentSessionLogsWriter.Write(_sb);
                
                _sb.Clear();
            }
        }

        /// <summary>
        /// Returns full path with filename where the file with given filename will be stored.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string GetFilePath(string filename)
        {
            var tempPath = GetDirectoryPath();
            return Path.Combine(tempPath, filename);
        }

        /// <summary>
        /// Returns full path of the directory where all persistent data is saved by default (data directory in persistent path_
        /// </summary>
        /// <returns></returns>
        private static string GetDirectoryPath()
        {
            return Path.Combine(Application.persistentDataPath, "data");
        }

        /// <summary>
        /// Flushes all the data in write streams to files. Run this before getting current logs to make sure that all buffered logs are read correctly from file
        /// </summary>
        public void Flush()
        {
            try
            {
                CurrentSessionLogsWriter?.Flush();
            }
            catch (Exception ex)
            {
                // ignored, the stream can be closed/disposed and flush may not succeed, I'm not sure how to handle it correctly and not flush if it's not possible
            }
        }

        public byte[] GetCurrentSessionLogsBytes()
        {
            Flush();

            using var fs = File.OpenRead(_currentSessionLogsPath);
            var bytes = new byte[fs.Length];
            var bytesRead = fs.Read(bytes, 0, bytes.Length);
            if (bytes.Length != bytesRead)
            {
                Debug.LogError($"Read {bytesRead} from logs stream, but the log stream had {bytes.Length} length!");
            }
            return bytes;
        }

        public byte[] GetPreviousSessionLogsBytes()
        {
            using var fs = File.OpenRead(_previousSessionLogsPath);
            var bytes = new byte[fs.Length];
            var bytesRead = fs.Read(bytes, 0, bytes.Length);
            if (bytes.Length != bytesRead)
            {
                Debug.LogError($"Read {bytesRead} from previous session logs stream, but the log stream had {bytes.Length} length!");
            }
            return bytes;
        }
        
        public void Dispose()
        {
            Flush();

            try
            {
                CurrentSessionLogs?.Dispose();
                PreviousSessionLogs?.Dispose();
                CurrentSessionLogsWriter?.Dispose();
            }
            catch (Exception _)
            {
                // ignored, Dispose can throw an exception, but there is no action that we can take here, we are disposing all resources of this class
            }
        }
    }
}