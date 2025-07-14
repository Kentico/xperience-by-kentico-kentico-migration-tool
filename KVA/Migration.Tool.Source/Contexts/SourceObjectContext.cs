using CMS.FormEngine;
using Migration.Tool.KXP.Api.Services.CmsClass;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Contexts;

public record DocumentSourceObjectContext(ICmsTree CmsTree, ICmsClass NodeClass, ICmsSite Site, FormInfo OldFormInfo, FormInfo NewFormInfo, int? DocumentId) : ISourceObjectContext;

/// <param name="UniqueKey">String key unique per combination [custom table, table row]. Not applied when the context-dependent operation doesn't work with individual rows</param>
public record CustomTableSourceObjectContext(string UniqueKey) : ISourceObjectContext;
