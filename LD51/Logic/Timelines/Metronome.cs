using Raylib_cs;

namespace LD51.Logic.Timelines;

public class Metronome
{
    private int _bpm;
    public int BPM
    {
        get => _bpm;
        set
        {
            _bpm = value;
            RecreateBeats();
        }
    }

    private Beat _beat1;
    private Beat _beat4;

    public Beat Beat1 => _beat1;
    public Beat Beat4 => _beat4;

    public event Action<int> OnBeat;
    public float BeatsPerSecond => BPM / 60f;
    private Sound _tickSound;
    private Sound _tickSecondarySound;

    public Metronome()
    {
        BPM = 120;
        _tickSound = Raylib.LoadSound("Resources/sounds/metronome_secondary.wav");
        _tickSecondarySound = Raylib.LoadSound("Resources/sounds/metronome.wav");
    }

    private void RecreateBeats()
    {
        _beat1 = new Beat(BeatsPerSecond, 1);
        _beat4 = new Beat(BeatsPerSecond / 4f, 4);
        _beat1.OnBeat += (b) =>
        {
            Raylib.PlaySound(_tickSound);
        };
        _beat4.OnBeat += (b) =>
        {
            Raylib.PlaySound(_tickSecondarySound);
            OnBeat?.Invoke(b);
        };
    }

    public void Update(float dt)
    {
        _beat1.Update(dt);
        _beat4.Update(dt);
    }
}
