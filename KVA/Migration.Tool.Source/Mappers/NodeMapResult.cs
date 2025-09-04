using CMS.DataEngine;
using Migration.Tool.Source.Model;

namespace Migration.Tool.Source.Mappers;
public record NodeMapResult(ICmsTree Node, Guid ContentItemGuid, List<Guid> ContentItemDataGuids, DataClassInfo TargetClassInfo, List<(string fieldName, ICmsTree node)> ChildLinks);
