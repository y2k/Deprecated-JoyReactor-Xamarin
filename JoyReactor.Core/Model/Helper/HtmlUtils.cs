using System;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace JoyReactor.Core.Model.Helper
{
    public static class HtmlUtils
    {
        public static string HtmlToString(this string s)
        {
            return s.Replace("<br>", Environment.NewLine);
        }

        public static string UnescapeDataString(this string s)
        {
            return Uri.UnescapeDataString(s);
        }

        public static Task LoadHtmlAsync(this HtmlDocument instance, string html)
        {
            return Task.Run(() => instance.LoadHtml(html));
        }
    }
}