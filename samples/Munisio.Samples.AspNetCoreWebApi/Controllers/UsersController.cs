using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private static readonly IList<User> _users = new List<User>
    {
        new(1, "Sander ten Brinke", "test@example.com"),
        new(2, "John Doe", "john@example.com"),
    };

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<User> GetUser(int id)
    {
        var user = _users.SingleOrDefault(x => x.Id == id);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:int:min(1)}")]
    public ActionResult UpdateUserData(int id, UpdateUserDataRequestModel requestModel)
    {
        var user = _users.SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        try
        {
            user.ChangeName(requestModel.Name);
            user.ChangeEmail(requestModel.Email);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}/activate")]
    public ActionResult ActivateUser(int id)
    {
        var user = _users.SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        try
        {
            user.Activate();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}/activate")]
    public ActionResult DeactivateUser(int id)
    {
        var user = _users.SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound();
        }

        try
        {
            user.Deactivate();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }
}

public record UpdateUserDataRequestModel([Required(AllowEmptyStrings = false)] string Name, [Required(AllowEmptyStrings = false)] string Email);
