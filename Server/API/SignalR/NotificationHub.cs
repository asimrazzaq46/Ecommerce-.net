﻿
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace API.SignalR;

[Authorize]
public class NotificationHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> UserConnection = new(); 
    public override Task OnConnectedAsync()
    {
        var email = Context.User.GetEmail();
        if (!string.IsNullOrEmpty(email))  UserConnection[email] = Context.ConnectionId;

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User.GetEmail();
        if (!string.IsNullOrEmpty(email)) UserConnection.TryRemove(email,out _);

        return base.OnDisconnectedAsync(exception);
    }


    public static string? GetConnectionIdByEmail(string email)
    {
         UserConnection.TryGetValue(email,out var connectionId);
        return connectionId;
    }


}
