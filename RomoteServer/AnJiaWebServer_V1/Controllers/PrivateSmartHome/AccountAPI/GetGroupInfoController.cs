using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnJiaWebServer_V1.Controllers
{
    /// <summary>
    /// 获取某个小组的具体信息
    /// 比如小组成员
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class GetGroupInfoController : Controller
    {




    }
}
