

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace TIE_Decor.Service;
public static class SessionExtensions
{
    // Phương thức mở rộng để set object vào Session
    public static void SetObject(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    // Phương thức mở rộng để get object từ Session
    public static T GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
