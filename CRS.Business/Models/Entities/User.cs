using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRS.Common.DataAnnotations;
using CRS.Resources;

namespace CRS.Business.Models.Entities
{
    public class User
    {
        #region Primitive properties

        public int Id { get; set; }
        [StringLengthExtended(30)]
        [Display(Name = "Username", ResourceType = typeof(Labels))]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        [StringLengthExtended(1000)]
        public string AvatarUrl { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? Birthday { get; set; }
        public string Sex { get; set; }
        public string Hobbies { get; set; }
        public string CookingExp { get; set; }
        [StringLengthExtended(1000)]
        public string SocialNetworkUrl { get; set; }

        [RequiredExtended]
        [MinExtended(0)]
        public int Point { get; set; }
        public string Level { get; set; }
        public int UserStateId { get; set; }
        public int? LocationId { get; set; }
        public string ResetPasswordKey { get; set; }
        public DateTime? LastLogin { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LastLoginAttempted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? TodayDate { get; set; }
        public int? TodayPoint { get; set; }

        #endregion

        #region Navigation properties

        [ForeignKey("UserStateId")]
        public UserState UserState { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [InverseProperty("PostedBy")]
        public virtual ICollection<Answer> Answers { get; set; }
        #endregion

        #region Other properties

        [NotMapped]
        public int NumberOfAnswers { get; set; }

        [NotMapped]
        public int NumberOfQuestions { get; set; }

        [NotMapped]
        public int NumberOfTips { get; set; }

        [NotMapped]
        public int NumberOfNews { get; set; }

        [NotMapped]
        public int NumberOfRecipes { get; set; }

        [NotMapped]
        public bool IsLoggedIn { get; set; }

        #endregion
    }
}