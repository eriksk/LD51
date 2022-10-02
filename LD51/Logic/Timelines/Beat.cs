namespace LD51.Logic.Timelines;

public class Beat
{
    private readonly float _interval;
    private readonly int _count;

    private float _current;
    private int _counter;

    public event Action<int> OnBeat;

    public float Progress => _current / _interval;
    public float TimePerBeat => _interval;

    public Beat(float interval, int count)
    {
        _interval = interval;
        _count = count;
        _current = interval;
        _counter = 0;
    }

    public void Update(float dt)
    {
        _current += dt;
        if (_current >= _interval)
        {
            _current -= _interval;
            OnBeat?.Invoke(_counter++);
            if (_counter > _count)
            {
                _counter = 0;
            }
        }
    }
}
