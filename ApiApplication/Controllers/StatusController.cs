using System;
using Microsoft.AspNetCore.Mvc;
using ApiApplication.Models;
using ApiApplication.Services;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly IImdbStatusHostedService _service;

        public StatusController(IImdbStatusHostedService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public ActionResult<StatusModel> Get()
        {
            var model = new StatusModel
            {
                Up = _service.Up,
                LastCall = _service.LastCall
            };

            return Ok(model);
        }
    }
}
