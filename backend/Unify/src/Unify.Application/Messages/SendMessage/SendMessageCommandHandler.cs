using Unify.Application.Abstractions.Clock;
using Unify.Application.Abstractions.Files;
using Unify.Application.Abstractions.Messaging;
using Unify.Domain.Abstractions;
using Unify.Domain.Messages;
using Unify.Domain.Users;

namespace Unify.Application.Messages.SendMessage;

public sealed class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileConversionService _fileConversionService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMessageRepository _messageRepository;

    public SendMessageCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IFileConversionService fileConversionService, IDateTimeProvider dateTimeProvider, IMessageRepository messageRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _fileConversionService = fileConversionService;
        _dateTimeProvider = dateTimeProvider;
        _messageRepository = messageRepository;
    }

    public async Task<Result<Guid>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        //validate
        var sender = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (sender is null)
        {
            return Result.Failure<Guid>(MessageErrors.SenderNotFound);
        }

        var recipients = await _userRepository.GetManyByIdAsync(request.RecipientsIds, cancellationToken);
        if (recipients.Count == 0)
        {
            return Result.Failure<Guid>(MessageErrors.RecipientNotFound);
        }


        var attachments = new List<Attachment>();

        if (request.Attachments != null && request.Attachments.Any())
        {
            var recipientsNotFound = recipients.Where(r => request.RecipientsIds.Contains(r.Id) == false).ToList();;
            if (recipientsNotFound.Any())
            {
                return Result.Failure(MessageErrors.RecipientNotFound, recipientsNotFound.First().Id);
            }

            var attachmentResults = await _fileConversionService.ConvertToAttachments(request.Attachments);
            if (attachmentResults.Any(result => result.IsFailure))
            {
                var firstError = attachmentResults.First(result => result.IsFailure);
                return Result.Failure<Guid>(firstError.Error);
            }

            attachments = attachmentResults.Select(a => a.Value).ToList();
        }



        //create and persist

        var message = Message.Send(sender,
            new Title(request.Title),
            new TextContent(request.Content),
            recipients.ToList(),
            attachments,
            _dateTimeProvider.UtcNow);

        _messageRepository.Add(message);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(message.Id);
    }

}