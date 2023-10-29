# Munisio

## Overview

This is an ASP.NET Core HATEOAS library that enables you to easily implement HATEOAS principles in your ASP.NET Core Web API projects. HATEOAS is a constraint of the REST architectural style that allows clients to navigate a web API by following hyperlinks contained in the responses.

✨ The inspiration for this project was to use HATEOAS to remove duplicate business logic from your front-end by simply checking for the existence of links of the actions you want to perform instead. If this sounds interesting to you, you can [read this blog post](https://stenbrinke.nl/blog/reducing-duplicate-code-in-our-applications-using-hateoas/) for more information.

❌ Minimal API's [are not supported](https://github.com/ArcadyIT/munisio/issues/19) at the time of writing. Contributions are welcome!

## Getting Started

To get started with this library, follow these simple steps:

### 1. Installation 

Install the package via NuGet Package Manager:

**// TODO: This can only be done after setting up NuGet publishing (https://github.com/ArcadyIT/munisio/issues/3)**
```
dotnet add package Munisio
```

### 2. Configuration

In your `Startup.cs` or wherever you can configure your `IServiceCollection`, configure the HATEOAS service:

```csharp
using Munisio;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Add Munisio's Action Filter to your MVC pipeline
        // so your DTO's can be filled with HATEOAS links
        services.AddControllers(x => x.AddHateoas());
        // ...
    }
}
```

Next, you need to tell Munisio where to find your HATEOAS providers. You can do this in 2 ways:

```csharp
using Munisio;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // This is done in the previous step.
        services.AddControllers(x => x.AddHateoas());
    
        // Option 1: Let Munisio search and register your providers automatically by searching the current assembly
        // or pass the assemblies that contain your providers as an argument.
        services.AddHateoasProviders(); 
    
        // Option 2: Configure your providers yourself. Tweet is a DTO in this case.
        services.AddTransient<IHateoasProvider<Tweet>, TweetHateoasProvider>();
    }
}
```

### 3. Usage

Munisio is now set up 🚀. Now you need to configure your models to support HATEOAS links.

Imagine we have the following DTO:

```csharp
public class Tweet
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public int Retweets { get; set; }
    public int Likes { get; set; }
    public bool IsDeleted { get; set; }
}
```

To be able to return HATEOAS links, you'll need to inherit from the `Munisio.Models.HateoasObject` class or implement the `Munisio.Models.IHateoasObject` interface:


```csharp
public class Tweet : HateoasObject
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int UserId { get; set; }
    public int Retweets { get; set; }
    public int Likes { get; set; }
    public bool IsDeleted { get; set; }

    // HateoasObject contains the following:
    // public ICollection<HateoasLink> Links { get; } = new List<HateoasLink>();

    public bool CanBeDeleted()
    {
        // This is only an example.
        // Normally you would only store this on your domain model and let it contain complex logic.
        return true;
    }
}
```

You'll now need to implement the `IHateoasProvider<TDTOType>` or `IAsyncHateoasProvider<TDTOType` interface, depending on if you need async support to resolve your links or not. I recommend storing these in the same project as your Web Api.

```csharp
public class TweetHateoasProvider : IAsyncHateoasProvider<Tweet>
{
    public TweetHateoasProvider()
    {
        // Feel free to inject services you need to resolve links into the constructor of your providers!
    }

    public async Task EnrichAsync(IHateoasContext context, Tweet model)
    {
        // You can add links by specifying the URL to the related endpoint yourself.
        // AddLink() adds a GET link.
        // You can also use AddPatchLink(), AddDeleteLink(), AddPutLink(), AddPostLink() accordingly.
        model.AddLink("getUser", $"api/users/{model.UserId}");

        // Or you can let ASP.NET Core resolve the link itself to avoid hardcoding URLs.
        // "GetUser" would be the name of the action in a UsersController, for example.
        model.AddLink("getTweets", context.LinkGenerator.GetPathByName(context.HttpContext, "GetTweets", values: null)!);

        // The "context" property contains ASP.NET Core's authorization service so you can add links only if a user is authorized to perform a specific action.
        // Furthermore, there are multiple extension methods available after calling Add*Link() so you can have fine-grained control about when a link is added or not.
        await model
            .AddDeleteLink("delete", $"api/tweets/{model.Id}")
            .When(() => tweet.CanBeDeleted())
            .WhenAsync(() => context.AuthorizeAsync(model, Operations.Delete)); // Note: Operations.Delete is custom!

        model.AddPatchLink("retweet", context.LinkGenerator.GetPathByName(context.HttpContext, "Retweet", values: null)!);
    }
}
```

If you were to retrieve this tweet using `api/tweets/1`, the result can look like this:

```json
{
  "id": 1,
  "text": "Just setting up my twttr.",
  "userId": 12,
  "retweets": 178778,
  "likes": 122462,
  "isDeleted": false,
  "links": [
    {
      "rel": "getUser",
      "href": "api/users/12",
      "method": "GET"
    },
    {
      "rel": "getTweets",
      "href": "api/tweets",
      "method": "GET"
    },
    {
      "rel": "delete",
      "href": "api/tweets/1",
      "method": "DELETE"
    },
    {
      "rel": "retweet",
      "href": "api/tweets/1/retweet",
      "method": "PATCH"
    }    
  ]
}
```

Finally:

- I recommend taking a look at the `samples` folder, Intellisense in your IDE or at the `HateoasExtensions.cs` and `HateoasLinkBuilder.cs` for more options!
- I recommend having 1 "provider class" per "type": `TweetHateoasProvider` could also implement `IHateoasProvider<>` or `IAsyncHateoasProvider<>` for other `Tweet` DTO types, like list models, etc..

## Advanced features

This library contains some more advanced features.

### Returning HATEOAS in lists
The samples provided in this Readme aren't very complex. More complex features can be found in the `samples/` folder. One of these "advanced" features is adding HATEOAS to a list entity and onto its children. An example:

```json
{
  "items": [
    {
      "id": 1,
      "text": "Just setting up my twttr.",
      "userId": 12,
      "retweets": 178778,
      "likes": 122462,
      "isDeleted": false,
      "links": [
        {
          "rel": "getUser",
          "href": "api/users/12",
          "method": "GET"
        },
        {
        "rel": "getTweets",
        "href": "api/tweets",
        "method": "GET"
        },
        {
          "rel": "delete",
          "href": "api/tweets/1",
          "method": "DELETE"
        },
        {
          "rel": "retweet",
          "href": "api/tweets/1/retweet",
          "method": "PATCH"
        }
      ]
    }
  ],
  "links": [
    {
      "rel": "addTweet",
      "href": "api/tweets",
      "method": "POST"
    }
  ]
}
```

In order to do this, you'll need to return a `HateoasCollection<>` from your API:

```csharp
[HttpGet(Name = "GetTweets")]
public ActionResult<HateoasCollection<Tweet>> GetTweets()
{
    var tweets = _database.GetTweets();
    var mappedTweets = HateoasCollection.ForItems(tweets);
    return Ok(mappedTweets);
}
```

The `Tweet` must also inherit from HateoasObject to make this work!

Now, you can extend the previously described `TweetHateoasProvider` as follows to get the result mentioned above.

```csharp
public class TweetHateoasProvider :
    IAsyncHateoasProvider<Tweet>,
    IHateoasProvider<HateoasCollection<Tweet>>
{
    // .... Existing code can be found above

    // New code!
    public void Enrich(IHateoasContext context, HateoasCollection<Tweet> model)
    {
        // Note: You probably want to add some more advanced checks here.
        // For example, "is the user logged in?"
        // But for now we'll keep things simple!
        model.AddPostLink("addTweet", "api/tweets");
    }
}
```

### Storing DTO's in different assemblies
Some projects do not store their DTO's in the same assembly as their Web API where the Controllers live. In this case, you should install `Munisio.Models` in those assemblies so you have access to types like `IHateoasObject`, `HateoasObject`, `HateoasCollection<>`, etc..


**// TODO: This can only be done after setting up NuGet publishing (https://github.com/ArcadyIT/munisio/issues/3)**
```
dotnet add package Munisio.Models
```

This way your DTO projects do not require a FrameworkReference for ASP.NET Core.

## Sample

For a more detailed usage example, check out the provided sample project in the `samples` folder.

## Contributions

Contributions are welcome! If you find a bug or have an idea for improvement, please open an issue or submit a pull request!

## License

This library is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
