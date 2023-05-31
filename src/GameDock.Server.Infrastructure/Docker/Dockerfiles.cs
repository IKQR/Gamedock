namespace GameDock.Server.Infrastructure.Docker;

public static class Dockerfiles
{
    public static string GetLinuxDockerfile(IEnumerable<int> ports, string runtimePath, string launchParameters) => $@"
            FROM mcr.microsoft.com/dotnet/runtime:7.0
            {string.Join(Environment.NewLine, ports.Select(x => $"EXPOSE " + x))}
            WORKDIR /app
            COPY . .
            ENTRYPOINT [""./{runtimePath}"", ""{launchParameters}""]
        ";
}