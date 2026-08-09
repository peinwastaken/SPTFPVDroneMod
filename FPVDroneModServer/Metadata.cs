using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace FPVDroneModServer
{
    public record Metadata : IModMetadata
    {
        public string ModGuid { get; init; } = "com.pein.fpvdronemod";
        public string Name { get; init; } = "FPV Drone Mod";
        public string Author { get; init; } = "pein";
        public Version Version { get; init; } = new Version("0.9.0");
        public Range SptVersion { get; init; } = new Range("~4.1.0");
        public string? Url { get; init; } = "https://github.com/peinwastaken/SPTFPVDroneMod";
        public string License { get; init; } = "MIT";
        public Dictionary<string, Range>? ModDependencies { get; init; } = new ()
        {
            { "com.wtt.commonlib", new Range("^3.0.3") }
        };
        
        // unused
        public List<string>? Incompatibilities { get; init; }
        public List<string>? Contributors { get; init; }
        public bool HasPrepatcher { get; init; }
    }
}
