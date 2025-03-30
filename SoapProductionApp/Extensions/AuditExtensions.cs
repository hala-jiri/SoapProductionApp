using System.Text.Json;

namespace SoapProductionApp.Extensions
{
    public static class AuditExtensions
    {
        public static string ToSafeJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
    }
}
