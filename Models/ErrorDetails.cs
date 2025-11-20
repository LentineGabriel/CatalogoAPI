using System.Text.Json;

namespace CatagoloAPI.Models;
public class ErrorDetails
{
    #region Properties
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string? Trace { get; set; }
    #endregion

    #region Methods
    public override string ToString() => JsonSerializer.Serialize(this);
    #endregion
}
