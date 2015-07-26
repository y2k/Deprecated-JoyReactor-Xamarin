using System;
using System.Text.RegularExpressions;

namespace JoyReactor.Core.Model.Parser
{
    public class PageDomainAnalyzator
    {
        string html;

        public PageDomainAnalyzator(string html)
        {
            this.html = html;
        }

        internal bool IsSecret()
        {
            var postCount = Regex.Matches(html, "class=\"postContainer\"").Count;
            var unsafeMarker = html.Contains(">секретные разделы</a>");
            if (postCount == 0 && unsafeMarker)
                return true;
            var unsafePostCount = Regex.Matches(html, "images/unsafe_").Count;
            if (postCount > 0 && postCount == unsafePostCount)
                return true;
            return false;
        }
    }
}