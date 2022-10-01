using System.Numerics;
using LD51.Logic.Songs;
using Raylib_cs;

namespace LD51.Logic.Timelines;

public class Metronome
{
    public int BPM;
    private float _realTime;
    private float _beatTime;
    private float _beatTime4th;

    public event Action OnBeat;
    public event Action<int> OnBeat4th;

    public float Sine { get; private set; }
    public float Sine4th { get; private set; }
    public float Hz => BPM / 60f;
    public float RealTime => _realTime;

    private Sound _tickSound;
    private Sound _tickSecondarySound;

    public Metronome()
    {
        Sine = 0f;
        Sine4th = 0f;
        _tickSound = Raylib.LoadSound("Resources/sounds/metronome_secondary.wav");
        _tickSecondarySound = Raylib.LoadSound("Resources/sounds/metronome.wav");
        OnBeat += () =>
        {
            Raylib.PlaySound(_tickSound);
        };
        OnBeat4th += (tick) =>
        {
            if (tick == 1) return;
            Raylib.PlaySound(_tickSecondarySound);
        };
    }

    public float GetBarsTime(int bars)
    {
        // beat time = time passed * hz
        // To time for bars should be one Hz per bar? I don't know this math...
        return bars * Hz;
    }

    public void Update(float dt)
    {
        _realTime += dt;
        var oldBeatTime = _beatTime;
        _beatTime = _realTime * Hz;
        _beatTime4th += Hz * 4f * dt;

        var oldSine = Sine;
        Sine = MathF.Sin(_beatTime * MathF.PI);
        var oldSine4th = Sine4th;
        Sine4th = MathF.Sin(_beatTime4th * MathF.PI);

        var beat = 0;
        if ((oldSine < 0f && Sine >= 0f))
        {
            OnBeat?.Invoke();
            beat = 1;
        }
        if ((oldSine4th < 0f && Sine4th >= 0f))
        {
            OnBeat4th?.Invoke(beat);
        }
    }
}

public class Timeline
{
    private Metronome _metronome;
    private float _beat;
    private float _beat4th;
    private Song _song;
    private int _beatNumber;

    const int lines = 4;
    const float offset = 128;
    const float spacing = 64;

    public Timeline()
    {
        _song = SongModel.Load("Resources/songs/song_1.json").CreateSong();
        _metronome = new Metronome()
        {
            BPM = _song.BPM
        };
        _metronome.OnBeat += () =>
        {
            System.Console.WriteLine("1/1");
            _beat = 1f;
        };
        _metronome.OnBeat4th += (beat) =>
        {
            _beatNumber++;
            if (_beatNumber > _song.Notes.Length - 1)
            {
                _beatNumber = 0;
            }
            System.Console.WriteLine("1/4");
            _beat4th = 1f;
        };
    }

    public void Update(float dt)
    {
        _metronome.Update(dt);
        _beat -= 5f * dt;
        if (_beat < .1f)
            _beat = .1f;

        _beat4th -= 5f * dt;
        if (_beat4th < .1f)
            _beat4th = .1f;
    }

    public void Draw()
    {

        var currentNote = 0;

        if (_beatNumber > -1 && _beat < _song.Notes.Length - 1)
        {
            switch (_song.Notes[_beatNumber])
            {
                case "c1": currentNote = 1; break;
                case "d1": currentNote = 1 | 2; break;
                case "e1": currentNote = 2; break;
                case "f1": currentNote = 2 | 4; break;
                case "g1": currentNote = 4; break;
                case "a1": currentNote = 4 | 8; break;
                case "b1": currentNote = 8; break;
                case "c2": currentNote = 1 | 2 | 4 | 8; break;
            }
        }

        var lookup = new Dictionary<int, int>()
        {
            { 1, 1 },
            { 2, 2 },
            { 3, 4 },
            { 4, 8 },
        };

        for (var i = 0; i < lines; i++)
        {
            var start = new Vector2(0, offset + spacing * i);
            var end = new Vector2(1280, start.Y);

            var number = lookup[i + 1];
            var isCurrent = (currentNote & number) == number;

            Raylib.DrawLineEx(
                start,
                end,
                2f,
                isCurrent ? Color.GREEN : Color.MAROON
            );
        }

        DrawBars();

        var height = (spacing * lines);
        var sineYPosition = offset - (spacing * 0.5f) + (height * 0.5f) + (_metronome.Sine * height * 0.5f);
        Raylib.DrawRectangle(
            128,
            (int)sineYPosition,
            4,
            4,
            Color.MAROON
        );

        var debugPosition = new Vector2(16, 512);

        if (_beat > 0f)
        {
            var h = (int)(128 * _beat);

            Raylib.DrawRectangle(
                1280 / 2,
                (720 - 128) - h,
                64,
                h,
                Color.MAROON
            );
        }

        if (_beat4th > 0f)
        {
            var h = (int)(128 * _beat4th);

            Raylib.DrawRectangle(
                (1280 / 2) + 78,
                (720 - 128) - h,
                64,
                h,
                Color.MAROON
            );
        }

        Raylib.DrawText("Wave Hz: " + MathF.Round(_metronome.Hz, 2), (int)debugPosition.X, (int)debugPosition.Y, 32, Color.MAROON);
        Raylib.DrawText("Time: " + MathF.Round(_metronome.RealTime, 2), (int)debugPosition.X, (int)debugPosition.Y + 32, 32, Color.MAROON);
        Raylib.DrawText("BeatNumber: " + _beatNumber, (int)debugPosition.X, (int)debugPosition.Y + 64, 32, Color.MAROON);
    }

    private void DrawBars()
    {
        const float basis = 64f;

        var barTime = _metronome.GetBarsTime(1);

        var bars = 8;
        var maxDistance = 1600 - basis;

        var oneBarDistance = maxDistance / (float)bars;
        var beatDistance = oneBarDistance;

        var xOffset = 0f;

        for (var i = 0; i < bars; i++)
        {
            var height = (spacing * lines);

            var start = new Vector2(basis + xOffset + beatDistance * i, offset - spacing * 0.5f);
            var end = new Vector2(basis + xOffset + beatDistance * i, start.Y + height);

            Raylib.DrawLine(
                (int)start.X,
                (int)start.Y,
                (int)end.X,
                (int)end.Y,
                Color.MAROON);
        }
    }
}