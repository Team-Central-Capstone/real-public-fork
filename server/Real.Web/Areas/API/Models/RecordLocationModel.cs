
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Real.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Real.Web.Areas.API.Models {
    public class RecordLocationModel {

        /// <summary>
        /// Firebase UID
        /// </summary>
        /// <example>12345</example>
        [Required]
        [MaxLength(36)]
        public string id { get; set; }
        
        /// <summary>
        /// Device ID
        /// </summary>
        /// <example>12345</example>
        [Required]
        [MaxLength(64)]
        public string deviceid { get; set; }

        /// <summary>
        /// Decimal latitude
        /// </summary>
        /// <example>41.69214870870307</example>
        [Required]
        public double latitude { get; set; }
        
        /// <summary>
        /// Decimal longitude
        /// </summary>
        /// <example>-72.76593865516493</example>
        [Required]
        public double longitude { get; set; }

        /// <summary>
        /// (optional) UTC Date/time position was recorded
        /// </summary>
        /// <example>2022-01-29 17:40:21.802275</example>
        [DataType(DataType.DateTime)]
        public DateTime? time { get; set; }


        // if the GPS speed is too great, ignore it


        
    }
}