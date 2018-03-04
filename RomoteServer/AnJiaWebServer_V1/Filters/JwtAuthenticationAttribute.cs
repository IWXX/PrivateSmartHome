using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Security.Claims;
using System.Security.Principal;
using System.Threading;

namespace AnJiaWebServer_V1.Filters
{
    public class JwtAuthenticationAttribute: Attribute
    {
  

    }
}
