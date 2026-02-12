using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Toolbox;

namespace RSShell.Core;

/// <summary>
///     Representing a simple RSS reader class.
/// </summary>
public static class RssReader
{
    /// <summary>
    ///     Attempts to fetch the RSS feed from the given <paramref name="uri" /> address.
    /// </summary>
    /// <param name="uri">The source of the RSS document.</param>
    /// <returns>
    ///     Parsed RSS channel of type <see cref="RssChannel" />.
    ///     If an error occurred, an empty <see cref="RssChannel" /> object is returned.
    /// </returns>
    public static async Task<RssChannel> Read(Uri uri)
    {
        // initialize the return object
        RssChannel channel = new()
        {
            Items = []
        };

        try
        {
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(requestUri: uri);
                if (response.IsSuccessStatusCode)
                {
                    string text = await response.Content.ReadAsStringAsync();

                    // read RSS document
                    XmlDocument doc = new();
                    doc.LoadXml(xml: text);

                    // parse RSS document
                    XmlNode? root = doc.SelectSingleNode(xpath: "/rss/channel");

                    // get channel properties
                    channel.Title = root?.SelectSingleNode(xpath: "title")?.InnerText ?? string.Empty;
                    channel.Description = root?.SelectSingleNode(xpath: "description")?.InnerText ?? string.Empty;
                    channel.Link = root?.SelectSingleNode(xpath: "link")?.InnerText ?? string.Empty;

                    // get items
                    XmlNodeList? nodes = root.SelectNodes(xpath: "item");

                    if (nodes != null && nodes.Count > 0)
                        foreach (XmlNode node in nodes)
                        {
                            RssItem item = new()
                            {
                                Title = node?.SelectSingleNode(xpath: "title")?.InnerText.Trim() ?? string.Empty,
                                Description = node?.SelectSingleNode(xpath: "description")?.InnerText.Trim() ??
                                              string.Empty,
                                Author = node?.SelectSingleNode(xpath: "author")?.InnerText.Trim() ?? string.Empty,
                                Date = node?.SelectSingleNode(xpath: "pubDate")?.InnerText.Trim() ?? string.Empty,
                                Link = node?.SelectSingleNode(xpath: "link")?.InnerText.Trim() ?? string.Empty,
                                Guid = node?.SelectSingleNode(xpath: "guid")?.InnerText.Trim() ?? string.Empty
                            };

                            channel.Items.Add(item: item);
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
            // an error occurred
            _ = Log.Exception(exception: ex, tag: nameof(RssReader.Read));
        }

        return channel;
    }
}