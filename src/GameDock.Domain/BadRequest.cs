namespace GameDock.Domain;

public record BadRequest(string Message, Exception? Exception = null);