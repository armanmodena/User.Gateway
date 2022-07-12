using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace User.Gateway.DTO.Error
{
    [Serializable]
    public class Error
    {
        public int Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Data { get; set; }

    }
}
