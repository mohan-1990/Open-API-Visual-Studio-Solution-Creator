using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service_Management.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service_Management.Models;

namespace Service_Management.Controllers
{
    [ApiController]
    [Route("api/service/v1")]
    public class ServiceController : ControllerBase
    {
        private readonly ILogger<ServiceController> _logger;
        private ServiceManagementContext _serviceManagementContext;
        private readonly IConfiguration _appConfiguration;

        public ServiceController(ILogger<ServiceController> logger, ServiceManagementContext serviceManagementContext, IConfiguration appConfiguration)
        {
            _logger = logger;
            _serviceManagementContext = serviceManagementContext;
            _appConfiguration = appConfiguration;
        }
		
		[Route("health")]
        [HttpGet]
        public IActionResult health()
        {
            bool dbConnection = _serviceManagementContext.Database.CanConnect();
            return dbConnection ? StatusCode(200, "Ok") : StatusCode(200, "Probably not ok");
        }
    }
}
