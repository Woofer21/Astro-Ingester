using AstroIngesterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCLI
{
    internal class ConfigHelpers
    {

        public static bool ValidateSharedArgs(string argKey, string arg, string key, int lineNumber, in OutputPathItem outputPathItem)
        {
            switch (argKey)
            {
                case "extension":
                    if (!ValidateExtension(arg))
                    {
                        Error($"Invalid extension '{arg}' for {key} at line {lineNumber}. Extensions must start with a '.'");
                        return false;
                    }

                    outputPathItem.AddExtension(arg);

                    break;
                case "comment":
                    outputPathItem.AddComment(arg);

                    break;
                case "beforedate":
                case "before_date":
                    if (!ValidateDate(arg, out DateTime dateParseResult))
                    {
                        Error($"Invalid date '{arg}' for {key} at line {lineNumber}. Expected valid date format.");
                        return false;
                    }

                    outputPathItem.AddBeforeDate(dateParseResult);

                    break;
                case "afterdate":
                case "after_date":
                    if (!ValidateDate(arg, out DateTime afterDateParseResult))
                    {
                        Error($"Invalid date '{arg}' for {key} at line {lineNumber}. Expected valid date format.");
                        return false;
                    }

                    outputPathItem.AddAfterDate(afterDateParseResult);

                    break;
                case "day":
                    bool validDay = ValidateDay(arg, out int dayParseResult, out string dayErrorCode);

                    if (!validDay)
                    {
                        switch (dayErrorCode)
                        {
                            case "invalid_format":
                                Error($"Invalid day '{arg}' for {key} at line {lineNumber}. Expected a number value.");
                                break;
                            case "invalid_day":
                                Error($"Invalid day '{arg}' for {key} at line {lineNumber}. Day must be between 1 and 31.");
                                break;
                            default:
                                Error($"Unknown error validating day '{arg}' for {key} at line {lineNumber}");
                                break;
                        }

                        return false;
                    }

                    outputPathItem.AddDay(dayParseResult);

                    break;
                case "month":
                    bool validMonth = ValidateMonth(arg, out int monthParseResult, out string monthErrorCode);

                    if (!validMonth)
                    {
                        switch (monthErrorCode)
                        {
                            case "invalid_format":
                                Error($"Invalid month '{arg}' for {key} at line {lineNumber}. Expected a number value.");
                                break;
                            case "invalid_month":
                                Error($"Invalid month '{arg}' for {key} at line {lineNumber}. Month must be between 1 and 12.");
                                break;
                            default:
                                Error($"Unknown error validating month '{arg}' for {key} at line {lineNumber}");
                                break;
                        }

                        return false;
                    }

                    outputPathItem.AddMonth(monthParseResult);

                    break;
                case "year":
                    bool validYear = ValidateYear(arg, out int yearParseResult, out string yearErrorCode);

                    if (!validYear)
                    {
                        switch (yearErrorCode)
                        {
                            case "invalid_format":
                                Error($"Invalid year '{arg}' for {key} at line {lineNumber}. Expected a number value.");
                                break;
                            case "invalid_year":
                                Error($"Invalid year '{arg}' for {key} at line {lineNumber}. Year must be between 1900 and {DateTime.Now.Year}.");
                                break;
                            default:
                                Error($"Unknown error validating year '{arg}' for {key} at line {lineNumber}");
                                break;
                        }

                        return false;
                    }

                    outputPathItem.AddYear(yearParseResult);

                    break;
                default:
                    Error($"Unknown option '{argKey}' in {key} at line {lineNumber}");
                    return false;
            }

            return true;
        }

        public static bool ValidateExtension(string extension)
        {
            return !(string.IsNullOrWhiteSpace(extension) || !extension.StartsWith('.'));
        }

        public static bool ValidateDate(string date, out DateTime parsedDate)
        {
            return DateTime.TryParse(date, out parsedDate);
        }

        public static bool ValidateDay(string day, out int parsedDay, out string errorCode)
        {
            errorCode = string.Empty;

            if (int.TryParse(day, out parsedDay))
            {
                if (parsedDay < 1 || parsedDay > 31)
                {
                    errorCode = "invalid_day";
                    return false;
                }
            }
            else
            {
                errorCode = "invalid_format";
                return false;
            }

            return true;
        }

        public static bool ValidateMonth(string month, out int parsedMonth, out string errorCode)
        {
            errorCode = string.Empty;

            if (int.TryParse(month, out parsedMonth))
            {
                if (parsedMonth < 1 || parsedMonth > 12)
                {
                    errorCode = "invalid_month";
                    return false;
                }
            }
            else
            {
                errorCode = "invalid_format";
                return false;
            }

            return true;
        }

        public static bool ValidateYear(string year, out int parsedYear, out string errorCode)
        {
            errorCode = string.Empty;

            if (int.TryParse(year, out parsedYear))
            {
                if (parsedYear < 1900 || parsedYear > DateTime.Now.Year)
                {
                    errorCode = "invalid_year";
                    return false;
                }
            }
            else
            {
                errorCode = "invalid_format";
                return false;
            }

            return true;
        }


        // ConfigManager logging helper functions
        private static void Info(string message)
        {
            ConsoleHelpers.Muted($"[info(CNFG)] {message}");
        }
        private static void Warning(string message)
        {
            ConsoleHelpers.Muted($"[warning(CNFG)] {message}");
        }
        private static void Error(string message)
        {
            ConsoleHelpers.Muted($"[error(CNFG)] error({message})");
        }
    }
}
