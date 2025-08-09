using AstroIngesterCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCLI
{
    internal class ConfigManager (FileTools fileTools)
	{
        private readonly FileTools _fileTools = fileTools;
        private bool _inputPathSet = false;

		public bool LoadConfig(FileInfo configFileInfo)
        {
            bool didntFail = true;

            string[] lines = File.ReadAllLines(configFileInfo.FullName);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//"))
                    continue;

                string[] parts = line.Split('=');
                if (parts.Length != 2)
                {
                    Error($"Invalid config line at {configFileInfo.FullName}:{i + 1}: '{line}'");
                    didntFail = false;
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim().Trim('"').Trim();

                switch(key.ToLower())
                {
                    case "verbose":
                        if (value.ToLower() == "true")
                        {
                            _fileTools.Verbose = true;
                            Warning("Verbose enabled");
                        }
                        break;
                    case "inputpath":
                    case "input_path":
                        didntFail &= HandleInputPath(key, value);

                        if (didntFail)
                            Info($"Loaded {key}: {value}");

                        break;
                    default:
                        Error($"Unknown config key '{key}' at {configFileInfo.FullName}:{i + 1}");
                        didntFail = false;
                        break;
                }
            }

            return didntFail;
        }

        private bool HandleInputPath(string key, string value)
        {
            if (_inputPathSet)
            {
                Error($"You can only have one {key} set");
                return false;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                Error($"{key} must be passed a value and can not be empty");
                return false;
            }

            if (!Directory.Exists(value))
            {
                Error($"{key} directory does not exist.");
                return false;
            }

            _fileTools.ManuallySelectDrive(value);
            _inputPathSet = true;
			return true;
        }


        // ConfigManager logging helper functions
        private void Info(string message)
        {
            ConsoleHelpers.Muted($"[info(CNFG)] {message}");
        }
        private void Warning(string message)
        {
            ConsoleHelpers.Muted($"[warning(CNFG)] {message}");
        }
        private void Error(string message)
        {
            ConsoleHelpers.Muted($"[error(CNFG)] error({message})");
        }
    }
}