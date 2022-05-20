namespace Migration.Toolkit.Common;

public class ToolkitConfiguration
{
    public string SourceConnectionString { get; set; }
    public string TargetConnectionString { get; set; }
    public EntityConfigurations EntityConfigurations { get; set; }
}