using System;
using System.IO;
using System.Threading.Tasks;
using GameDock.Server.Handlers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GameDock.Server.Hubs;

public class ImageHub : Hub
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;

    public ImageHub(ILogger<ImageHub> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("{connectionId} connected", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _logger.LogWarning("{connectionId} disconnected", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task<string> Build(string fileName, byte[] fileData)
    {
        var stream = new MemoryStream(fileData);
        stream.Flush();

        var result = await _mediator.Send(new StartBuildRequest(Context.ConnectionId,fileName, stream));

        return $"{result.ImageName}:{result.Tag}";
    }
}