﻿//using AngleSharp.Dom;
//using AngleSharp.Dom.Html;
//using ComboRox.Core.Utilities.SimpleGuard;
//using Html2Amp.Sanitization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace Devdiscourse.Hubs
//{
//    public class AmpIFrameSanitizer : MediaSanitizer
//    {
//        public override bool CanSanitize(IElement element)
//        {
//            return element != null
//                && element is IHtmlInlineFrameElement;
//        }

//        public override IElement Sanitize(IDocument document, IElement htmlElement)
//        {
//            Guard.Requires(document, "document").IsNotNull();
//            Guard.Requires(htmlElement, "htmlElement").IsNotNull();
//            htmlElement.SetAttribute("sandbox", "allow-scripts allow-same-origin");
//            htmlElement.SetAttribute("layout", "responsive");
//            return this.SanitizeCore<IHtmlInlineFrameElement>(document, htmlElement, "amp-iframe");
//        }

//        private bool IsValidSourceAttribute(IElement htmlElement)
//        {
//            var source = new Uri(htmlElement.GetAttribute("src"));
//            var sandbox = htmlElement.GetAttribute("sandbox");

//            // iframes could not be in the same origin as the container, unless they do not specify allow-same-origin.
//            if (this.RunContext != null
//                && this.RunContext.Configuration != null
//                && this.RunContext.RelativeUrlsHostAsUri != null)
//            {
//                if (this.RunContext.RelativeUrlsHostAsUri.Host == source.Host)
//                {
//                    if (!string.IsNullOrEmpty(sandbox) && sandbox.Contains("allow-same-origin"))
//                    {
//                        return true;
//                    }
//                }
//                else
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        protected override bool ShoulRequestResourcesOnlyViaHttps
//        {
//            get { return true; }
//        }
//    }
//}