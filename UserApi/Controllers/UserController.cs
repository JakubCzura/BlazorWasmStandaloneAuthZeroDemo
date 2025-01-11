using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.AuthApi;

namespace UserApi.Controllers;

public class UserController : ApiControllerBase
{
    private static readonly List<User> _users =
    [
        new() { Name = "John", Surname = "Doe" },
        new() { Name = "Johny", Surname = "Be Good" },
        new() { Name = "Ann", Surname = "Funny" }
    ];

    [HttpGet("all")]
    public IActionResult GetAll() => Ok(_users);

    [HttpPost("add")]
    [AllowAnonymous]
    public IActionResult AddUser([FromBody] User newUser)
    {
        _users.Add(newUser);
        return Ok();
    }
}