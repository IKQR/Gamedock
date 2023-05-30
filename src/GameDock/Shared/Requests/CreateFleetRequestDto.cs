using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GameDock.Shared.Requests;

public class CreateFleetRequestDto
{
    [Required]
    public Guid BuildId { get; set; }
    [Required]
    public string Runtime { get; set; }
    [Required]
    public int[] Ports { get; set; }
    [AllowNull]
    public string LaunchParameters { get; set; }
    [AllowNull]
    public Dictionary<string, string> Variables { get; set; }
}