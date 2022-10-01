namespace LD51.Framework;

public class Pool<T> where T : class, new()
{
    private readonly T[] _items;
    private int _capacity;
    private int _count;

    public Pool(int capacity)
    {
        _capacity = capacity;
        _items = new T[capacity];
        for (var i = 0; i < _items.Length; i++)
            _items[i] = new T();
    }

    public int Count => _count;

    public T this[int index]
    {
        get
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            if (index > _count - 1) throw new ArgumentOutOfRangeException(nameof(index));
            return _items[index];
        }
    }

    public T Pop()
    {
        if (_count > _items.Length - 1) return null;
        return _items[_count++];
    }

    public void Push(int index)
    {
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
        if (index > _count - 1) throw new ArgumentOutOfRangeException(nameof(index));

        var temp = _items[_count - 1];
        _items[_count - 1] = _items[index];
        _items[index] = temp;
        _count--;
    }
}