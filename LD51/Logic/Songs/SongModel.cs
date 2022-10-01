using System.Text.Json;

namespace LD51.Logic.Songs;

public class SongModel
{
    public int BPM { get; set; }
    public string[] Notes { get; set; }

    public static SongModel Load(string path) => JsonSerializer.Deserialize<SongModel>(File.ReadAllText(path), new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    });

    public Song CreateSong()
    {
        var notes = new List<string>();

        foreach (var bar in Notes)
        {
            var segment = bar;
            while (segment.Any())
            {
                if (segment[0] == '-')
                {
                    notes.Add(string.Empty);
                    segment = segment.Substring(1);
                    continue;
                }
                notes.Add(segment.Substring(0, 2));
                segment = segment.Substring(2);
            }
        }

        return new Song()
        {
            BPM = BPM,
            Notes = notes.ToArray()
        };
    }
}

public class Song
{
    public int BPM { get; set; }
    public string[] Notes { get; set; }
}