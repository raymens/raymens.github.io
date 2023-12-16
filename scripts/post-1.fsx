#r "nuget: System.Net.Http.Json"

open System.Net.Http
open System.Net.Http.Json
open System.Runtime.InteropServices

[<CLIMutable>]
type User = { Username: string; Email: string }

// Part 1
type UserService = { GetUser: string -> User option }

let realUserService url (apiKey: string) =
    let httpClient = new HttpClient()
    httpClient.DefaultRequestHeaders.Add("Authorization", apiKey)
    httpClient.BaseAddress <- url
    { GetUser = fun username -> httpClient.GetFromJsonAsync<User>($"/user/{username}").Result |> Some }

let fakeUserService userList =
    { GetUser = fun username -> userList |> List.tryFind (fun x -> x.Username = username) }


let client service =
    let getUser username = service.GetUser username

    { GetUser = getUser }

// 1.1

// use of "Single case union type"
type Username = Username of string

type UserServiceSingleUnion = { GetUser: Username -> User }

// use of record
type GetUserRequest = { Username: string }

type UserServiceRecordRequest = { GetUser: GetUserRequest -> User }

// 1.2
// use of record
type GetUserRequestDoc = {
    /// The username of the user
    Username: string
}

type UserServiceRecordRequestDoc = { GetUser: GetUserRequest -> User }

// 2

[<Interface>]
type IUserService =
    abstract member GetUser: username: string -> User

type RealUserService(url, apiKey: string) =
    let httpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", apiKey);
        client.BaseAddress = url
        client

    interface IUserService with
        member this.GetUser(username) =
            httpClient.GetFromJsonAsync<User>($"/user/{username}").Result


// 2.1

type RealUserServiceState(url, apiKey: string) =
    let httpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", apiKey);
        client.BaseAddress = url
        client

    let mutable counter = 0

    interface IUserService with
        member this.GetUser(username) =
            counter <- counter + 1
            httpClient.GetFromJsonAsync<User>($"/user/{username}").Result

// 2.2

[<Interface>]
type IUserServiceOptional =
    abstract member GetUser: username: string * ?isEnabled : bool -> User

type RealUserServiceOptional(url, apiKey: string) =
    let httpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", apiKey);
        client.BaseAddress = url
        client

    interface IUserServiceOptional with
        member this.GetUser(username, isEnabled) =
            // isEnabled is an option
            httpClient.GetFromJsonAsync<User>($"/user/{username}").Result

// 2.3

// 2.2

[<Interface>]
type IUserServiceOptionalDocs =
    /// <summary>
    /// Gets a user by username
    /// </summary>
    /// <param name="username">Username used for searching</param>
    /// <param name="isEnabled">Verify if the user IsEnabled or not</param>
    abstract member GetUser: username: string * ?isEnabled : bool -> User

type RealUserServiceOptionalDocs(url, apiKey: string) =
    let httpClient =
        let client = new HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", apiKey);
        client.BaseAddress = url
        client

    interface IUserServiceOptionalDocs with
        member this.GetUser(username, isEnabled) =
            // isEnabled is an option
            httpClient.GetFromJsonAsync<User>($"/user/{username}").Result

// 4.
type UserServiceType = Real of HttpClient | Fake of User list

let getUser username = function
    | Real httpClient ->  httpClient.GetFromJsonAsync<User>($"/user/{username}").Result
    | Fake userList -> userList |> List.find (fun x -> x.Username = username)

let realService =
    let client = new HttpClient()
    client.DefaultRequestHeaders.Add("Authorization", "key");
    client.BaseAddress <- Uri "Url"
    Real client
let user =
    getUser "raymen" realService