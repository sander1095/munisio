using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Munisio.Samples.AspNetCoreWebApi.Database;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserDatabase _database;

    public UsersController(UserDatabase database)
    {
        _database = database;
    }

    [HttpGet("{id:int:min(1)}")]

    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<User> GetUser(int id)
    {
        var user = _database.FindUser(id);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult UpdateUserData(int id, UpdateUserDataRequestModel requestModel)
    {
        var user = _database.FindUser(id);

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult ActivateUser(int id)
    {
        var user = _database.FindUser(id);

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

    [HttpPatch("{id:int:min(1)}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult DeactivateUser(int id)
    {
        var user = _database.FindUser(id);

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
