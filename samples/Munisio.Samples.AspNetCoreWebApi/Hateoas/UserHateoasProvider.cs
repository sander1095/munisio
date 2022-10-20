using Munisio.Samples.AspNetCoreWebApi.Database;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Hateoas;

public class UserHateoasProvider : IHateoasProvider<User>
{
    private readonly UserDatabase _database;

    public UserHateoasProvider(UserDatabase database)
    {
        _database = database;
    }

    public void Enrich(IHateoasContext context, User model)
    {
        var user = _database.GetUser(model.Id);

        model.AddPatchLink("update", $"api/users/{user.Id}")
            .When(() => user.CanChangeEmail() && user.CanChangeName());

        model.AddPatchLink("activate", $"api/users/{user.Id}/activate")
            .When(() => user.CanBeActivated());

        model.AddPatchLink("deactivate", $"api/users/{user.Id}/deactivate")
            .When(() => user.CanBeDeactivated());
    }
}
