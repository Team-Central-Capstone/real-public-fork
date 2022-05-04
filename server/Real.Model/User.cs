using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Model {

    internal static partial class Extensions {
        internal static ZodiacSign GetZodiacSign(this DateTime? Birthdate) {
            if (!Birthdate.HasValue)
                return ZodiacSign.Unknown;

            switch (Birthdate.Value.Month) {
                case 1:
                    return Birthdate.Value.Day <= 19 ? ZodiacSign.Capricorn : ZodiacSign.Aquarius;
                case 2:
                    return Birthdate.Value.Day <= 18 ? ZodiacSign.Aquarius : ZodiacSign.Pisces;
                case 3:
                    return Birthdate.Value.Day <= 20 ? ZodiacSign.Pisces : ZodiacSign.Aries;
                case 4:
                    return Birthdate.Value.Day <= 19 ? ZodiacSign.Aries : ZodiacSign.Taurus;
                case 5:
                    return Birthdate.Value.Day <= 20 ? ZodiacSign.Taurus : ZodiacSign.Gemini;
                case 6:
                    return Birthdate.Value.Day <= 21 ? ZodiacSign.Gemini : ZodiacSign.Cancer;
                case 7:
                    return Birthdate.Value.Day <= 22 ? ZodiacSign.Cancer : ZodiacSign.Leo;
                case 8:
                    return Birthdate.Value.Day <= 22 ? ZodiacSign.Leo : ZodiacSign.Virgo;
                case 9:
                    return Birthdate.Value.Day <= 22 ? ZodiacSign.Virgo : ZodiacSign.Libra;
                case 10:
                    return Birthdate.Value.Day <= 23 ? ZodiacSign.Libra : ZodiacSign.Scorpio;
                case 11:
                    return Birthdate.Value.Day <= 21 ? ZodiacSign.Scorpio : ZodiacSign.Sagittarius;
                case 12:
                    return Birthdate.Value.Day <= 21 ? ZodiacSign.Sagittarius : ZodiacSign.Capricorn;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public enum ZodiacSign {
        Unknown,
        Capricorn,
        Aquarius,
        Pisces,
        Aries,
        Taurus,
        Gemini,
        Cancer,
        Leo,
        Virgo,
        Libra,
        Scorpio,
        Sagittarius,
    }

    public class User : EntityBase {

        /// <summary>
        /// UID assigned from Firebase
        /// </summary>
        [IndexColumn("IX_Users_FirebaseUserId", 0, IsUnique = true)]
        [MaxLength(36)]
        public string FirebaseUserId { get; set; }

        [StringLength(320)]
        public string Email { get; set; }

        /// <summary>
        /// Is the user active in the system
        /// </summary>
        public bool Active { get; set; } = false;

        
        [Display(Name = "User since")]
        public DateTime RegisteredTimestamp { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Last login")]
        public DateTime LastLoginTimestamp { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "Profile last updated")]
        public DateTime? ProfileLastUpdatedTimestamp { get; set; }

        
        [Display(Name = "First name")]
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [Display(Name = "Preferred name")]
        [MaxLength(30)]
        public string PreferredName { get; set; }
        
        [Display(Name = "Last name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [NotMapped]
        public string DisplayName { 
            get {
                if (!String.IsNullOrEmpty(LastName) && (!String.IsNullOrEmpty(PreferredName ?? FirstName)))
                    return $"{LastName}, {PreferredName ?? FirstName}";
                return $"User {Id}";
            }
        }

        [DataType(DataType.Date)]
        [Remote("AtLeast18YearsOld", "Validation", "API", ErrorMessage = "You must be at least 18 years old to use this app")]
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// Zodiac sign, calculated from the user's birthdate
        /// </summary>
        public string Zodiac => Birthdate.GetZodiacSign().ToString();

        [System.Text.Json.Serialization.JsonIgnore]
        public ZodiacSign ZodiacSign => Birthdate.GetZodiacSign();

        [Display(Name = "Gender")]
        public int? UserGenderId { get; set; }
        public virtual UserGender UserGender { get; set; }

        [Display(Name = "Body Type")]
        public int? UserBodyTypeId { get; set; }
        public virtual UserBodyType UserBodyType { get; set; }

        [Display(Name = "Height (inches)")]
        public int HeightInches { get; set; }

        [Display(Name = "Profile Summary")]
        [StringLength(1000)]
        public string ProfileIntro { get; set; }


        [Display(Name = "Gender(s) attracted to")]
        public virtual List<UserGender> GendersAttractedTo { get; set; } = new List<UserGender>();

        // public int MinimumCompatabilityRating { get; set; }
        // public double MaximumDistance { get; set; }


        [NotMapped]
        public Location CurrentLocation { get; set; }

        [NotMapped]
        public virtual List<Location> Locations { get; set; } 

        [Display(Name = "Survey Responses")]
        public virtual List<UserSurveyResponse> UserSurveyResponses { get; set; } = new List<UserSurveyResponse>();

        [Display(Name = "Pictures")]
        public virtual List<UserImage> UserImages { get; set; } = new List<UserImage>();


    }
    
}
