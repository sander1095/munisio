using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Munisio.Models;
using Munisio.Samples.AspNetCoreWebApi.Database;
using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Controllers;

[ApiController]
[Route("api/tweets")]
public class TweetsController : ControllerBase
{

    private readonly TweetDatabase _database;

    public TweetsController(TweetDatabase database)
    {
        _database = database;
    }

    [HttpGet(Name = "GetTweets")]
    [ProducesResponseType(typeof(HateoasCollection<Tweet>), StatusCodes.Status200OK)]
    public ActionResult<HateoasCollection<Tweet>> GetTweets()
    {
        var tweets = _database.GetTweets();

        var mappedTweets = HateoasCollection.ForItems(tweets);

        return Ok(mappedTweets);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetTweet")]
    [ProducesResponseType(typeof(Tweet), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Tweet> GetTweet(int id)
    {
        var tweet = _database.FindTweet(id);

        return tweet is null ? NotFound() : Ok(tweet);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Tweet), StatusCodes.Status201Created)]
    public ActionResult<Tweet> AddTweet(AddTweetRequestModel requestModel)
    {
        var tweet = new Tweet(new Random().Next(10, 1000000), requestModel.Text, 43);

        _database.AddTweet(tweet);

        return CreatedAtAction("GetTweet", new { id = tweet.Id }, tweet);
    }

    [HttpPatch("{id:int:min(1)}/like")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Tweet> LikeTweet(int id)
    {
        var tweet = _database.FindTweet(id);

        if (tweet is null)
        {
            return NotFound();
        }

        try
        {
            tweet.Like();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPatch("{id:int:min(1)}/retweet")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Tweet> RetweetTweet(int id)
    {
        var tweet = _database.FindTweet(id);

        if (tweet is null)
        {
            return NotFound();
        }

        try
        {
            tweet.Retweet();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Tweet> DeleteTweet(int id)
    {
        var tweet = _database.FindTweet(id);

        if (tweet is null)
        {
            return NotFound();
        }

        try
        {
            tweet.Delete();
        }
        catch (InvalidOperationException ex)
        {
            return Problem(detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }
}

public record AddTweetRequestModel([Required(AllowEmptyStrings = false)] string Text);

