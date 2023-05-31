using System.Text;

namespace GameDock.Server.Infrastructure.Docker;

public class DockerfileBuilder
{
    public int[] Ports { get; set; }
    public string RuntimeKey { get; set; }
    public string EntrypointFile { get; set; }
    public string LaunchParameters { get; set; }
    public IDictionary<string, string> EnvironmentVariables { get; set; }
    
    public string Build()
    {
        var dockerfile = new StringBuilder();

        dockerfile.AppendLine($"FROM {Runtimes[RuntimeKey]}");

        foreach (var port in Ports)
        {
            dockerfile.AppendLine($"EXPOSE {port}");
        }

        foreach (var env in EnvironmentVariables)
        {
            dockerfile.AppendLine($"ENV {env.Key}={env.Value}");
        }

        dockerfile.AppendLine($"WORKDIR /app");
        dockerfile.AppendLine($"COPY . .");

        dockerfile.AppendLine($@"ENTRYPOINT [""./{EntrypointFile}"", ""{LaunchParameters}""]");

        return dockerfile.ToString();
    }
    
    private static readonly Dictionary<string, string> Runtimes = new()
    {
        ["go"] = "golang:latest",
        ["ubuntu"] = "ubuntu:latest",
        ["python"] = "python:latest",
        ["dotnet"] = "mcr.microsoft.com/dotnet/runtime:latest",
    };
}