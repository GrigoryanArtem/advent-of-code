using Puzzles.Base;

namespace Puzzles.Web;

public class PuzzleLoader
{
    private readonly HttpClient _client;

    public PuzzleLoader(string path)
    {
        if (!File.Exists(path))
            throw new PuzzlesException("Session token file not found.");

        var sessionToken = File.ReadAllText(path);
        _client = new() { BaseAddress = new(@"https://adventofcode.com/") };
        _client.DefaultRequestHeaders.Add("Cookie", sessionToken);
    }
    
    public async Task<string> GetInput(int year, int day, CancellationToken cancellationToken)
    {
        var response = await _client.GetAsync($"{year}/day/{day}/input", cancellationToken);
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
