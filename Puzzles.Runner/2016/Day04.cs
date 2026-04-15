using System.Text.RegularExpressions;

namespace Puzzles.Runner._2016;

[Puzzle("Security Through Obscurity", 4, 2016)]
public partial class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private const string STORAGE_NAME = "northpole-object-storage";

    private struct Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Checksum { get; set; }
    }

    private Room[] _rooms = [];

    public void Init()
        => _rooms = [..input.Lines.Select(StrToRoom)];    

    public string SolvePart1()
        => _rooms.Sum(GetRealRoomId).ToString();

    public string SolvePart2()
    {
        foreach (var room in _rooms)
        {
            var decrypted = Decrypt(room);

            if (decrypted.Contains(STORAGE_NAME))
                return room.Id.ToString();
        }

        return AOC.NO_ANSWER;
    }

    private static string Decrypt(Room room)
        => new([.. room.Name.Select(ch => Shift(ch, room.Id))]);    

    private static int GetRealRoomId(Room room)
    {
        var top = room.Name.GroupBy(ch => ch)
            .Where(g => Char.IsLetter(g.Key))
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Take(5)
            .Select(g => g.Key)
            .ToArray();

        var valid = top.SequenceEqual(room.Checksum);

        return valid ? room.Id : 0;
    }

    private static Room StrToRoom(string str)
    {
        var match = RoomRegex().Match(str);

        return new()
        {
            Id = Convert.ToInt32(match.Groups["id"].Value),
            Name = match.Groups["name"].Value,
            Checksum = match.Groups["checksum"].Value
        };
    }

    [GeneratedRegex(@"^(?<name>[a-z-]+)-(?<id>\d+)\[(?<checksum>[a-z]+)\]$")]
    private static partial Regex RoomRegex();

    private static char Shift(char source, int shift)
        => source >= 'a' && source <= 'z'
            ? (char)((source - 'a' + shift) % ('z' - 'a' + 1) + 'a')
            : source;
}
