using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Lambda.Entrevista.Application.Model
{
    public class EntrevistaRequest
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }
    }
}
