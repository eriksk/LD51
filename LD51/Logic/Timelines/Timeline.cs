using System.Numerics;
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

    public Timeline()
    {
        _metronome = new Metronome()
        {
            BPM = 60
        };
        _metronome.OnBeat += () =>
        {
            System.Console.WriteLine("1/1");
            _beat = 1f;
        };
        _metronome.OnBeat4th += (beat) =>
        {
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
        const int lines = 4;

        const float offset = 128;
        const float spacing = 64;

        for (var i = 0; i < lines; i++)
        {
            var start = new Vector2(0, offset + spacing * i);
            var end = new Vector2(1280, start.Y);

            Raylib.DrawLine(
                (int)start.X,
                (int)start.Y,
                (int)end.X,
                (int)end.Y,
                Color.MAROON
            );
        }

        var height = (spacing * lines);
        var sineYPosition = offset - (spacing * 0.5f) + (height * 0.5f) + (_metronome.Sine * height * 0.5f);
        Raylib.DrawRectangle(
            128,
            (int)sineYPosition,
            4,
            4,
            Color.MAROON
        );

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

        Raylib.DrawText("Wave Hz: " + MathF.Round(_metronome.Hz, 2), 128, 128, 32, Color.MAROON);
        Raylib.DrawText("Time: " + MathF.Round(_metronome.RealTime, 2), 128, 128 + 32, 32, Color.MAROON);
    }
}