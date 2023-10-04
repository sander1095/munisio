using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Database;

public class UserDatabase
{
    private static readonly IList<User> _users = new List<User>
    {
        new(1, "Sander ten Brinke", "test@example.com"),
        new(2, "John Doe", "john@example.com"),
        new(12, "Jack Dorsey", "jack@twitter.com"),
    };

    public UserDatabase()
    {
        foreach (var user in _users)
        {
            // Important: This is not necessary in real code, just for this sample
            user.Links.Clear();
        }
    }

    public User GetUser(int id)
    {
        return _users.Single(x => x.Id == id);
    }

    public User? FindUser(int id)
    {
        return _users.SingleOrDefault(x => x.Id == id);
    }
}
