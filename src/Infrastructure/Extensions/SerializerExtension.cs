using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CleanArchitecture.Infrastructure.Extensions
{
    public static class SerializerExtension
    {
        public static StringContent ToStringContent(this Object o)
        {
            string json = JsonSerializer.Serialize(o, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
