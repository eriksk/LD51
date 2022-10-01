using LD51.Framework;
using LD51.Logic.Scenes;
using Raylib_cs;

namespace LD51;

public class Game : IGame
{
    private readonly string _title;

    private IScene _scene;

    public int Width => Raylib.GetScreenWidth();
    public int Height => Raylib.GetScreenHeight();

    public Game(int width, int height, int targetFps, string title)
    {
        _title = title;
        Initialize(width, height, targetFps);
    }

    public void Run()
    {
        Load();
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Raylib.BeginDrawing();
            Draw();
            Raylib.EndDrawing();
        }
    }

    private void Initialize(int width, int height, int targetFps)
    {
        Raylib.InitWindow(width, height, _title);
        Raylib.SetTargetFPS(targetFps);
        Raylib.SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        Raylib.HideCursor();
        Raylib.InitAudioDevice();
    }

    private void Load()
    {
        _scene = new GameScene(this);
        _scene.Load();
    }

    public void Close()
    {
        Raylib.CloseWindow();
    }

    private void Update()
    {
        _scene.Update();
    }

    private void Draw()
    {
        Raylib.ClearBackground(Color.RAYWHITE);
        _scene.Draw();

        Raylib.DrawText("FPS: " + Raylib.GetFPS(), 16, Height - 32, 16, Color.MAROON);
    }
}