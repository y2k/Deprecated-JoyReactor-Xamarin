using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.Model.Web;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model.Parser
{
    public class TagInformationRequest
    {
        public List<Group> LinkedTags { get; set; }

        ID id;
        string pageHtml;

        public TagInformationRequest(ID id)
        {
            this.id = id;
        }

        public async Task ComputeAsync()
        {
            pageHtml = await new Downloader{ id = id }.DownloadTagPageWithCheckDomainAsync();
            await ExtractLinkedTags();
        }

        #region Extract tag information

        int GetNextPageOfTagList()
        {
            var currentPageRx = new Regex(@"<span class='current'>(\d+)</span>");
            return currentPageRx.FirstInt(pageHtml) - 1;
        }

        #endregion

        #region Extract linked tags

        async Task ExtractLinkedTags()
        {
            var doc = new HtmlDocument();
            await doc.LoadHtmlAsync(pageHtml);

            LinkedTags = new List<Group>();
            foreach (var s in doc.DocumentNode.Select("div.sidebar_block"))
                ExtractTagsFromBlock(s);
        }

        void ExtractTagsFromBlock(HtmlNode blockNode)
        {
            var title = blockNode.Select("h2.sideheader.random").FirstOrDefault()?.InnerText.Trim();
            var tags = LinkedTagExtractor.Get()
                .Select(s => s.Extract(blockNode))
                .Where(s => s != null && s.Count > 0)
                .Select(s => new Group { Title = title, Tags = s.ToList() })
                .FirstOrDefault();
            if (title != null && tags != null)
                LinkedTags.Add(tags);
        }

        abstract class LinkedTagExtractor
        {
            internal static ICollection<LinkedTagExtractor> Get()
            {
                return new LinkedTagExtractor[]
                { 
                    new PopularTagExtractor(), 
                    new RandomTagExtractor(), 
                    new SubTagExtractor()
                };
            }

            internal abstract ICollection<Tag> Extract(HtmlNode root);

            class PopularTagExtractor : LinkedTagExtractor
            {
                internal override ICollection<Tag> Extract(HtmlNode root)
                {
                    var weekBest = root.Descendants().FirstOrDefault(s => s.Id == "blogs_week_content");
                    return weekBest == null ? null : new RandomTagExtractor().Extract(weekBest);
                }
            }

            class RandomTagExtractor : LinkedTagExtractor
            {
                internal override ICollection<Tag> Extract(HtmlNode root)
                {
                    return root
                        .Select("a > img")
                        .Select(s => new Tag { BestImage = s.Attr("src"), Title = s.Attr("alt") })
                        .ToList();
                }
            }

            class SubTagExtractor : LinkedTagExtractor
            {
                internal override ICollection<Tag> Extract(HtmlNode root)
                {
                    return root
                        .Select("td > img")
                        .Select(s => new Tag
                        {
                            BestImage = s.Attr("src"),
                            Title = LinkToTagName(FindLinkToTag(s))
                        })
                        .ToList();
                }

                static string FindLinkToTag(HtmlNode s)
                {
                    return s.ParentNode.ParentNode.ChildNodes[1].FirstChild.Attr("href");
                }

                static string LinkToTagName(string link)
                {
                    return Uri.UnescapeDataString(Uri.UnescapeDataString(link.Substring("/tag/".Length))).Replace('+', ' ');
                }
            }
        }

        #endregion

        class Downloader
        {
            internal ID id { get; set; }

            IProviderAuthStorage authStorage = ServiceLocator.Current.GetInstance<IProviderAuthStorage>();
            WebDownloader downloader = ServiceLocator.Current.GetInstance<WebDownloader>();

            internal async Task<string> DownloadTagPageWithCheckDomainAsync()
            {
                return await downloader.GetTextAsync(
                    await GenerateUrl(),
                    new RequestParams
                    {
                        Cookies = await authStorage.GetCookiesAsync(),
                        UseForeignProxy = true,
                    });
            }

            async Task<Uri> GenerateUrl()
            {
                const int currentPage = 0;
                var url = new StringBuilder("http://");
                if (id.Type == ID.TagType.Favorite)
                {
                    url.Append(TagUrlBuilder.DefaultDomain);
                    if (id.Tag == null)
                        id.Tag = await authStorage.GetCurrentUserNameAsync();
                    url.Append("/user/").Append(Uri.EscapeDataString(id.Tag)).Append("/favorite");
                    return new Uri("" + url);
                }
                return new TagUrlBuilder().Build(id, currentPage);
            }
        }

        public class Group
        {
            internal string Title { get; set; }

            internal List<Tag> Tags { get; set; }
        }
    }
}