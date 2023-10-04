using Munisio.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Models;

/// <remarks>
/// In this simple example our <see cref="Tweet"/> class extends <see cref="HateoasObject"/>.
/// In a real application your domain model and API model would most likely be seperate classes and only the API model would extend <see cref="HateoasObject"/>
/// </remarks>
public class Tweet : HateoasObject
{
    public int Id { get; private set; }
    public string Text { get; private set; }
    public int UserId { get; private set; }
    public int Retweets { get; set; }
    public int Likes { get; set; }
    public bool IsDeleted { get; private set; }

    public Tweet(int id, string text, int userId)
    {
        Id = id;
        Text = text;
        UserId = userId;
    }

    /// <remarks>
    /// This is not how Twitter really works. But it is a good example of how using HATEOAS can reduce client-side business logic.
    /// The client can simply check for the existence of a "delete" HATEAOAS link when deciding if the deletion UI needs to be shown.
    /// If the logic changes (for example, limiting tweet deletion to a max of 2000 retweets), you only need to update this code and all clients are magically updated.
    /// </remarks>
    public bool CanBeDeleted() => !IsDeleted && Retweets < 1000;
    public bool CanBeLiked() => !IsDeleted;
    public bool CanBeRetweeted() => !IsDeleted;


    public void Delete()
    {
        if (!CanBeDeleted())
        {
            throw new InvalidOperationException("Tweet can't be deleted because it's too popular");
        }

        IsDeleted = true;
    }

    public void Like()
    {
        if (!CanBeLiked())
        {
            throw new InvalidOperationException("Tweet can't be liked because it's deleted");
        }

        Likes++;
    }

    public void Retweet()
    {
        if (!CanBeRetweeted())
        {
            throw new InvalidOperationException("Tweet can't be retweeted because it's deleted");
        }

        Retweets++;
    }
}
