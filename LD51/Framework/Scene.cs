namespace LD51.Framework;

public interface IScene
{
    void Load();
    void Update();
    void Draw();
}

public abstract class Scene : IScene
{
    protected IGame Game { get; }

    public Scene(IGame game)
    {
        Game = game ?? throw new ArgumentNullException(nameof(game));
    }

    public abstract void Load();
    public abstract void Update();
    public abstract void Draw();
}
