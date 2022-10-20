using Munisio.Samples.AspNetCoreWebApi.Models;

namespace Munisio.Samples.AspNetCoreWebApi.Database;

public class TweetDatabase
{
    private static readonly IList<Tweet> _tweets = new List<Tweet>
    {
        new(1, "Just setting up my twttr.", 12) { Retweets = 178778, Likes = 122462},
        new(2, "Another tweet", 43) { Retweets = 23, Likes = 444 },
    };

    public TweetDatabase()
    {
        foreach (var tweet in _tweets)
        {
            // Important: This is not necessary in real code, just for this sample
            tweet.Links.Clear();
        }
    }

    public Tweet? FindTweet(int id)
    {
        return _tweets.SingleOrDefault(x => x.Id == id && !x.IsDeleted);
    }

    public IEnumerable<Tweet> GetTweets()
    {
        return _tweets.Where(x => !x.IsDeleted).ToList();
    }

    public void AddTweet(Tweet tweet)
    {
        _tweets.Add(tweet);
    }
}
