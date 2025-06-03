
using CleanArchitecture.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}