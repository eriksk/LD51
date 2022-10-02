using System.Numerics;
using LD51.Logic.Songs;
using Raylib_cs;

namespace LD51.Logic.Timelines;

public class Timeline
{
    private Metronome _metronome;
    private Song _song;
    private int _beatNumber;

    const int Lines = 4;
    const float Offset = 128;
    const float Spacing = 64;
    const float TimelineDuration = 10f;
    const float TimelineWidth = 1600;
    const float Basis = 64f;

    Dictionary<int, int> LineToNoteBitsLookup = new Dictionary<int, int>()
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 4 },
        { 4, 8 },
    };

    public Timeline()
    {
        _song = SongModel.Load("Resources/songs/song_1.json").CreateSong();
        _metronome = new Metronome()
        {
            BPM = _song.BPM
        };
        _metronome.OnBeat += (b) =>
        {
            _beatNumber++;
            if (_beatNumber > _song.Notes.Length - 1)
            {
                _beatNumber = 0;
            }
        };
    }

    public void Update(float dt)
    {
        _metronome.Update(dt);
    }

    public void Draw()
    {
        var currentNote = 0;

        if (_beatNumber > -1 && _beatNumber < _song.Notes.Length - 1)
        {
            currentNote = GetNoteBits(_song.Notes[_beatNumber]);
        }

        // Highlight current note
        for (var i = 0; i < Lines; i++)
        {
            var start = new Vector2(0, Offset + Spacing * i);
            var end = new Vector2(1280, start.Y);

            var number = LineToNoteBitsLookup[i + 1];
            var isCurrent = (currentNote & number) == number;

            Raylib.DrawLineEx(
                start,
                end,
                2f,
                isCurrent ? Color.GREEN : Color.MAROON
            );
        }

        DrawBars(_metronome.Beat4, Color.GRAY, 1);
        DrawBars(_metronome.Beat1, Color.MAROON, 2);

        DrawNotes((int)(TimelineDuration / _metronome.Beat4.TimePerBeat));
    }

    private int GetNoteBits(string note)
    {
        switch (note)
        {
            case "c1": return 1;
            case "d1": return 1 | 2;
            case "e1": return 2;
            case "f1": return 2 | 4;
            case "g1": return 4;
            case "a1": return 4 | 8;
            case "b1": return 8;
            case "c2": return 1 | 2 | 4 | 8;
        }

        return 0;
    }

    private void DrawNotes(int bars)
    {
        var currentIndex = _beatNumber;

        for (var barIndex = 0; barIndex < bars; barIndex++)
        {
            var note = _song.Notes[currentIndex];
            var bits = GetNoteBits(note);

            for (var lineIndex = 0; lineIndex < Lines; lineIndex++)
            {
                var number = LineToNoteBitsLookup[lineIndex + 1];
                var isCurrent = (bits & number) == number;
                if (isCurrent)
                {
                    DrawOnTimeline(_metronome.Beat4, lineIndex, barIndex + 1, Color.MAROON);
                }
            }

            currentIndex++;
            if (currentIndex > _song.Notes.Length - 1)
            {
                currentIndex = 0;
            }
        }
    }

    private void DrawOnTimeline(Beat beat, int line, int barsAhead, Color color)
    {
        var position = GetPositionOnLine(beat, line, barsAhead);

        Raylib.DrawCircle(
            (int)position.X,
            (int)position.Y,
            16,
            color
        );
    }

    private Vector2 GetPositionOnLine(Beat beat, int line, int barsAhead)
    {
        var bars = TimelineDuration / beat.TimePerBeat;
        var barSegmentWidth = TimelineWidth / (float)bars;
        var fullWidth = barSegmentWidth * barsAhead;
        var offset = beat.Progress * -barSegmentWidth;

        var x = Basis + offset + fullWidth;
        var y = Offset - Spacing * 0.5f + (Spacing * line) + Spacing * 0.5f;

        return new Vector2(x, y);
    }

    private void DrawBars(Beat beat, Color color, int lineThickness)
    {
        var bars = TimelineDuration / beat.TimePerBeat;
        var barSegmentWidth = TimelineWidth / (float)bars;
        var xOffset = beat.Progress * -barSegmentWidth;

        for (var i = 0; i < bars; i++)
        {
            var height = (Spacing * Lines);

            var start = new Vector2(Basis + xOffset + barSegmentWidth * i, Offset - Spacing * 0.5f);
            var end = new Vector2(Basis + xOffset + barSegmentWidth * i, start.Y + height);

            Raylib.DrawLineEx(
                start,
                end,
                lineThickness,
                color);
        }
    }
}