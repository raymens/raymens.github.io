---
layout: post
title: Composition or inheritance
author: @raymens77
published: 2023-12-17
---

Hi all,

This is my first blog article so I'd appreciate your feedback, you can find me on [Twitter / X]("https://twitter.com/raymens77").
I was hoping to touch on more subjects and in a more detailed way but I'm surprised by the time it takes to write.
Something I hope I will be able to improve in the future.

# Article

For several reasons you might need to construct types that need different implementations.
Usually you want to keep the implementation details away from the consumer of that type.

In case of C# this usually implies an interface with multiple implementations.
Initially you can start with some basic functions where the first parameter is the interface.
However, this might not scale or can become verbose.
Especially when actual state is involved and other cross cutting concerns e.g. connections, caching, logging. 

Some examples of types where this could apply are:
- Abstract Database Service that needs a live connection while deployed and uses lists for tests
- Api client implementations where there's an API rate limit that must be kept track of

<!--more-->
<!-- For example like the following example (not fully valid C#).
```csharp
interface IApiService
{
    Account GetAccount(string accountCode);
}

class ApiService : IApiService
{
    private readonly string url;
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public ApiService(string url, string apiKey) 
    {
        this.url = url;
        this.apiKey = apiKey;
        this.httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
        httpClient.BaseAddress = url;
    }

    public Account GetAccount(string accountCode)
    {
        return httpClient.GetFromJsonAsync<Account>($"/account/{accountCode}");
    }
}
``` -->

## Function records

```fsharp
type UserService =
    { GetUser: string -> User }

let realUserService url apiKey =
    let httpClient = HttpClient()
    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
    httpClient.BaseAddress <- url;    
    { GetUser = fun username -> httpClient.GetFromJsonAsync<User>($"/user/{username}") }

let fakeUserService userList =
    { GetUser = fun username -> userList |> List.tryFind (fun x -> x.Username = username) }
```

One way of having multiple abstracted implementations is to create a record that contains one or multiple function properties.
This is a very succint (implies short and simple) way that is my preference for just that reason.
There are some downsides however...

Named parameters are not supported. This will become an issue once you need multiple parameters and even more when these parameters are of the same type.
There are some workarounds that can be applied, you could apply single case union types or create another record that contains all the parameters.

Something like the following:
```fsharp
// use of "Single case union type"
type Username = Username of string

type UserService =
    { GetUser: Username -> User }

// use of record
type GetUserRequest = { Username : string }

type UserService =
    { GetUser: GetUserRequest -> User }
```

Optional parameters are also in the strict sense not supported, but that can be bypassed by defining them with the `option` type.

```fsharp
// The bool could mean anything but in this case it's for "is enabled".
// If it's None it will be ignored and otherwise will check if the user is enabled or not.
type UserService =
    { GetUser: string -> bool option -> User }

```

Code Documentation and it's implementation within the IDEs is also quite important and even more so when the size and team increases.
The support of documentation depends on the different subflavors applied.

In the individual properties of the records you can write the shorthand `///` documentation and explain the meaning.
```fsharp
/// Manage Users
type UserService =
    {   /// Get User using username and an optional IsEnabled check
        GetUser: string -> bool option -> User }

```
When your parameters are also properties you can be more precise by doing the following:

```fsharp
/// Define requirements for getting a user
type GetUserRequestDoc = 
    { /// The username of the user
      Username: string
      /// When `Some``, check if the user is enabled in case of true and disabled in case of false.
      /// Do nothing when `None``.
      IsEnabled: bool option }

```

## Inheritance
```fsharp
[<Interface>]
type IUserService =
    abstract member GetUser: username: string -> User

type RealUserService(url, apiKey) =
    let httpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", apiKey);
        client.BaseAddress <- url
        client

    interface IUserService with
        member this.GetUser(username) =
            httpClient.GetFromJsonAsync<User>($"/user/{username}")
```

This implementation should feel natural to the C# (and related) developers.
There's an explicit interface that defines the contract what all implementations need to abide too.
In thise case the service explicitly implements the interface.
For me this always feels like a step back into being more verbose.
It's like writing C# code while using F# syntax.

The upside is that named parameters are very much possible as can be seen in the initial example.

State can also be easily tracked within the object it self, which is visualized below.
```fsharp
// .....

// for example a counter that could be to track for API rate limits
let mutable counter = 0

interface IUserService with
    member this.GetUser(username) =
        counter <- counter + 1
        httpClient.GetFromJsonAsync<User>($"/user/{username}").Result
```
Optional parameters are also naturally supported.
```fsharp
// Optional parameters
[<Interface>]
type IUserServiceOptional =
    abstract member GetUser: username: string * ?isEnabled : bool -> User

type RealUserServiceOptional(url, apiKey: string) =
    // ...

    interface IUserServiceOptional with
        member this.GetUser(username, isEnabled) =
            // isEnabled is an option
            httpClient.GetFromJsonAsync<User>($"/user/{username}").Result
```

Documentation can be done similar to the record implementation but the more verbose XML documentation is also supported.
This form allows you to refer to specific parameters and is especially helpfull if your code is being consumed by another (*non-F#*) dotnet library.

```fsharp
[<Interface>]
type IUserServiceOptionalDocs =
    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">Username used for searching</param>
    /// <param name="isEnabled">Verify if the user IsEnabled or not</param>
    abstract member GetUser: username: string * ?isEnabled : bool -> User
```

# Alternatives

There are some more alternative ways of accomplishing similar things.
Most likely even quite some that I'm not even familiar with.

For example, you can make all of the required functions be supplied one by one like so.
```fsharp
let someMethodThatNeedsToGetAUserAndThenDelete
    (getUser: string -> User)
    (deleteUser: User -> bool)
    =
    let user = getUser "raymen"
    deleteUser user
``` 
Which you can then also turn into a type itself...
```fsharp
type GetUser = string -> User
type DeleteUser = User -> bool

let someMethodThatNeedsToGetAUserAndThenDelete
    (getUser: GetUser)
    (deleteUser: DeleteUser)
    =
    let user = getUser "raymen"
    deleteUser user
``` 
And also mix and match these tools with the record implementations mentioned before.

A complete diffent take is to use Discriminated Unions together with functions like the following.
```fsharp
type UserServiceType = Real of HttpClient | Fake of User list

let getUser username = function
    | Real httpClient ->  httpClient.GetFromJsonAsync<User>($"/user/{username}").Result
    | Fake userList -> userList |> List.find (fun x -> x.Username = username)

let realService = 
    let client = new HttpClient()
    client.DefaultRequestHeaders.Add("Authorization", "key");
    client.BaseAddress <- "Url"
    Real client
let user =
    getUser "raymen" realService
```
This can work in some scenarios, but the obvious limitation is that every single function must be aware of the implementation being used.
Even more of an issue when another implementation is added and therefore all these functions need to be adjusted to support it.

# Conclusion

There are no winners or losers.

For me personally I like to start in the most succint way possible, functions and data.

When later on a need develops that requires more control, or the need to have state, I usually refactor to a solution that is more OO than before.

<!-- 
# Alternatives

There are many more alternative ways of accomplishing



### Pros
Types

## Currying

```fsharp

let getClient url apiKey =
    let httpClient = HttpClient()
    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);
    httpClient.BaseAddress = url;

let client = getClient url apiKey

let realGetAccount httpClient accountCode = httpClient.GetFromJsonAsync<Account>($"/account/{accountCode}")

let getAccount accountCode = realGetAccount client

``` -->