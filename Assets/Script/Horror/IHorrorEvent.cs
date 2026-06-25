public interface IHorrorEvent
{
    string EventId { get; }
    bool IsActive { get; }
    void Initialize(HorrorEventConfig config);
    void Trigger(float intensity);
    void Tick(float deltaTime);
    void Stop();
}
