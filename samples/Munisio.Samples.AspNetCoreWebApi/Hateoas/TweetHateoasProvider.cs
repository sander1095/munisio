using Microsoft.AspNetCore.Mvc;
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

        // You can use the context.LinkGenerator property to generate links to endpoints.
        // This way you do not need to hardcode them.
        model.AddLink("getTweets", context.LinkGenerator.GetPathByName(context.HttpContext, "GetTweets", values: null)!);

        // However, you can also use the route parameter to specify the route string itself!
        model.AddLink("getUser", $"api/users/{model.UserId}");

        // Here's an example of a dynamic HATEOAS link that is only added if the action is possible.
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
