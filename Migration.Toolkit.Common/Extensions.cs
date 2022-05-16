using System.Linq.Expressions;

namespace Migration.Toolkit.Common;

public static class Extensions
{
    public static bool IsIn<T>(this T value, params T[] values) => values.Contains(value);
    
    public static string GetMemberName<T>(this Expression<Func<T,object>> expr)
    {
        var stack = new Stack<string>();

        var me = expr.Body.NodeType switch
        {
            ExpressionType.Convert => (expr.Body is UnaryExpression ue ? ue.Operand : null) as MemberExpression,
            ExpressionType.ConvertChecked => (expr.Body is UnaryExpression ue ? ue.Operand : null) as MemberExpression,
            _ => expr.Body as MemberExpression
        };
        
        while (me != null)
        {
            stack.Push(me.Member.Name);
            me = me.Expression as MemberExpression;
        }
        
        return string.Join(".", stack.ToArray());
    }
}