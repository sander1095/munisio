using Microsoft.AspNetCore.Mvc;
using Munisio.Models;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Controllers;

[ApiController]
[Route("api/tweets")]
public class TweetsController : ControllerBase
{
    private static readonly IList<Tweet> _tweets = new List<Tweet>
    {
        new(1, "Just setting up my twttr.", 12) { Retweets = 178778, Likes = 122462},
        new(1, "Another tweet", 43) { Retweets = 23, Likes = 444 },
    };

    [HttpGet]
    public ActionResult<HateoasCollection<Tweet>> GetTweets()
    {
        var tweets = _tweets.Where(x => !x.IsDeleted);

        var mappedTweets = HateoasCollection.ForItems(tweets);

        return Ok(mappedTweets);
    }

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<Tweet> GetTweet(int id)
    {
        var tweet = _tweets.SingleOrDefault(x => x.Id == id);

        return tweet is null ? NotFound() : Ok(tweet);
    }

    [HttpPost("{id:int:min(1)}")]
    public ActionResult<Tweet> AddTweet(AddTweetRequestModel requestModel)
    {
        var tweet = _tweets.SingleOrDefault(x => x.Id == id);

        return tweet is null ? NotFound() : Ok(tweet);
    }
}
