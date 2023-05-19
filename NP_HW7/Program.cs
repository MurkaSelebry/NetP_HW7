using Newtonsoft.Json;
using RestSharp;

//Чтобы проверить работоспособность измените то, что отмечено *

class Program
{
    //*Измените acessToken
    static string accessToken = "vk1.a.N_ZdJ8F8KVpaa-uDrNEPr09TB--FIfkKGViCZZOuR3GI0DKHrKFk0loc_AClCZd9Yuz030__d69GTZMl9PWbM0t8AitP6YghVJIVqlkiCwmBRqblZREz66Qo4qua1KFzY6vDHuB-1aRNrlte3KP0v5stl9M3kYLF1cq72-IADLzO30cis7hhXmaqlQqXD2fCGu97X53Gu1dXexMRq_576Q";
    //*Можно не менять (это мой юзер id (можете поставить свой))
    static string userId = "195794304";
    //*В большинстве случаев юзер-агент похожий либо такой же
    static string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36";

    static FriendsResponse GetFriendsResponse(RestClient client, RestRequest request)
    {
        RestResponse response = client.Execute(request);
        string jsonResponse = response.Content;
        return JsonConvert.DeserializeObject<FriendsResponse>(jsonResponse);
    }
    static User GetUserInfo(RestClient client, RestRequest request)
    {
        RestResponse userResponse = client.Execute(request);
        string userJsonResponse = userResponse.Content;

        UsersResponse user = JsonConvert.DeserializeObject<UsersResponse>(userJsonResponse);

        if (user != null && user.Response != null && user.Response.Count > 0)
        {
            return user.Response[0];
        }
        return null;
    }
    static RestRequest SetRestRequest(string functionName, string user_id)
    {
        RestRequest request = new RestRequest(functionName, Method.Get);
        request.AddParameter("user_id", user_id);
        request.AddParameter("access_token", accessToken);
        request.AddParameter("v", "5.131");
        request.AddHeader("User-Agent", userAgent);
        return request;
    }
    static void PrintFriends(List<long> friendIds, RestClient client)
    {
        foreach (long friendId in friendIds)
        {
            RestRequest userRequest = SetRestRequest("users.get", friendId.ToString());

            User? userInfo = GetUserInfo(client, userRequest);
            Console.WriteLine($"Friend ID: {friendId}");
            if (userInfo != null)
            {
                Console.WriteLine($"Имя: {userInfo.FirstName}");
                Console.WriteLine($"Фамилия: {userInfo.LastName}");
            }
            Console.WriteLine();


        }
    }
    static void Main(string[] args)
    {

        RestClient client = new RestClient("https://api.vk.com/method");
        RestRequest request = SetRestRequest("friends.get", userId);
        FriendsResponse friends = GetFriendsResponse(client, request);
        if (friends != null && friends.Response != null)
        {
            List<long> friendIds = friends.Response.Items;
            Console.WriteLine($"Кол-во друзей у {GetUserInfo(client, SetRestRequest("users.get", userId)).FirstName}: {friends.Response.Count} ");
            PrintFriends(friendIds, client);
            foreach (long friendId in friendIds)
            {

                RestRequest requestToFriend = SetRestRequest("friends.get", friendId.ToString());
                FriendsResponse friendsToFriend = GetFriendsResponse(client, requestToFriend);
                if (friendsToFriend != null && friendsToFriend.Response != null)
                {
                    List<long> friendx2Ids = friendsToFriend.Response.Items;
                    Console.Clear();
                    Console.WriteLine($"Кол-во друзей у {GetUserInfo(client, SetRestRequest("users.get", friendId.ToString())).FirstName}: {friendsToFriend.Response.Count} ");
                    Thread.Sleep(1000);//Задержка, чтобы показать смену друзей (можно убрать, если наглядность понятна)
                    PrintFriends(friendx2Ids, client);
                }
            }
        }


    }
}

class FriendsResponse
{
    [JsonProperty("response")]
    public FriendsList Response { get; set; }
}

class FriendsList
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("items")]
    public List<long> Items { get; set; }
}

class UsersResponse
{
    [JsonProperty("response")]
    public List<User> Response { get; set; }
}

class User
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }
}