namespace GameDock.Shared.Requests;

public class UploadRequest
{
    public string BuildName { get; set; }
    public string Version { get; set; }
    public string RuntimePah { get; set; }
    public string LaunchParameters { get; set; }
}