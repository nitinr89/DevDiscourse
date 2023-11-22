using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using ComboRox.Core.Utilities.SimpleGuard;
using Html2Amp.Sanitization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Devdiscourse.Hubs
{
    public class TwitterSanitizer : MediaSanitizer
    {
        protected readonly List<string> AllowedAttribtes = new List<string>() { "width", "height", "id" };

        public const string CardIdRegex = @"^/(\w+)/status/(?<id>[^/\?]+)";
        public override bool CanSanitize(IElement element)
        {
            if (element == null || !(element is IHtmlQuoteElement) || element.GetAttribute("class") != "twitter-tweet")
            {
                return false;
            }

            var sourceAttributeValue = ((IHtmlQuoteElement)element).Children.Where(s => s.LocalName == "a").First().GetAttribute("href");

            Uri sourceUri;
            if (Uri.TryCreate(sourceAttributeValue, UriKind.Absolute, out sourceUri))
            {
                return Regex.IsMatch(sourceUri.Host, @"^(www\.)?twitter?\.com$");
            }

            return false;
        }

        public override IElement Sanitize(IDocument document, IElement htmlElement)
        {
            Guard.Requires(document, "document").IsNotNull();
            Guard.Requires(htmlElement, "htmlElement").IsNotNull();

            var ampElement = document.CreateElement("amp-twitter");

            htmlElement.CopyAttributes(ampElement, this.AllowedAttribtes);
            this.SetElementLayout(htmlElement, ampElement);

            Uri videoUri = new Uri(htmlElement.Children.Where(s => s.LocalName == "a").First().GetAttribute("href"));

            var cardId = this.GetCardId(videoUri);
            ampElement.SetAttribute("data-tweetid", cardId);
            ampElement.SetAttribute("layout", "responsive");
            ampElement.SetAttribute("width", "375");
            ampElement.SetAttribute("height", "472");

            //var videoParams = HttpUtility.ParseQueryString(videoUri.Query);
            //this.SetVideoParams(ampElement, videoParams);

            htmlElement.Parent.ReplaceChild(ampElement, htmlElement);

            return ampElement;
        }

        protected virtual void SetVideoParams(IElement ampElement, NameValueCollection videoParams)
        {
            Guard.Requires(ampElement, "ampElement").IsNotNull();
            Guard.Requires(videoParams, "videoParams").IsNotNull();

            foreach (var paramName in videoParams.AllKeys)
            {
                var ampParamAttributeName = "data-param-" + paramName;
                ampElement.SetAttribute(ampParamAttributeName, videoParams[paramName]);
            }
        }

        protected virtual string GetCardId(Uri videoUri)
        {
            Guard.Requires(videoUri, "videoUri").IsNotNull();

            var cardIdMatch = Regex.Match(videoUri.LocalPath, CardIdRegex);

            return cardIdMatch.Groups["id"].Value;
        }
    }
}