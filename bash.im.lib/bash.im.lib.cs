using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace bash.im.lib
{
    public class Quote {
        public int Id { get; set; }
        public int Votes { get; set; }
        public DateTime PublishedTime { get; set; }
        public string Text { get; set; }
    }


    public interface IQuoteSource
    {
        List<Quote> Quotes();
        List<Quote> Quotes(int seed);
        List<Quote> More();
    }


    public static class BashToXml
    {
        static string RemoveTag(this string content, string tag, bool closed)
        {
            if (closed)
            {
                for (var begin = content.IndexOf("<" + tag); begin > -1; begin = content.IndexOf("<" + tag))
                {
                    var end = content.IndexOf("</" + tag);
                    content = content.Substring(0, begin) + content.Substring(end + 3 + tag.Length);
                }
            }
            else
            {
                for (var begin = content.IndexOf("<" + tag); begin > -1; begin = content.IndexOf("<" + tag))
                {
                    var end = content.IndexOf("/>");
                    content = content.Substring(0, begin) + content.Substring(end + 2);
                }
            }
            return content;
        }

        static string ReplaceRange(this string content, params string[] entities)
        {
            for (int ein = 0; ein < entities.Length; ein += 2)
                while (content.IndexOf(entities[ein]) != -1) content = content.Replace(entities[ein], entities[ein+1]);

            return content;
        }
        static string RemoveTagRange(this string content, params string[] entities)
        {
            foreach (var ent in entities) content = content.RemoveTag(ent, true);
            return content;
        }

        static string RemoveComments(this string content)
        {
            for (int begin = content.IndexOf("<!--"); begin != -1; begin = content.IndexOf("<!--")) {
                var end = content.IndexOf("-->");
                content = content.Substring(0, begin) + content.Substring(end + 3);
            }
            return content;
        }

        


        public static XDocument Get(string url)
        {
            HttpClient http = new HttpClient();

            var httpResult = http.GetAsync(url);
            while (!httpResult.IsCompleted) ;

            var enc = Encoding.GetEncoding("windows-1251");
            var byteArrayResult = httpResult.Result.Content.ReadAsByteArrayAsync();
            while (!httpResult.IsCompleted) ;
            var byteArray = byteArrayResult.Result;

            var content = enc.GetString(byteArray, 0, byteArray.Length).Substring(16);

            var begin = content.IndexOf(@"<div class=""quote"">");
            var end = Math.Max(content.IndexOf(@"<div class=""pager"">", begin), content.IndexOf(@"<div class=""quote more"">", begin));

            if (end < 0) end = content.IndexOf(@"<div class=""inside"">");


            var pb = content.IndexOf(@"<input type=""text"" name=""page"" class=""page"" pattern=""[0-9]+""");
            var pend = pb > 0 ? content.IndexOf("</span>", pb) : -1;

            XElement pager = null;

            if (pb >= 0) pager = XDocument.Parse(content.Substring(pb, pend - pb)).Root;

            int currPage = 0, minPage = 0, maxPage = 0;

            bool pages = false;

            if (pager != null)
                pages = int.TryParse(pager.Attribute("min").Value, out minPage) && int.TryParse(pager.Attribute("max").Value, out maxPage) && int.TryParse(pager.Attribute("value").Value, out currPage);    

            var ect = content.Substring(begin, end - begin)
                .ReplaceRange(
                "&nbsp;", " ",
                "&rarr;", "→",
                "&middot", "·",
                "&ndash;", "–",
                "&mdash;", "—",
                "<br />", "\n",
                "<br>", "\n"
                )
                .RemoveTagRange("iframe", "script", "noscript")
                .RemoveComments();

            var qs = "<quotes>";

            if (pages)            
                qs = string.Format("<quotes min=\"{0}\" max=\"{1}\" page=\"{2}\">", minPage, maxPage, currPage);
            

            return XDocument.Parse(qs + ect + "</quotes>");            
        }
    }

    public class MainPageQuoteSource : IQuoteSource
    {
         
        public List<Quote> Quotes()
        {
            throw new NotImplementedException();
        }

        public List<Quote> More()
        {
            throw new NotImplementedException();
        }


        public List<Quote> Quotes(int seed)
        {
            throw new NotImplementedException();
        }
    }




}
