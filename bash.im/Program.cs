using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bash.im.lib;
using System.Xml.Linq;


namespace bash.im
{
    static class Program
    {
        public static string Attribute(this XElement ele, string name, string def)
        {
            try { return ele.Attribute(name).Value; }
            catch { return def; }
        }

        static void Main(string[] args)
        {
            var doc = BashToXml.Get("http://bash.im/abyssbest").Root;

            string s = "";
                               

            foreach (var ent in doc.Descendants("div").Where(div => div.Attribute("class", null) == "quote"))
            {
                // get quote number and rating
                var rSpan = ent.Descendants("span").FirstOrDefault(e => e.Attribute("class", "") == "rating");
                if (rSpan == null) continue;

                var Id = int.Parse(rSpan.Attribute("id").Value.Substring(1));
                int? Rating = 0;
                               
                try { Rating = int.Parse(rSpan.Value); }
                catch { Rating = null; }

                // get date number and rating
                var dSpan = ent.Descendants("span").FirstOrDefault(e => e.Attribute("class", "") == "date");
                if (dSpan == null) continue;
                DateTime date = DateTime.Parse(dSpan.Value);

                var tDiv = ent.Descendants("div").FirstOrDefault(e => e.Attribute("class", "") == "text");
                if (tDiv == null) continue;
                var Text = tDiv.Value.Trim();

                s += string.Format("ID: {0}, R: {1} ({3})\n{2}\n\n", Id, Rating, Text, date);
            }
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(s);

            if (doc.Attribute("page") != null)
            {
                int mi = int.Parse(doc.Attribute("min").Value);
                int ma = int.Parse(doc.Attribute("max").Value);
                int p = int.Parse(doc.Attribute("page").Value);

                Console.WriteLine("{0} -> {1}: {2}", mi, ma, p);
            }

            return;
        }
    }
}
