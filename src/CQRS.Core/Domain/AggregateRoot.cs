using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    public Guid Id => _id;

    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

    public void MarkChangesAsCommitted() => _changes.Clear();

    private void ApplyChange(BaseEvent @event, bool isNew) 
    {
        var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
        if (method == null)
            throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {@event.GetType().Name}");

        method.Invoke(this, new object[] { @event });

        if (isNew)
            _changes.Add(@event);
    }

    protected void RaiseEvent(BaseEvent @event) => ApplyChange(@event, true);

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events) 
        {
            ApplyChange(@event, false);
        }
    }
}

/*
* Beneficios do AggregateRoot
* Ele gerencia qual método apply é invocado no Aggregate concreto com base no tipo de evento.
* Confirma as alterações que foram aplicadas ao Aggregate.
* A Aggregate Root é a entidade dentro do agregado que é responsável por sempre mantê-lo em um estado consistente.
* A Aggregate Root mantém a lista de alterações não confirmadas na forma de eventos, que precisam ser aplicadas ao agregado e persistidas no armazenamento de eventos.

*/
