using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class OutputPathItem
    {
        public string Path { get; set; }
        public List<string> Extentions { get; set; } = [];
        public List<string> Comments { get; set; } = [];

        private readonly List<string> _extensions = [];
        private readonly List<string> _comments = [];
        private readonly List<DateTime> _beforeDates = [];
        private readonly List<DateTime> _afterDates = [];
        private readonly List<int> _days = [];
        private readonly List<int> _months = [];
        private readonly List<int> _years = [];

        public IReadOnlyList<string> Extensions => _extensions.AsReadOnly();
        public IReadOnlyList<string> Comments2 => _comments.AsReadOnly();
        public IReadOnlyList<DateTime> BeforeDates => _beforeDates.AsReadOnly();
        public IReadOnlyList<DateTime> AfterDates => _afterDates.AsReadOnly();
        public IReadOnlyList<int> Days => _days.AsReadOnly();
        public IReadOnlyList<int> Months => _months.AsReadOnly();
        public IReadOnlyList<int> Years => _years.AsReadOnly();

        public bool HasExtentions => _extensions.Count > 0;
        public bool HasComments => _comments.Count > 0;
        public bool HasBeforeDates => _beforeDates.Count > 0;
        public bool HasAfterDates => _afterDates.Count > 0;
        public bool HasDays => _days.Count > 0;
        public bool HasMonths => _months.Count > 0;
        public bool HasYears => _years.Count > 0;

        public OutputPathItem(string path)
        {
            Path = path;
        }

        public OutputPathItem(string path, List<string> extentions, List<string> comments)
        {
            Path = path;
            Extentions = extentions;
            Comments = comments;
        }

        public void AddExtension(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension) || !extension.StartsWith('.'))
            {
                throw new ArgumentException("Extension must start with a dot (.) and cannot be empty.", nameof(extension));
            }
            _extensions.Add(extension);
        }
        public void AddExtension(IEnumerable<string> extensions)
        {
            if (extensions.Any(ext => string.IsNullOrWhiteSpace(ext) || !ext.StartsWith('.')))
            {
                throw new ArgumentException("All extensions must start with a dot (.) and cannot be empty.", nameof(extensions));
            }
            _extensions.AddRange(extensions);
        }

        public void AddComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Comment cannot be empty.", nameof(comment));
            }
            _comments.Add(comment);
        }
        public void AddComment(IEnumerable<string> comments)
        {
            if (comments.Any(comment => string.IsNullOrWhiteSpace(comment)))
            {
                throw new ArgumentException("All comments cannot be empty.", nameof(comments));
            }
            _comments.AddRange(comments);
        }

        public void AddBeforeDate(DateTime date)
        {
            _beforeDates.Add(date);
        }
        public void AddBeforeDate(IEnumerable<DateTime> dates)
        {
            _beforeDates.AddRange(dates);
        }

        public void AddAfterDate(DateTime date)
        {
            _afterDates.Add(date);
        }
        public void AddAfterDate(IEnumerable<DateTime> dates)
        {
            _afterDates.AddRange(dates);
        }

        public void AddDay(int day)
        {
            if (day < 1 || day > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(day), "Day must be between 1 and 31.");
            }
            _days.Add(day);
        }
        public void AddDay(IEnumerable<int> days)
        {
            if (days.Any(d => d < 1 || d > 31))
            {
                throw new ArgumentOutOfRangeException(nameof(days), "All days must be between 1 and 31.");
            }
            _days.AddRange(days);
        }

        public void AddMonth(int month)
        {
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
            }
            _months.Add(month);
        }
        public void AddMonth(IEnumerable<int> months)
        {
            if (months.Any(m => m < 1 || m > 12))
            {
                throw new ArgumentOutOfRangeException(nameof(months), "All months must be between 1 and 12.");
            }
            _months.AddRange(months);
        }

        public void AddYear(int year)
        {
            if (year < 1900 || year > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(year), $"Year must be between 1900 and {DateTime.Now.Year}.");
            }
            _years.Add(year);
        }
        public void AddYear(IEnumerable<int> years)
        {
            if (years.Any(y => y < 1900 || y > DateTime.Now.Year))
            {
                throw new ArgumentOutOfRangeException(nameof(years), $"All years must be between 1900 and {DateTime.Now.Year}.");
            }
            _years.AddRange(years);
        }
    }
}
