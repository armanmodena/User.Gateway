using System;
using System.Text.Json.Serialization;

namespace User.Gateway.DTO
{
    [Serializable]
    public class ErrorDto
    {
        public int Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Errors { get; set; }
    }
}
