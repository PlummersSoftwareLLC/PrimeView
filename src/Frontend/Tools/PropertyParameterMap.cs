using PrimeView.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Tools
{
	public static class PropertyParameterMap
	{
		private static readonly Dictionary<string, string> map;

		static PropertyParameterMap()
		{
			string result = nameof(Result);
			string reportSummary = nameof(ReportSummary);

			map = new()
			{
				{ $"{result}.{nameof(Result.Implementation)}", "im" },
				{ $"{result}.{nameof(Result.Solution)}", "so" },
				{ $"{result}.{nameof(Result.Label)}", "la" },
				{ $"{result}.{nameof(Result.Passes)}", "ps" },
				{ $"{result}.{nameof(Result.Duration)}", "du" },
				{ $"{result}.{nameof(Result.Threads)}", "td" },
				{ $"{result}.{nameof(Result.Algorithm)}", "al" },
				{ $"{result}.{nameof(Result.IsFaithful)}", "ff" },
				{ $"{result}.{nameof(Result.Bits)}", "bt" },
				{ $"{result}.{nameof(Result.PassesPerSecond)}", "pp" },
				{ $"{result}.{nameof(Result.IsMultiThreaded)}", "mt" },
				{ $"{reportSummary}.{nameof(ReportSummary.Date)}", "dt" },
				{ $"{reportSummary}.{nameof(ReportSummary.User)}", "us" },
				{ $"{reportSummary}.{nameof(ReportSummary.CpuVendor)}", "cv" },
				{ $"{reportSummary}.{nameof(ReportSummary.CpuBrand)}", "cb" },
				{ $"{reportSummary}.{nameof(ReportSummary.CpuCores)}", "cc" },
				{ $"{reportSummary}.{nameof(ReportSummary.CpuProcessors)}", "cp" },
				{ $"{reportSummary}.{nameof(ReportSummary.OsPlatform)}", "op" },
				{ $"{reportSummary}.{nameof(ReportSummary.OsRelease)}", "or" },
				{ $"{reportSummary}.{nameof(ReportSummary.Architecture)}", "ar" },
				{ $"{reportSummary}.{nameof(ReportSummary.IsSystemVirtual)}", "sv" },
				{ $"{reportSummary}.{nameof(ReportSummary.DockerArchitecture)}", "da" },
				{ $"{reportSummary}.{nameof(ReportSummary.ResultCount)}", "rc" }
			};
		}

		public static string GetPropertyParameterName<T>(string propertyName)
		{
			if (propertyName == null)
				return null;
			
			string name = $"{typeof(T).Name}.{propertyName}";

			return map.ContainsKey(name) ? map[name] : null;
		}

		public static string GetPropertyParameterName<T>(this Expression<Func<T, object>> expression)
		{
			if (expression == null)
				return null;

			if (expression.Body is not MemberExpression body)
			{
				UnaryExpression ubody = (UnaryExpression)expression.Body;
				body = ubody.Operand as MemberExpression;
			}

			MemberInfo memberInfo = body?.Member;

			if (memberInfo == null)
				return null;

			string name = $"{memberInfo.DeclaringType.Name}.{memberInfo.Name}";

			return map.ContainsKey(name) ? map[name] : null;
		}
	}
}
