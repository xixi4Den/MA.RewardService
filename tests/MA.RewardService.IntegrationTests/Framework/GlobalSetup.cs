namespace MA.RewardService.IntegrationTests.Framework;

[TestClass]
public class GlobalSetup
{
    [AssemblyInitialize]
    public static async Task Init(TestContext context)
    {
        await TestContainerHelper.Start();
    }

    [AssemblyCleanup]
    public static async Task Cleanup()
    {
        await TestContainerHelper.Stop();
    }
}