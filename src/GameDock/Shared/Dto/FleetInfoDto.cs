using System;
using System.Collections.Generic;

namespace GameDock.Shared.Dto;

public class FleetInfoDto
{
    public Guid Id { get; set; }
    public Guid BuildId { get; set; }
    public string Status { get; set; }
    public string Runtime { get; set; }
    public int[] Ports { get; set; }
    public string LaunchParameters { get; set; }
    public IDictionary<string, string> Variables { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}