﻿using System;
using Microsoft.AspNetCore.Mvc;
using User.Gateway.DTO;

namespace User.Gateway.Controllers
{
    public class BaseController : Controller
    {
        protected ObjectResult ErrorResponse(Exception ex, int code = 500)
        {
            var error = new
            {
                Status = code,
                Message = ex.Message,
                InnerException = ex.InnerException?.Message,
                Data = ex.Data,
                Source = ex.Source
            };
            return StatusCode(code, error);
        }

        protected ObjectResult HttpResponse(ErrorDto data)
        {
            return data.Status > 1000 ? StatusCode(400, data) : StatusCode(data.Status, data);
        }

        protected ObjectResult HttpResponse(ResponseDataDto data)
        {
            return data.Status > 1000 ? StatusCode(400, data) : StatusCode(data.Status, data);
        }
    }
}