using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System;

namespace Imato.Net.Cmd
{
    public class ShellRunner
    {
        private readonly string? _command, _workDir;
        private readonly ILogger? _logger;
        private readonly Action? _onExitHandler;
        private Process? _process;

        public ShellRunner(string command,
            string? workDir = null,
            ILogger? logger = null,
            Action? onExitHandler = null)
        {
            _command = command;
            _workDir = workDir ?? Environment.CurrentDirectory;
            _logger = logger;
            _onExitHandler = onExitHandler;
        }

        private void Log(string message, LogLevel level)
        {
            if (_logger != null && !string.IsNullOrEmpty(message))
            {
                _logger.Log(level, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {level}: {message}");
            }
        }

        private void LogExit(object sender, EventArgs e)
        {
            Log($"End execute command {_command}", LogLevel.Information);
            _onExitHandler?.Invoke();
        }

        public void Run()
        {
            Start();
            End();
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_command))
            {
                Log("Command is empty", LogLevel.Error);
                return;
            }

            var exe = _command.Split(" ")[0];
            var cmdInfo = new ProcessStartInfo(exe, _command.Replace($"{exe} ", ""));
            cmdInfo.WorkingDirectory = _workDir;
            cmdInfo.RedirectStandardOutput = true;
            cmdInfo.RedirectStandardError = true;
            cmdInfo.UseShellExecute = false;
            cmdInfo.CreateNoWindow = true;
            _process = new Process { StartInfo = cmdInfo, EnableRaisingEvents = true };
            _process.OutputDataReceived += (o, e) => Log(e.Data, LogLevel.Information);
            _process.ErrorDataReceived += (o, e) => Log(e.Data, LogLevel.Information);
            _process.Exited += LogExit;
            Log($"Start command: {_command} in directory {cmdInfo.WorkingDirectory}", LogLevel.Information);
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _process.WaitForExit();
        }

        public void End()
        {
            Log($"End {_command}", LogLevel.Information);
            _process?.Close();
        }
    }
}