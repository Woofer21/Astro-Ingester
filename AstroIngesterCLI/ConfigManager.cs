using AstroIngesterCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                        bool inputSucc = HandleInputPath(key, value);
                        didntFail &= inputSucc;

                        if (inputSucc)
                            Info($"Loaded {key}: {value}");

                        break;
                    case "outputsort":
                    case "output_sort":
                        bool outSortSucc = HandleOutputPath(key, value, i + 1, "sort");
                        didntFail &= outSortSucc;

                        if (outSortSucc)
                            Info($"Loaded {key}: {value.Split(',')[0]}");

                        break;
                    case "outputcopy":
                    case "output_copy":
                        bool outCopySucc = HandleOutputPath(key, value, i + 1, "copy");
                        didntFail &= outCopySucc;

                        if (outCopySucc)
                            Info($"Loaded {key}: {value.Split(',')[0]}");

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

        private bool HandleOutputPath(string key, string value, int lineNumber, string type)
        {

            if (string.IsNullOrWhiteSpace(value))
            {
                Error($"{key} must be passed a value and can not be empty");
                return false;
            }

            int pathLength = value.Split(',')[0].Length;
            string sortPath = value.Split(',')[0].Trim('"').Trim();
            string modifedSortPath = sortPath;
            string allOptions = value.Substring(pathLength).Trim(',').Trim();

            MatchCollection matches = Regex.Matches(allOptions, @"\w+\[[^\]]+\]");

            if (string.IsNullOrWhiteSpace(sortPath))
            {
                Error($"{key} at line {lineNumber} must be passed a path and can not be empty");
                return false;
            }

            if (modifedSortPath.IndexOf("<year>") > -1)
            {
                modifedSortPath = modifedSortPath.Substring(0, modifedSortPath.IndexOf("<year>"));
            }
            if (modifedSortPath.IndexOf("<month>") > -1)
            {
                modifedSortPath = modifedSortPath.Substring(0, modifedSortPath.IndexOf("<month>"));
            }
            if (modifedSortPath.IndexOf("<day>") > -1)
            {
                modifedSortPath = modifedSortPath.Substring(0, modifedSortPath.IndexOf("<day>"));
            }

            if (!Directory.Exists(modifedSortPath))
            {
                Error($"First argument of {key} at line {lineNumber} must be a valid directory path");
                return false;
            }

            OutputPathItem outputPathItem = new(sortPath);

            if (matches.Count > 0)
            {
                foreach (Capture capture in matches)
                {
                    string captureValue = capture.Value;
                    string[] captureParts = captureValue.Split('[');

                    string argKey = captureParts[0].Trim().ToLower();
                    string[] options = captureParts[1].Split(',');

                    foreach (string option in options)
                    {
                        string cleanOption = option.Trim(']').Trim();

                        if (string.IsNullOrWhiteSpace(cleanOption))
                        {
                            Error($"Invalid option '{option}' for {key} at line {lineNumber}");
                            return false;
                        }

                        bool isAllValid = ConfigHelpers.ValidateSharedArgs(argKey, cleanOption, key, lineNumber, outputPathItem);

                        if (!isAllValid)
                        {
                            Error($"Unknown error processing option '{cleanOption}' for {key} at line {lineNumber}");
                        }
                    }
                }
            }

            _fileTools.AddOutputPath(outputPathItem, type);
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