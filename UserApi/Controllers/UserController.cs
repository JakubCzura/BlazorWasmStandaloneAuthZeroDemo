using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Auth;
using Shared.UserApi;

namespace UserApi.Controllers;

public class UserController : ApiControllerBase
{
    private static readonly List<UserInfo> _usersInfo =
    [
        new() { Name = "John", Surname = "Doe", Email = "johnemail@mail.com", Phone = "900000001" },
        new() { Name = "Johny", Surname = "Be Good", Email = "johnygoodemail@mail.com", Phone = "800000002" },
        new() { Name = "Ann", Surname = "Funny", Email = "annfunny@mail.com", Phone = "700000005" }
    ];

    [HttpGet("all")]
    [Authorize(Policy = PolicyConstants.GetUser)]
    public IActionResult GetAll() => Ok(_usersInfo.Select(x => new User() { Id = x.Id, Name = x.Name, Surname = x.Surname}));

    [HttpGet("all/info")]
    [Authorize(Roles = "admin, user")]
    public IActionResult GetAllInfo() => Ok(_usersInfo);

    [HttpPost("add")]
    [AllowAnonymous]
    public IActionResult AddUser([FromBody] UserInfo newUser)
    {
        _usersInfo.Add(newUser);
        return Ok();
    }
}