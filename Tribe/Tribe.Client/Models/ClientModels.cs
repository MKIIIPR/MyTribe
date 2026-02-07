using System.ComponentModel.DataAnnotations;
using static Tribe.Bib.CommunicationModels.ComModels;

namespace Tribe.Client.Models;

// LoginRequest, LoginResponse und UserInfo sind in Tribe.Bib.CommunicationModels.ComModels definiert.
// Dieses File enthält nur Client-spezifische Modelle.

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}