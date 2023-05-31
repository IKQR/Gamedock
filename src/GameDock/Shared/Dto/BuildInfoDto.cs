using System;

namespace GameDock.Shared.Dto;

public class BuildInfoDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}