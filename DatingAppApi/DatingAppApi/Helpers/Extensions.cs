using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace DatingAppApi.Helpers
{
    public static class Extensions
    {
        public static void AddAplicationError(this HttpResponse response, string mensaje)
        {
            response.Headers.Add("Application-Error", mensaje);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}