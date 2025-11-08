using SPTarkov.Server.Core.Models.Spt.Mod;
using Range = SemanticVersioning.Range;
using Version = SemanticVersioning.Version;

namespace FPVDroneServerMod
{
    public record Metadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.pein.fpvdronemod";
        public override string Name { get; init; } = "FPV Drone Mod";
        public override string Author { get; init; } = "pein";
        public override Version Version { get; init; } = new Version("0.1.0");
        public override Range SptVersion { get; init; } = new Range("~4.0.0");
        public override string? Url { get; init; } = "https://github.com/peinwastaken";
        public override bool? IsBundleMod { get; init; } = true;
        public override string License { get; init; } = "MIT";
        public override Dictionary<string, Range>? ModDependencies { get; init; } = new ()
        {
            { "com.wtt.commonlib", new Range("~2.0.0") }
        };
        
        // unused
        public override List<string>? Incompatibilities { get; init; }
        public override List<string>? Contributors { get; init; }
    }
}
