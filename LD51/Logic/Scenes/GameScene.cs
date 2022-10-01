using LD51.Framework;
using LD51.Logic.Instruments;
using LD51.Logic.Timelines;
using Raylib_cs;

namespace LD51.Logic.Scenes;

public class GameScene : Scene
{
    private Timeline _timeline;
    private Flute _flute;

    public GameScene(IGame game) : base(game)
    {
    }

    public override void Load()
    {
        _flute = new Flute();
        _timeline = new Timeline();
    }

    public override void Update()
    {
        var dt = Raylib.GetFrameTime();

        _timeline.Update(dt);
        _flute.Update();
    }

    public override void Draw()
    {
        Raylib.DrawText("BLOCKFLÖJT SIMULATOR", 16, 16, 64, Color.MAROON);

        _timeline.Draw();
        _flute.Draw();
    }
}