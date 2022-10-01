using Raylib_cs;

namespace LD51;

public class Game
{
    private readonly int _width;
    private readonly int _height;
    private readonly string _title;

    public Game(int width, int height, string title)
    {
        _width = width;
        _height = height;
        _title = title;
    }

    public void Run()
    {
        Initialize();
        Load();
        while (!Raylib.WindowShouldClose())
        {
            Update();
            Raylib.BeginDrawing();
            Draw();
            Raylib.EndDrawing();
        }
    }

    private void Initialize()
    {
        Raylib.InitWindow(_width, _height, _title);
        Raylib.SetTargetFPS(120);
    }

    private void Load()
    {
    }

    private void Update()
    {
    }

    private void Draw()
    {
        Raylib.ClearBackground(Color.RAYWHITE);
    }
}