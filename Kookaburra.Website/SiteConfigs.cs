namespace Kookaburra.Website
{
    public class SiteConfigs
    {
        public WidgetConfig Widget { get; set; }
    }

    public class WidgetConfig
    {
        public string LocalHost { get; set; }

        public string StagingHost { get; set; }

        public string ProductionHost { get; set; }
    }
}