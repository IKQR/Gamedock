using System;

namespace GameDock.Shared.Responses;

public class UploadResponse
{
    public Guid Id { get; init; }
    public string Status { get; init; }
}