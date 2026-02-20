using System.Runtime.CompilerServices;

namespace JakubKastner.Extensions;

public static class BoolExtensions
{
	public static string ToCssClass(this bool condition, string? trueClass = null, string? falseClass = null, [CallerArgumentExpression(nameof(condition))] string? conditionExpr = null)
	{
		trueClass ??= conditionExpr?.ToLower();
		falseClass ??= string.Empty;

		return $" {(condition ? trueClass : falseClass)}";
	}
}
