using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient.Attributes;

namespace WebClient
{
    public enum HttpResponseState
    {
        Continue=100,
        [Remark("Switching Protocols")]
        SwitchingProtocols=101,
        OK = 200,
        Created = 201,
        Accepted = 202,
        [Remark("Non-Authoritative Information")]
        NonAuthoritativeInformation =203,
        [Remark("No Content")]
        NoContent = 204,
        [Remark("Reset Content")]
        ResetContent = 205,
        [Remark("Partial Content")]
        PartialContent = 206,
        [Remark("Multiple Choices")]
        MultipleChoices = 300,
        [Remark("Moved Permanently")]
        MovedPermanently = 301,
        Found = 302,
        [Remark("See Other")]
        SeeOther = 303,
        [Remark("Not Modified")]
        NotModified = 304,
        [Remark("Use Proxy")]
        UseProxy = 305,
        Unused = 306,
        [Remark("Temporary Redirect")]
        TemporaryRedirect = 307,
        [Remark("Bad Request")]
        BadRequest = 400,
        [Remark("Not Found")]
        NotFound = 404,
        [Remark("Internal Server Error")]
        InternalServerError = 500,

    }
}
