using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

using Toolbox;

namespace RSShell.Core;

/// <summary>
/// Representing a simple RSS reader class.
/// </summary>
public static class RssReader
{
    /// <summary>
    /// Attempts to fetch the RSS feed from the given <paramref name="uri"/> address.
    /// </summary>
    /// <param name="uri">The source of the RSS document.</param>
    /// <returns>
    /// Parsed RSS channel of type <see cref="RssChannel"/>.
    /// If an error occurred, an empty <see cref="RssChannel"/> object is returned.
    /// </returns>
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
                                Title = node?.SelectSingleNode("title")?.InnerText.Trim() ?? string.Empty,
                                Description = node?.SelectSingleNode("description")?.InnerText.Trim() ?? string.Empty,
                                Author = node?.SelectSingleNode("author")?.InnerText.Trim() ?? string.Empty,
                                Date = node?.SelectSingleNode("pubDate")?.InnerText.Trim() ?? string.Empty,
                                Link = node?.SelectSingleNode("link")?.InnerText.Trim() ?? string.Empty,
                                Guid = node?.SelectSingleNode("guid")?.InnerText.Trim() ?? string.Empty,
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
            // an error occurred
            _ = Log.Exception(ex, nameof(Read));
        }

        return channel;
    }
}
