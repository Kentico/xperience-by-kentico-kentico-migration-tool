namespace Migration.Toolkit.Model;

public class DataClassModel
{
    public virtual int ClassID { get; set; }
    public virtual string ClassDisplayName { get; set; }
    
    public string ClassXmlSchema { get; set; }
    public string ClassSearchSettings { get; set; }
    public string ClassCodeGenerationSettings { get; set; }
    public int ClassFormLayoutType { get; set; }
}