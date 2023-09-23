using izolabella.Backend.Objects.ErrorMessages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace izolabella.Backend.Objects.Constants
{
    public static class Errors
    {
        public static ErrorMessage MissingPayload => new(HttpStatusCode.BadRequest, "Method requires missing object.", "Please add an object to send.");
        public static ErrorMessage WrongEntity => new(HttpStatusCode.UnprocessableEntity, "The entity sent could not be understood.", "Please use the correct entity for this endpoint.");
        public static ErrorMessage NotFound => new(HttpStatusCode.NotFound, "Not found.", "");
    }
}