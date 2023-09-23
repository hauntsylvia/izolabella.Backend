using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.ErrorMessages.Base
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ErrorMessage
    {
        public ErrorMessage(HttpStatusCode Code, string Message, string HelpfulDescription)
        {
            this.Code = Code;
            this.Message = Message;
            this.HelpfulDescription = HelpfulDescription;
        }

        [JsonIgnore]
        public HttpStatusCode Code { get; }
        public string Message { get; }
        public string HelpfulDescription { get; }
    }
}