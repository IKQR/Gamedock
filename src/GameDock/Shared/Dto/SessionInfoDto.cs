﻿using System;

namespace GameDock.Shared.Dto;

public class SessionInfoDto
{
    public string Id { get; set; }
    public Guid BuildId { get; set; }
    public string Ip { get; set; }
    public ushort Port { get; set; }
    public DateTime StartedAt { get; set; }
}