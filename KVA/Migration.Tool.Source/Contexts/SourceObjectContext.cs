using CMS.FormEngine;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Contexts;

public record DocumentSourceObjectContext(ICmsTree CmsTree, ICmsClass NodeClass, ICmsSite Site, FormInfo OldFormInfo, FormInfo NewFormInfo, int? DocumentId) : ISourceObjectContext;
