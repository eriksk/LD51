using System.Numerics;
using LD51.Framework;
using Raylib_cs;

namespace LD51.Logic;

public class CellEntity : Entity
{
    private Vector2 _innerPosition;
    private float _innerSpeed;
    private float _innerRadius;
    private float _innerAngle;
    private readonly Texture2D _sprites;

    public CellEntity(Texture2D sprites)
    {
        _innerPosition = Vector2.Zero;
        _innerAngle = Rand.Next(0f, MathF.PI * 2f);
        _innerSpeed = Rand.Next(5f, 15f);
        this._sprites = sprites;
    }

    public override void Update(float dt)
    {
        Position += Velocity * dt;

        _innerRadius = Radius * 0.5f;
        var direction = new Vector2(MathF.Cos(_innerAngle), MathF.Sin(_innerAngle));
        _innerPosition += direction * _innerSpeed * dt;
        const float margin = 4f;
        if (_innerPosition.Length() + _innerRadius >= Radius - margin)
        {
            _innerSpeed = Rand.Next(5f, 15f);
            var angleChange = MathF.PI * 0.5f;
            _innerAngle += MathF.PI + Rand.Next(-angleChange, angleChange);
        }
    }

    public override void Draw()
    {
        var source = new Rectangle(0, 0, 16, 16);
        var dest = new Rectangle(
            (int)(Position.X - Radius),
            (int)(Position.Y - Radius),
            Radius * 2,
            Radius * 2
        );

        Raylib.DrawTexturePro(
            _sprites,
            source,
            dest,
            Vector2.Zero,
            0f,
            Color.WHITE
        );

        source = new Rectangle(16, 0, 16, 16);
        dest = new Rectangle(
            (int)(Position.X + _innerPosition.X - Radius),
            (int)(Position.Y + _innerPosition.Y - Radius),
            Radius * 2,
            Radius * 2
        );

        Raylib.DrawTexturePro(
            _sprites,
            source,
            dest,
            Vector2.Zero,
            0f,
            Color.WHITE
        );
        // Raylib.DrawCircle((int)Position.X, (int)Position.Y, Radius, Color.BLUE);
        // Raylib.DrawCircle(
        //     (int)(Position.X + _innerPosition.X),
        //     (int)(Position.Y + _innerPosition.Y),
        //     _innerRadius,
        //     Color.GREEN);
    }
}