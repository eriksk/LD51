using System.Numerics;
using Raylib_cs;

namespace LD51.Logic.Instruments;

public class Flute
{
    private readonly Dictionary<string, KeyboardKey> Keys = new Dictionary<string, KeyboardKey>()
    {
        { "A", KeyboardKey.KEY_A },
        { "S", KeyboardKey.KEY_S },
        { "D", KeyboardKey.KEY_D },
        { "F", KeyboardKey.KEY_F }
    };

    private readonly Dictionary<KeyboardKey, bool> _pressed;

    private Dictionary<string, Sound> _sounds;

    public Flute()
    {
        _pressed = Keys.Select(x => x.Value).ToDictionary(x => x, x => false);
        _sounds = new Dictionary<string, Sound>()
        {
            { "c1", Raylib.LoadSound("Resources/sounds/flute_c1.wav") },
            { "d1", Raylib.LoadSound("Resources/sounds/flute_d1.wav") },
            { "e1", Raylib.LoadSound("Resources/sounds/flute_e1.wav") },
            { "f1", Raylib.LoadSound("Resources/sounds/flute_f1.wav") },
            { "g1", Raylib.LoadSound("Resources/sounds/flute_g1.wav") },
            { "a1", Raylib.LoadSound("Resources/sounds/flute_a1.wav") },
            { "b1", Raylib.LoadSound("Resources/sounds/flute_b1.wav") },
            { "c2", Raylib.LoadSound("Resources/sounds/flute_c2.wav") },
        };
    }

    public void Update()
    {
        var dt = Raylib.GetFrameTime();

        foreach (var key in Keys)
            _pressed[key.Value] = Raylib.IsKeyDown(key.Value);

        var tone = GetTone();

        if (tone == null)
        {
            foreach (var sound in _sounds)
            {
                if (Raylib.IsSoundPlaying(sound.Value))
                    Raylib.PauseSound(sound.Value);
            }
        }
        else
        {
            foreach (var sound in _sounds)
            {
                if (sound.Key == tone) continue;
                if (Raylib.IsSoundPlaying(sound.Value))
                    Raylib.PauseSound(sound.Value);
            }

            if (!Raylib.IsSoundPlaying(_sounds[tone]))
            {
                Raylib.PlaySound(_sounds[tone]);
            }
        }
    }

    private string GetTone()
    {
        if (_pressed[KeyboardKey.KEY_A] && _pressed[KeyboardKey.KEY_S] && _pressed[KeyboardKey.KEY_D] && _pressed[KeyboardKey.KEY_F]) 
            return "c2";

        if (_pressed[KeyboardKey.KEY_A] && _pressed[KeyboardKey.KEY_S]) return "d1";
        if (_pressed[KeyboardKey.KEY_S] && _pressed[KeyboardKey.KEY_D]) return "f1";
        if (_pressed[KeyboardKey.KEY_D] && _pressed[KeyboardKey.KEY_F]) return "a1";
 
        if (_pressed[KeyboardKey.KEY_A]) return "c1";
        if (_pressed[KeyboardKey.KEY_S]) return "e1";
        if (_pressed[KeyboardKey.KEY_D]) return "g1";
        if (_pressed[KeyboardKey.KEY_F]) return "b1";

        return null;
    }

    public void Draw()
    {
        const float offset = 128;
        const float spacing = 64;

        var i = 0;
        foreach (var key in Keys)
        {
            var pressed = _pressed[key.Value];

            var center = new Vector2(64, offset + spacing * i);

            const int radius = 24;

            Raylib.DrawCircle(
                (int)center.X,
                (int)center.Y,
                radius,
                new Color(0, 0, 0, 200)
            );

            DrawCenteredText(key.Key, center, pressed ? 38 : 32, pressed ? Color.MAROON : Color.RAYWHITE);
            i++;
        }
    }

    private void DrawCenteredText(string text, Vector2 position, int fontSize, Color color)
    {
        var width = Raylib.MeasureText(text, fontSize);

        Raylib.DrawText(
            text,
            (int)(position.X - width / 2),
            (int)(position.Y - fontSize / 2),
            fontSize,
            color);
    }
}