using System.Numerics;

namespace LD51.Logic;

public class Entity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Radius;
    public float Lifetime;

    public virtual void Update(float dt)
    {
    }

    public virtual void Draw()
    {
    }
}
