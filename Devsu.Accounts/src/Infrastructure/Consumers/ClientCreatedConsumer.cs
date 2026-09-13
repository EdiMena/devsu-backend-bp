using Application.Abstractions;
using Contracts;
using Domain;
using MassTransit;

namespace Infrastructure.Consumers;

public class ClientCreatedConsumer : IConsumer<ClientCreated>
{
    private readonly IKnownClientRepository _repository;
    public ClientCreatedConsumer(IKnownClientRepository repository) => _repository = repository;

    public async Task Consume(ConsumeContext<ClientCreated> context)
    {
        var message = context.Message;
        var existing = await _repository.GetByIdAsync(message.ClientId);
        if (existing is null)
            await _repository.AddAsync(new KnownClient(message.ClientId, message.Name, message.IsActive));
    }
}

public class ClientUpdatedConsumer : IConsumer<ClientUpdated>
{
    private readonly IKnownClientRepository _repository;
    public ClientUpdatedConsumer(IKnownClientRepository repository) => _repository = repository;

    public async Task Consume(ConsumeContext<ClientUpdated> context)
    {
        var message = context.Message;
        var existing = await _repository.GetByIdAsync(message.ClientId);
        if (existing is not null)
        {
            existing.UpdateFrom(message.Name, message.IsActive);
            await _repository.UpdateAsync(existing);
        }
        else
        {
            await _repository.AddAsync(new KnownClient(message.ClientId, message.Name, message.IsActive));
        }
    }
}

