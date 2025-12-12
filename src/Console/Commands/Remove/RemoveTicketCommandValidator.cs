using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Remove;

/// <summary>
/// Validator for the 'remove ticket' command.
/// </summary>
internal class RemoveTicketCommandValidator(
    IUnitOfWork unitOfWork,
    IStringLocalizer<Program> localizer) : ICommandValidator<RemoveTicketCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    public async Task<ValidationResult> ValidateAsync(CommandContext context, RemoveTicketCommand command, CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate Ticket
        if (string.IsNullOrWhiteSpace(command.TicketKey) && !command.TicketId.HasValue)
        {
            result.AddError("Ticket key or ID is required.");
        }
        else if (!string.IsNullOrWhiteSpace(command.TicketKey) && command.TicketId.HasValue)
        {
            result.AddError("Cannot specify both ticket key and ID.");
        }
        else if (!string.IsNullOrWhiteSpace(command.TicketKey))
        {
            if (!await _uow.Tickets.ExistsByKeyAsync(command.TicketKey, token))
            {
                result.AddError(_localizer["Error.Ticket.NotFound"]);
            }
        }
        else if (command.TicketId.HasValue)
        {
            if (await _uow.Tickets.GetByIdAsync(command.TicketId.Value, token) is null)
            {
                result.AddError(_localizer["Error.Ticket.NotFound"]);
            }
        }

        // Validate Iteration
        if (string.IsNullOrWhiteSpace(command.IterationKey) && !command.IterationId.HasValue)
        {
            result.AddError("Iteration key or ID is required.");
        }
        else if (!string.IsNullOrWhiteSpace(command.IterationKey) && command.IterationId.HasValue)
        {
            result.AddError("Cannot specify both iteration key and ID.");
        }
        else if (!string.IsNullOrWhiteSpace(command.IterationKey))
        {
            if (!await _uow.Iterations.ExistsByKeyAsync(command.IterationKey, token))
            {
                result.AddError(_localizer["Error.Iteration.NotFound"]);
            }
        }
        else if (command.IterationId.HasValue)
        {
            if (await _uow.Iterations.GetByIdAsync(command.IterationId.Value, token) is null)
            {
                result.AddError(_localizer["Error.Iteration.NotFound"]);
            }
        }

        return result;
    }
}
