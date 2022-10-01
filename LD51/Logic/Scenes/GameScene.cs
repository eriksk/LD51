using System.Numerics;
using LD51.Framework;
using Raylib_cs;

namespace LD51.Logic.Scenes;

public class GameScene : Scene
{
    private List<CellEntity> _cells;
    private Texture2D _sprites;
    private PlayerEntity _player;
    private BulletManager _bulletManager;

    public GameScene(IGame game) : base(game)
    {
    }

    public override void Load()
    {
        // TODO: load from bin, not project
        _sprites = Raylib.LoadTexture("Resources/gfx/sprites.png");
        _bulletManager = new BulletManager(_sprites);
        _cells = new List<CellEntity>();
        _cells.Add(new CellEntity(_sprites)
        {
            Radius = 8f,
            Position = new Vector2(64, 64),
            Velocity = new Vector2(Rand.Next(-1f, 1f), Rand.Next(-1f, 1f)) * 128f
        });
        _player = new PlayerEntity(_sprites, _bulletManager)
        {
            Position = new Vector2(Game.Width, Game.Height) * 0.5f,
        };
    }

    public override void Update()
    {
        var dt = Raylib.GetFrameTime();
        var bounds = new Rectangle(0, 0, 1280, 720);

        _player.Update(dt);

        for (var i = 0; i < _cells.Count; i++)
        {
            var cell = _cells[i];
            cell.Lifetime += dt;
            var directionToPlayer = (_player.Position  - cell.Position);
            const float transitionSpeed = 500f;
            cell.Velocity += Vector2.Normalize(directionToPlayer) * transitionSpeed * dt;

            const float maxSpeed = 128f;
            if (cell.Velocity.Length() > maxSpeed)
            {
                cell.Velocity = Vector2.Normalize(cell.Velocity) * maxSpeed;
            }
            cell.Update(dt);

            if (cell.Lifetime > 3f)
            {
                _cells.Add(new CellEntity(_sprites)
                {
                    Radius = 8f,
                    Position = cell.Position,
                    Velocity = new Vector2(Rand.Next(-1f, 1f), Rand.Next(-1f, 1f)) * 128f
                });
                cell.Lifetime = 0f;
            }

            if (cell.Position.X - cell.Radius < bounds.x)
            {
                cell.Position.X = bounds.x + cell.Radius;
                cell.Velocity.X *= -1f;
            }
            if (cell.Position.X + cell.Radius > bounds.x + bounds.width)
            {
                cell.Position.X = bounds.x + bounds.width - cell.Radius;
                cell.Velocity.X *= -1f;
            }

            if (cell.Position.Y - cell.Radius < bounds.y)
            {
                cell.Position.Y = bounds.y + cell.Radius;
                cell.Velocity.Y *= -1f;
            }
            if (cell.Position.Y + cell.Radius > bounds.y + bounds.height)
            {
                cell.Position.Y = bounds.y + bounds.height - cell.Radius;
                cell.Velocity.Y *= -1f;
            }

            if(_bulletManager.Collides(cell.Position, cell.Radius, out var bullet))
            {
                bullet.Kill();
                _cells.RemoveAt(i--);
                continue;
            }
        }

        _bulletManager.Update(dt);
    }

    public override void Draw()
    {
        Raylib.ClearBackground(Color.BLACK);
        foreach (var entity in _cells)
        {
            entity.Draw();
        }

        _bulletManager.Draw();

        _player.Draw();

        // Raylib.DrawRectangle(
        //     (Game.Width / 2) - 32,
        //     (Game.Height / 2) - 32,
        //     64,
        //     64,
        //     Color.MAROON
        // );


        Raylib.DrawText("Entities: " + _cells.Count, 16, 64, 32, Color.MAROON);
        // DrawCenteredText("LD51", new Vector2(Game.Width / 2f, 128f), 64, Color.MAROON);
        // DrawCenteredText("BY SKOGGY", new Vector2(Game.Width / 2f, Game.Height - 128f), 64, Color.MAROON);
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