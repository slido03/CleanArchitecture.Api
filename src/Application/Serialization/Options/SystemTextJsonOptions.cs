using CleanArchitecture.Application.Interfaces.Serialization.Options;
using System.Text.Json;

namespace CleanArchitecture.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}