// namespace Migration.Toolkit.TestConsole;
//
// using System.Text;
// using Migration.Toolkit.Core.Services.CmsClass;
//
// public static class GenHelper
// {
//     public static void AppendFieldMappingDefinitionAsMarkdown(StringBuilder builder)
//     {
//         builder.AppendLine("|Source data type|Target data type|Source control|Target component|Actions|");
//         builder.AppendLine("|---|---|---|---|---|");
//         foreach (var (sourceDataType, value) in FieldMappingInstance.Default)
//         {
//             foreach (var (sourceComponent, targetComponent) in value.FormComponents)
//             {
//                 builder.AppendLine(
//                     $"|{string.Join("|", sourceDataType, value.TargetDataType, sourceComponent, targetComponent.TargetFormComponent, string.Join(",", targetComponent.Actions))}|");
//             }
//         }
//     }
// }