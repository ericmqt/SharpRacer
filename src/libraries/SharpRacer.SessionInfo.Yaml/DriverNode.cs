using YamlDotNet.Serialization;

namespace SharpRacer.SessionInfo.Yaml;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public sealed class DriverNode
{
    public string? AbbrevName { get; set; }
    public object CarClassColor { get; set; } = default!;
    public double CarClassEstLapTime { get; set; }
    public string CarClassDryTireSetLimit { get; set; } = default!;
    public int CarClassID { get; set; }
    public int CarClassLicenseLevel { get; set; }
    public string CarClassMaxFuelPct { get; set; } = default!;
    public string CarClassPowerAdjust { get; set; } = default!;
    public int CarClassRelSpeed { get; set; }
    public string? CarClassShortName { get; set; }
    public string CarClassWeightPenalty { get; set; } = default!;
    public string CarDesignStr { get; set; } = default!;
    public int CarID { get; set; }
    public int CarIdx { get; set; }
    public int CarIsAI { get; set; }
    public int CarIsElectric { get; set; }
    public int CarIsPaceCar { get; set; }
    public string CarNumber { get; set; } = default!;
    public string CarNumberDesignStr { get; set; } = default!;
    public int CarNumberRaw { get; set; }
    public string CarPath { get; set; } = default!;
    public string CarScreenName { get; set; } = default!;
    public string CarScreenNameShort { get; set; } = default!;

    [YamlMember(Alias = "CarSponsor_1")]
    public int CarSponsor1 { get; set; }

    [YamlMember(Alias = "CarSponsor_2")]
    public int CarSponsor2 { get; set; }

    public int CurDriverIncidentCount { get; set; }

    public string HelmetDesignStr { get; set; } = default!;
    public string? Initials { get; set; }
    public int IRating { get; set; }
    public int IsSpectator { get; set; }
    public int LicLevel { get; set; }
    public int LicSubLevel { get; set; }
    public string LicString { get; set; } = default!;
    public object LicColor { get; set; } = default!;
    public string SuitDesignStr { get; set; } = default!;
    public int TeamID { get; set; }
    public int TeamIncidentCount { get; set; }
    public string TeamName { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public int UserID { get; set; }
}
