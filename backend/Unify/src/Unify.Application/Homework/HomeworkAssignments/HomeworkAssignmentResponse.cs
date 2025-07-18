﻿using Unify.Application.Files;

namespace Unify.Application.Homework.HomeworkAssignments;

public sealed record HomeworkAssigmentResponse(
    Guid Id,
    Guid ClassOfferingId,
    string Title,
    string Description,
    string? Criteria,
    DateTime DueDate,
    bool Locked,
    List<FileResponse>? Attachments);
