using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Migration.Tool.Source.Mappers.ContentItemMapperDirectives;
internal record OverrideTemplateDirective(string TemplateIdentifier, JObject TemplateProperties) : IContentItemDirective;
