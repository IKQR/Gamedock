namespace GameDock.Server.Application.Services;

public interface IImageBuilder
{
    Task BuildImageFromFleet(string key, Stream sourceCode, int[] ports, string runtime, string entrypointFile,
        string launchParameters, IDictionary<string, string> environmentVariables,
        CancellationToken cancellationToken = default);
}