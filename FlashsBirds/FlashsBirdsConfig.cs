using OnixRuntime.Api.OnixClient;
namespace FlashsBirds {
    public partial class FlashsBirdsConfig : OnixModuleSettingRedirector {
	    [Value(5)]
	    [MinMax(1, 100)]
	    [Name("Maximum Birds", "The maximum amount of birds that can spawn.")]
	    public partial int MaxBirds { get; set; }
    }
}