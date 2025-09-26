using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace RSShell.Core;

/// <summary>
/// Representing a simple RSS reader class.
/// </summary>
public static class RssReader
{
    public static async Task<RssChannel> Read(Uri uri)
    {
        // initialize the return object
        RssChannel channel = new RssChannel
        {
            Items = []
        };

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string text = await response.Content.ReadAsStringAsync();

                    // read RSS document
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(text);

                    // parse RSS document
                    var root = doc.SelectSingleNode("/rss/channel");

                    // get channel properties
                    channel.Title = root?.SelectSingleNode("title")?.InnerText ?? string.Empty;
                    channel.Description = root?.SelectSingleNode("description")?.InnerText ?? string.Empty;
                    channel.Link = root?.SelectSingleNode("link")?.InnerText ?? string.Empty;

                    // get items
                    XmlNodeList nodes = root.SelectNodes("item");

                    if (nodes != null && nodes.Count > 0)
                    {
                        foreach (XmlNode node in nodes)
                        {
                            RssItem item = new RssItem
                            {
                                Title = node?.SelectSingleNode("title")?.InnerText ?? string.Empty,
                                Description = node?.SelectSingleNode("description")?.InnerText ?? string.Empty,
                                Author = node?.SelectSingleNode("author")?.InnerText ?? string.Empty,
                                Date = node?.SelectSingleNode("pubDate")?.InnerText ?? string.Empty,
                                Link = node?.SelectSingleNode("link")?.InnerText ?? string.Empty,
                                Guid = node?.SelectSingleNode("guid")?.InnerText ?? string.Empty,
                            };

                            channel.Items.Add(item);
                        }
                    }
                }

                else
                {
                    // error, source inaccessible
                    return channel;
                }
            }
        }

        catch (Exception ex)
        {
            // TODO: add logging
        }

        return channel;
    }
}
