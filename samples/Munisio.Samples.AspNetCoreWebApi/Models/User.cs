using Munisio.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Models;

/// <remarks>
/// In this simple example our <see cref="User"/> class extends <see cref="HateoasObject"/>.
/// In a real application your domain model and API model would most likely be seperate classes and only the API model would extend <see cref="HateoasObject"/>
/// </remarks>
public class User : HateoasObject
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public bool IsActive { get; private set; } = true;

    public User(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public bool CanChangeName() => IsActive;
    public bool CanChangeEmail() => IsActive;
    public bool CanBeActivated() => !IsActive;
    public bool CanBeDeactivated() => IsActive;

    public void ChangeName(string name)
    {
        if (!CanChangeName())
        {
            throw new InvalidOperationException("Name can't be changed because the user is deactivated");
        }

        Name = name;
    }

    public void ChangeEmail(string email)
    {
        if (!CanChangeEmail())
        {
            throw new InvalidOperationException("Email can't be changed because the user is deactivated");
        }

        Email = email;
    }


    public void Activate()
    {
        if (!CanBeActivated())
        {
            throw new InvalidOperationException("User can't be activated because they are already activated");
        }

        IsActive = true;
    }

    public void Deactivate()
    {
        if (!CanBeDeactivated())
        {
            throw new InvalidOperationException("User can't be deactivated because they are already deactivated");
        }

        IsActive = false;
    }
}
