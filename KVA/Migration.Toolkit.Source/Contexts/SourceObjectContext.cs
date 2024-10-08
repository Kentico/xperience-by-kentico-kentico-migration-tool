using CMS.FormEngine;
using Migration.Toolkit.KXP.Api.Services.CmsClass;
using Migration.Toolkit.Source.Model;

namespace Migration.Toolkit.Source.Contexts;

public record DocumentSourceObjectContext(ICmsTree CmsTree, ICmsClass NodeClass, ICmsSite Site, FormInfo OldFormInfo, FormInfo NewFormInfo, int? DocumentId) : ISourceObjectContext;
