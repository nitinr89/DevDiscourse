using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Devdiscourse.Models.Others
{
    public class CommonEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Display(Name = "Event Title")]
        public string? EventTitle { get; set; }
        [Display(Name = "Event Venue")]
        public string? EventVenue { get; set; }
        [Display(Name = "Event Tagline")]
        public string? EventTagline { get; set; }
        [Display(Name = "Logo Image")]
        public string? LogoUrl { get; set; }
        [Display(Name = "Banner Image")]
        public string? BackgroundBannerUrl { get; set; }

        public string? Content { get; set; }
        [Display(Name = "Registration Url")]
        public string? RegistrationUrl { get; set; }
        [Display(Name = "Short Description")]
        public string? ShortDescription { get; set; }
        [Display(Name = "Keywords")]
        public string? PageKeywords { get; set; }

        public string? Name { get; set; }
        [Display(Name = "Event Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Event End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        [Display(Name = "Address Locality")]
        public string? AddressLocality { get; set; }
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }
        [Display(Name = "Address Region")]
        public string? AddressRegion { get; set; }
        [Display(Name = "Address Country")]
        public string? AddressCountry { get; set; }
        [Display(Name = "Event Schema Image Url")]
        public string? Image { get; set; }
        [Display(Name = "Event Schema Description")]
        public string? Description { get; set; }
        [Display(Name = "Event Schema Offer URL")]
        public string? Url { get; set; }

        public int? Price { get; set; }
        [Display(Name = "Price Currency")]
        public string? PriceCurrency { get; set; }
        [Display(Name = "Availability Url")]
        public string? AvailabilityUrl { get; set; }
        [Display(Name = "Valid From Date")]
        public DateTime? ValidFrom { get; set; }

        public string? Type { get; set; }
        [Display(Name = "Card Image")]
        public string? CardUrl { get; set; }

        public bool Featured { get; set; }

        [Display(Name = "Event Theme")]
        public int? EventTheme { get; set; }

        public virtual ICollection<EventNavLink>? EventNavLinks { get; set; }

        public string GenerateSecondSlug()
        {
            string phrase = string.Format("{0}-{1}", Id, Name);

            string str = RemoveAccent(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 130 ? str.Length : 130).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private string RemoveAccent(string text)
        {
            //byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

    }
}