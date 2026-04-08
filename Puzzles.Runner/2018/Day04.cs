using System.Globalization;

namespace Puzzles.Runner._2018;

[Puzzle("Repose Record", 4, 2018)]
public partial class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private const string DT_FROMAT = "yyyy-MM-dd HH:mm";
    private const int HOUR_MINS = 60;

    private enum EventType
    {
        Shift,
        FallsAsleep,
        WakesUp
    }

    private struct Log
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public EventType Type { get; set; }
    }

    private Log[] _logs = [];

    public void Init()
        => _logs = [.. input.Lines.Select(Str2Log).OrderBy(l => l.DateTime)];    

    public string SolvePart1()
    {
        var timeTable = CreateTimeTable(_logs);

        var maxSum = timeTable.MaxBy(x => x.Value.Sum());
        var idx = maxSum.Value.AsSpan().IndexOfMax();

        return (maxSum.Key * idx).ToString();
    }

    public string SolvePart2()
    {
        var timeTable = CreateTimeTable(_logs);

        var maxVal = 0;
        var maxIdx = 0;
        var guardId = 0;

        foreach (var kv in timeTable)
        {
            foreach(var (sleep, idx) in kv.Value.WithIndex())
            {
                if(sleep > maxVal)
                {
                    maxVal = sleep;
                    maxIdx = idx;
                    guardId = kv.Key;
                }
            }
        }             
        

        return (guardId * maxIdx).ToString();
    }

    private static Dictionary<int, int[]> CreateTimeTable(Log[] logs)
    {
        Dictionary<int, int[]> dict = [];

        var id = 0;
        var start = 0;

        foreach (var log in logs)
        {
            if (log.Type == EventType.Shift)
            {
                dict.TryAdd(log.Id, new int[HOUR_MINS]);
                id = log.Id;
            }
            else if (log.Type == EventType.FallsAsleep)
            {
                start = log.DateTime.Minute;
            }
            else
            {                
                for (int i = start; i < log.DateTime.Minute; i++)
                    dict[id][i]++;
            }
        }

        return dict;
    }

    private static Log Str2Log(string str)
    {
        var dtEnd= DT_FROMAT.Length + 1;
        var dt = DateTime.ParseExact(str[1..dtEnd], DT_FROMAT, CultureInfo.InvariantCulture);

        var eventStart = dtEnd + 2;
        var eventStr = str[eventStart..];
        var type = eventStr switch
        {
            "falls asleep" => EventType.FallsAsleep,
            "wakes up" => EventType.WakesUp,
            _ => EventType.Shift
        };

        int id = 0;
        if(type == EventType.Shift)
        {
            var idStart = str.IndexOf('#');
            id = 0;
            for (int i = idStart + 1; str[i] != ' '; i++)
                id = (id * 10) + (str[i] - '0');
        }

        return new() 
        { 
            DateTime = dt,
            Id = id,
            Type = type
        };
    }
}
