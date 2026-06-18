using DevFlow.Application.Users.LoginUser;
using DevFlow.Application.Users.LogoutUser;
using DevFlow.Application.Users.RefreshToken;
using DevFlow.Application.Users.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;

        }
       


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Register")]
        public async Task<IActionResult>Register(RegisterUserCommand command)
        {
            var result= await _mediator.Send(command);
            return Ok(result);

        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            await _mediator.Send(command);
            return Ok("Logged out");
        }
        [HttpGet("test")]
        [Authorize]
        public IActionResult TestProtected()
        {
            return Ok("You are authorized");
        }


    }
}
