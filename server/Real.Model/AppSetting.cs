using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace Real.Model {

    public enum AppSettingType {
        GPSRoundDecimalPlaces = 1,
    }

    public class AppSetting : EntityBase {
        public AppSettingType AppSettingType { get; set; }
        public string Setting { get; set; }
        public string Value { get; set; }

    }
}
