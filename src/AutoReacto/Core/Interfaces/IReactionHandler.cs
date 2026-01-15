using Discord.WebSocket;

namespace AutoReacto.Core.Interfaces;

/// <summary>
/// Interface for handling message reactions
/// </summary>
public interface IReactionHandler
{
    /// <summary>
    /// Processes a message and adds appropriate reactions
    /// </summary>
    /// <param name="message">The Discord message to process</param>
    /// <returns>Number of reactions added</returns>
    Task<int> HandleMessageAsync(SocketMessage message);
}
