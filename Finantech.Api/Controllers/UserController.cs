﻿using Finantech.Decorators;
using Finantech.DTOs.Events;
using Finantech.DTOs.User;
using Finantech.Extensions;
using Finantech.Services.Interfaces;
using Finantech.Utils;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Finantech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest data)
        {
            var result = await _userService.CreateUserAync(data);

            if (result.IsSuccess)
            {
                return Created("Auth/Authenticate", result.Value);
            }

            return result.HandleReturnResult();

        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail/{token}")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var result = await _userService.ConfirmEmailAsync(token);

            return result.HandleReturnResult();
        }

        [AllowAnonymous]
        [HttpGet("SendConfirmEmail/{userId}")]
        public async Task<IActionResult> GenerateConfirmEmailToken(int userId)
        {
            var result = await _userService.GenerateConfirmEmailTokenAsync(userId);

            return result.HandleReturnResult();
        }

        [ExtractTokenInfo]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            int userId = (int)(HttpContext.Items["UserId"] as int?)!;

            var result = await _userService.ChangePasswordAsync(changePasswordRequest, userId);

            return result.HandleReturnResult();
        }

        [AllowAnonymous]
        [HttpPost("SendForgotPasswordEmail")]
        public async Task<IActionResult> ChangePassword([FromBody] ForgotPasswordEvent forgotPasswordEmail)
        {
            var result = await _userService.GenerateForgotPasswordTokenAsync(forgotPasswordEmail.Email);

            return result.HandleReturnResult();
        }

        [AllowAnonymous]
        [HttpGet("VerifyForgotPasswordToken{token}")]
        public async Task<IActionResult> VerifyForgotPasswordToken([FromRoute] string token)
        {
            var result = await _userService.VerifyForgotPasswordTokenAsync(token);

            return result.HandleReturnResult();
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword{token}")]
        public async Task<IActionResult> ForgotPassword([FromRoute] string token, [FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            var result = await _userService.ForgotPasswordAsync(token, forgotPasswordRequest);

            return result.HandleReturnResult();
        }
    }
}
