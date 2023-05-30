using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GameDock.Shared.Dto;

public class CreateFleetDto
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