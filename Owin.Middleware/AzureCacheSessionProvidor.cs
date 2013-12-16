using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Owin;

namespace Owin.Middleware
{
    public static class AzureCacheSessionProvidor 

{
    private const string SessionCookieName = "SESSIONID";
    private const int SessionIdLength = 32;
    private static readonly RNGCryptoServiceProvider Rng = new RNGCryptoServiceProvider();

    /// <summary>
    /// 
    /// </summary>
    public static string PreInvoke(IOwinContext context, string owinSessionKeyName)
    {
        var cache = context.Cache();
        if (cache == null)
            throw new InvalidOperationException("no azure cache client");

        ConcurrentDictionary<string, object> session;

        var sessionId = context.Request.Cookies[SessionCookieName];
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            sessionId = ToHex(GetRandomBytes(SessionIdLength));
            session = new ConcurrentDictionary<string, object>();
        }
        else
        {
            session = cache.TryGet<ConcurrentDictionary<string, object>>(sessionId);
            if (session == null)
            {
                sessionId = ToHex(GetRandomBytes(SessionIdLength));
                session = new ConcurrentDictionary<string, object>();
            }
        }

        context.Environment[owinSessionKeyName] = session;
        context.Response.Cookies.Append(SessionCookieName, sessionId);

        return sessionId;
    }

    public static void PostInvoke(IOwinContext context, string owinSessionKeyName, string sessionId)
    {
        var cache = context.Cache();
        if (cache == null)
            throw new InvalidOperationException("no azure cache client");

        // ReSharper disable once PossibleInvalidCastException
        var session = context.Environment[owinSessionKeyName];

        cache.Put(sessionId, session);
    }


    private static byte[] GetRandomBytes(int length)
    {
        var data = new byte[length];
        Rng.GetNonZeroBytes(data);
        return data;
    }


    private static readonly char[] Hexchars = new char[]
    {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

    private static string ToHex(byte[] data)
    {
        var c = new char[data.Length*2];
        for (var i = 0; i < data.Length; i++)
        {
            byte d = data[i];
            c[i*2] = Hexchars[d/0x10];
            c[i*2 + 1] = Hexchars[d%0x10];
        }
        return new string(c);
    }

}

}
