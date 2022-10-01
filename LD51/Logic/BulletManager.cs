using System.Numerics;
using LD51.Framework;
using Raylib_cs;

namespace LD51.Logic;

public class BulletManager
{
    const float SPEED = 1024f;
    const float MAX_LIFETIME = 3f;
    const float HIT_RADIUS = 8f;

    private readonly Texture2D _sprites;
    private Pool<Bullet> _bullets;
    private Rectangle _source;

    public BulletManager(Texture2D sprites)
    {
        _bullets = new Pool<Bullet>(256);
        _sprites = sprites;
        _source = new Rectangle(16 * 3, 0, 16, 16);
    }

    public bool TryFire(Vector2 position, Vector2 direction)
    {
        var b = _bullets.Pop();
        if (b == null) return false;

        if (direction == Vector2.Zero) direction = new Vector2(1f, 0f);

        b.Position = position;
        b.Velocity = Vector2.Normalize(direction) * SPEED;
        b.Rotation = MathF.Atan2(direction.Y, direction.X);
        b.Alive = true;
        b.Lifetime = 0f;

        return true;
    }

    public bool Collides(Vector2 position, float radius, out Bullet bullet)
    {
        for (var i = 0; i < _bullets.Count; i++)
        {
            var b = _bullets[i];

            var distance = Vector2.Distance(position, b.Position);
            if (distance > radius + HIT_RADIUS) continue;

            bullet = b;
            return true;
        }

        bullet = null;
        return false;
    }

    public void Update(float dt)
    {
        for (var i = 0; i < _bullets.Count; i++)
        {
            var b = _bullets[i];
            b.Position += b.Velocity * dt;

            if (b.Lifetime > MAX_LIFETIME || !b.Alive)
            {
                _bullets.Push(i--);
                continue;
            }

            b.Lifetime += dt;
        }

    }

    public void Draw()
    {
        for (var i = 0; i < _bullets.Count; i++)
        {
            var b = _bullets[i];

            var dest = new Rectangle(
                (int)(b.Position.X - 8),
                (int)(b.Position.Y - 8),
                16,
                16
            );

            Raylib.DrawTexturePro(
                _sprites,
                _source,
                dest,
                new Vector2(8, 8),
                Raylib.RAD2DEG * b.Rotation,
                Color.WHITE
            );
        }
    }
}

public class Bullet
{
    public Vector2 Position, Velocity;
    public float Rotation;
    public float Lifetime;
    public bool Alive;

    public void Kill()
    {
        Alive = false;
    }
}
