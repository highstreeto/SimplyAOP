using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimplyAOP.IoCExample.Services
{
    public class StatisticsService : AspectWeaver.Class
    {
        public StatisticsService(AspectConfiguration config)
            : base(config) {
        }

        public StringStats Process(IEnumerable<string> strings_) {
            return Advice(strings_, strings => {
                return new StringStats(
                    maxLength: strings.Max(s => s.Length),
                    minLength: strings.Min(s => s.Length),
                    avgLength: strings.Average(s => s.Length),
                    wordOccurrences: strings
                        .SelectMany(s => s.Split(' '))
                        .Select(s => s.Trim())
                        .GroupBy(w => w)
                        .ToDictionary(g => g.Key, g => g.Count())
                );
            });
        }
    }

    public class StringStats
    {
        public StringStats(int maxLength, int minLength, double avgLength, IReadOnlyDictionary<string, int> wordOccurrences) {
            MaxLength = maxLength;
            MinLength = minLength;
            AvgLength = avgLength;
            WordOccurrences = wordOccurrences;
        }

        public int MaxLength { get; }
        public int MinLength { get; }
        public double AvgLength { get; }
        public IReadOnlyDictionary<string, int> WordOccurrences { get; }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine($"Length min: {MinLength} max: {MaxLength} avg: {AvgLength:F2}");
            foreach (var entry in WordOccurrences.OrderByDescending(kv => kv.Value)) {
                sb.AppendLine($"- {entry.Key} ({entry.Value})");
            }
            return sb.ToString();
        }
    }
}
