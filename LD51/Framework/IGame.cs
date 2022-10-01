namespace LD51.Framework;

public interface IGame
{
    int Width { get; }
    int Height { get; }
    void Close();
}
