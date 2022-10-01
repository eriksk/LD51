using System.Numerics;
using LD51.Framework;
using Raylib_cs;

namespace LD51.Logic;

public class PlayerEntity : Entity
{
    public float Rotation;
    private readonly Texture2D _sprites;
    private readonly BulletManager _bulletManager;
    private int _fireSources;
    private float _fireInterval = 0.2f;
    private float _fireCurrent;

    public PlayerEntity(Texture2D sprites, BulletManager bulletManager)
    {
        _sprites = sprites;
        _bulletManager = bulletManager ?? throw new ArgumentNullException(nameof(bulletManager));
        _fireSources = 1;
        _fireInterval = 1f;
    }

    public override void Update(float dt)
    {
        _fireCurrent += dt;
        var input = new Vector2();

        if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT) || Raylib.IsKeyDown(KeyboardKey.KEY_A)) input.X -= 1f;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT) || Raylib.IsKeyDown(KeyboardKey.KEY_D)) input.X += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_UP) || Raylib.IsKeyDown(KeyboardKey.KEY_W)) input.Y -= 1f;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN) || Raylib.IsKeyDown(KeyboardKey.KEY_S)) input.Y += 1f;

        var fire = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);

        if (input.Length() > 0.1f)
        {
            var len = input.Length();
            input = Vector2.Normalize(input) * MathExt.Clamp01(len);
        }

        const float speed = 1048f;
        const float damping = 5f;

        var mousePosition = Raylib.GetMousePosition();

        var aimDirection = Vector2.Normalize((mousePosition - Position));
        var aimAngle = MathF.Atan2(aimDirection.Y, aimDirection.X);

        if (!float.IsNaN(aimAngle))
        {
            Rotation = aimAngle;
        }

        if (fire && _fireCurrent >= _fireInterval)
        {
            const float distanceBetweenGuns = 8;
            for (var i = 0; i < _fireSources; i++)
            {
                var direction = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));
                var sideAngle = Rotation + Raylib.DEG2RAD * 90f;
                var sideDirection = new Vector2(MathF.Cos(sideAngle), MathF.Sin(sideAngle));
                var halfI = _fireSources / 2;
                var offset = sideDirection * distanceBetweenGuns * (-halfI + i);
                _bulletManager.TryFire(Position + offset, direction);
            }
            _fireCurrent = 0f;
        }

        Velocity += input * speed * dt;
        Velocity += -Velocity * damping * dt;
        Position += Velocity * dt;
    }

    public override void Draw()
    {
        var source = new Rectangle(16 * 2, 0, 16, 16);
        var dest = new Rectangle(
            (int)(Position.X - 8),
            (int)(Position.Y - 8),
            16,
            16
        );

        Raylib.DrawTexturePro(
            _sprites,
            source,
            dest,
            new Vector2(8, 8),
            Raylib.RAD2DEG * Rotation,
            Color.WHITE
        );


        var mousePosition = Raylib.GetMousePosition();

        Raylib.DrawCircleLines((int)mousePosition.X, (int)mousePosition.Y, 4, Color.WHITE);
        Raylib.DrawCircle((int)mousePosition.X, (int)mousePosition.Y, 1, Color.WHITE);
    }

}
