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
    private readonly IKnownClientRepository _knownClientRepository;
    private readonly IAccountRepository _accountRepository;

    public ClientUpdatedConsumer(IKnownClientRepository knownClientRepository,
        IAccountRepository accountRepository)
    {
        _knownClientRepository = knownClientRepository;
        _accountRepository = accountRepository;
    }

    public async Task Consume(ConsumeContext<ClientUpdated> context)
    {
        var message = context.Message;
        var existing = await _knownClientRepository.GetByIdAsync(message.ClientId);
        if (existing is not null)
        {
            existing.UpdateFrom(message.Name, message.IsActive);
            await _knownClientRepository.UpdateAsync(existing);
        }
        else
        {
            await _knownClientRepository.AddAsync(new KnownClient(message.ClientId, message.Name, message.IsActive));
        }

        var accounts = await _accountRepository.GetAllByClientIdAsync(message.ClientId);
        foreach (var account in accounts.Where(a => a.IsActive != message.IsActive))
        {
            if (message.IsActive) account.Activate();
            else account.Deactivate();
            await _accountRepository.UpdateAsync(account);
        }
    }
}