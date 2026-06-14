using DevFlow.Application.Users.LoginUser;
using DevFlow.Application.Users.LogoutUser;
using DevFlow.Application.Users.RefreshToken;
using DevFlow.Application.Users.RegisterUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LoginUserHandle _loginHandler;
        private readonly RefreshTokenHandle _refreshHandler;
        private readonly LogoutHandle _logoutHandler;
        private readonly RegisterUserHandler _registerUserHandler;
        public AuthController(LoginUserHandle loginHandler, RefreshTokenHandle refreshHandler, LogoutHandle logoutHandler,RegisterUserHandler registerhandler)
        {
            _loginHandler = loginHandler;
            _refreshHandler = refreshHandler;
            _logoutHandler = logoutHandler;
            _registerUserHandler= registerhandler;

        }
       


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            var result = await _loginHandler.Handle(command);
            return Ok(result);
        }
        [HttpPost("Register")]
        public async Task<IActionResult>Register(RegisterUserCommand command)
        {
            var result= await _registerUserHandler.Handle(command);
            return Ok(result);

        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        {
            var result = await _refreshHandler.Handle(command);
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutCommand command)
        {
            await _logoutHandler.Handle(command);
            return Ok("Logged out");
        }



    }
}
