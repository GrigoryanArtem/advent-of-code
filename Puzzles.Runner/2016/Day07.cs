namespace Puzzles.Runner._2016;

[Puzzle("Internet Protocol Version 7", 7, 2016)]
public partial class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private record Ip
    {
        public required string[] Supernets { get; init; }
        public required string[] Hypers { get; init; }
    }

    private Ip[] _ips = [];

    public void Init()
        => _ips = [.. input.Lines.Select(str => Parse(str))];    

    public string SolvePart1()
        => _ips.Count(SupportsTls).ToString();

    public string SolvePart2()
        => _ips.Count(SupportsSsl).ToString();

    private static bool SupportsTls(Ip ip)
        => ip.Hypers.All(h => !ABBA(h)) && ip.Supernets.Any(s => ABBA(s));

    private static bool ABBA(ReadOnlySpan<char> str)
    {        
        for (int i = 0; i < str.Length - 3; i++)
        {            
            if(str[i] != str[i + 1] &&
                str[i] == str[i + 3] &&
                str[i + 1] == str[i + 2])
                return true;
        }
        
        return false;
    }

    private static bool SupportsSsl(Ip ip)
    {        
        foreach(var supernet in ip.Supernets)
            for(int i = 0; i < supernet.Length - 2; i++)
                if (supernet[i] == supernet[i + 2] && supernet[i] != supernet[i + 1] && 
                    ip.Hypers.Any(h => ContainBAB(h, supernet[i], supernet[i + 1])))
                        return true;

        return false;
    }        

    private static bool ContainBAB(string str, char a, char b)
    {
        for (int i = 0; i < str.Length - 2; i++)
            if (str[i] == b && str[i + 2] == b && str[i + 1] == a)
                return true; 

        return false;
    }

    private static Ip Parse(ReadOnlySpan<char> ip)
    {
        var hypers = new List<string>();
        var supernets = new List<string>();

        List<string>[] containers = [supernets, hypers];
        var cidx = 0;

        var start = 0;
        for (int i = 0; i < ip.Length; i++)
        {
            if (ip[i] == '[' || ip[i] == ']')
            {
                containers[cidx].Add(new string(ip[start..i]));
                cidx = (cidx + 1) % containers.Length;
                start = i + 1;
            }
        }

        containers[cidx].Add(new string(ip[start..]));

        return new()
        {
            Hypers = [.. hypers],
            Supernets = [.. supernets]
        };
    }
}