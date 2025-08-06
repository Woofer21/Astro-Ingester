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
                    ConsoleHelpers.Error($"Invalid config line at {configFileInfo.FullName}:{i + 1}: '{line}'");
                    didntFail = false;
                    continue;
                }

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch(key)
                {
                    case "InputPath":
                    case "Input_Path":
                    case "input_path":
                        didntFail &= HandleInputPath(key, value);
                        if (didntFail)
                        {
                            ConsoleHelpers.Muted($"[info(CNFG)] Loaded input path: success({value})");
                        }
                        break;
                }
            }

            return didntFail;
        }

        private bool HandleInputPath(string key, string value)
        {
            if (_inputPathSet)
            {
                ConsoleHelpers.Error($"You can only have one {key} set");
                return false;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                ConsoleHelpers.Error($"{key} must be passed a value and can not be empty");
                return false;
            }

            if (!Directory.Exists(value))
            {
                ConsoleHelpers.Error($"{key} directory does not exist.");
                return false;
            }

            _fileTools.ManuallySelectDrive(value);
            _inputPathSet = true;
			return true;
        }
    }
}