using Munisio.Models;
using Munisio.Samples.AspNetCoreWebApi.Database;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Hateoas;

public class TweetHateoasProvider :
    IHateoasProvider<Tweet>,
    IAsyncHateoasProvider<HateoasCollection<Tweet>>
{
    private readonly TweetDatabase _database;

    public TweetHateoasProvider(TweetDatabase database)
    {
        _database = database;
    }

    public void Enrich(IHateoasContext context, Tweet model)
    {
        var tweet = _database.GetTweet(model.Id);

        model.AddLink("getTweets", "api/tweets");
        model.AddLink("getUser", $"api/users/{model.UserId}");

        model
            .AddPatchLink("like", $"api/tweets/{tweet.Id}/like")
            .When(() => tweet.CanBeLiked());

        model
            .AddPatchLink("retweet", $"api/tweets/{tweet.Id}/retweet")
            .When(() => tweet.CanBeRetweeted());

        model
            .AddDeleteLink("delete", $"api/tweets/{tweet.Id}")
            .When(() => tweet.CanBeDeleted());
    }

    /// <remarks>
    /// We implement <see cref="IAsyncHateoasProvider{TModel}"/> here to demonstrate that 
    /// if your <see cref="EnrichAsync(IHateoasContext, HateoasCollection{Tweet})"/> function would need async support, 
    /// you would need to implement <see cref="IAsyncHateoasProvider{TModel}"/> instead of <see cref="IHateoasProvider{TModel}"/>.
    /// 
    /// This method doesn't really need async support, so we just return a completed task.
    /// </summary>
    public Task EnrichAsync(IHateoasContext context, HateoasCollection<Tweet> model)
    {
        model.AddPostLink("addTweet", "api/tweets");

        return Task.CompletedTask;
    }
}
