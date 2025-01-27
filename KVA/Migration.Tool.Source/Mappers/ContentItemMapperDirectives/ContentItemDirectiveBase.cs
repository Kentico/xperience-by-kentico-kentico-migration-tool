using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal abstract class ContentItemDirectiveBase
{
    public string? PageTemplateIdentifier { get; set; }
    public JObject? PageTemplateProperties { get; set; }
    public Guid? ContentFolderGuid { get; set; }

}
