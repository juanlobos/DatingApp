using System;
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

        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage,
        int totalItems,int totalPages)
        {

        }

        public static int CalculateEdad(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }
            return age;
        }
    }
}