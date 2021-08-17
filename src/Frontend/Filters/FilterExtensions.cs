using PrimeView.Entities;
using PrimeView.Frontend.Pages;
using PrimeView.Frontend.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PrimeView.Frontend.Filters
{
	public static class FilterExtensions
	{
		private static readonly Func<Result, bool> successfulResult;

		static FilterExtensions()
		{
			Expression<Func<Result, bool>> successfulResultExpression = result => result.Status == null || result.Status == "success";
			successfulResult = successfulResultExpression.Compile();
		}

		public static IEnumerable<Result> Viewable(this IEnumerable<Result> source)
		{
			return source.Where(successfulResult);
		}

		public static IEnumerable<Result> ApplyFilters(this IEnumerable<Result> source, ReportDetails page)
		{
			var filterImplementations = page.FilterImplementations;

			if (filterImplementations.Count > 0)
				source = source.Where(r => filterImplementations.Contains(r.Implementation));

			var filteredResults = source.Where(r =>
				r.IsMultiThreaded switch
				{
					true => page.FilterParallelMultithreaded,
					_ => page.FilterParallelSinglethreaded
				}
				&& r.Algorithm switch
				{
					"base" => page.FilterAlgorithmBase,
					"wheel" => page.FilterAlgorithmWheel,
					_ => page.FilterAlgorithmOther
				}
				&& r.IsFaithful switch
				{
					true => page.FilterFaithful,
					_ => page.FilterUnfaithful
				}
				&& r.Bits switch
				{
					null => page.FilterBitsUnknown,
					1 => page.FilterBitsOne,
					_ => page.FilterBitsOther
				}
			);

			return page.OnlyHighestPassesPerSecondPerThreadPerLanguage
				? filteredResults
					.GroupBy(r => r.Implementation)
					.SelectMany(group => group.Where(r => r.PassesPerSecond == group.Max(r => r.PassesPerSecond)))
				: filteredResults;
		}

		public static string CreateSummary(this IFilterPropertyProvider filter, ILanguageInfoProvider languageInfoProvider)
		{
			List<string> segments = new();

			segments.Add(filter.FilterImplementations.Count switch
			{
				0 => "all languages",
				1 => $"{languageInfoProvider.GetLanguageInfo(filter.FilterImplementations[0]).Name}",
				_ => $"{filter.FilterImplementations.Count} languages"
			});

			segments.Add((filter.FilterParallelSinglethreaded, filter.FilterParallelMultithreaded) switch
			{
				(true, false) => "single-threaded",
				(false, true) => "multithreaded",
				_ => null
			});

			segments.Add((filter.FilterAlgorithmBase, filter.FilterAlgorithmWheel, filter.FilterAlgorithmOther) switch
			{
				(false, false, false) => null,
				(true, false, false) => "base algorithm",
				(false, true, false) => "wheel algorithm",
				(false, false, true) => "other algorithms",
				(true, true, true) => "all algorithms",
				_ => "multiple algorithms"
			});

			segments.Add((filter.FilterFaithful, filter.FilterUnfaithful) switch
			{
				(true, false) => "faithful",
				(false, true) => "unfaithful",
				_ => null
			});

			segments.Add((filter.FilterBitsOne, filter.FilterBitsOther, filter.FilterBitsUnknown) switch
			{
				(true, false, false) => "one bit",
				(false, true, false) => "multiple bits",
				(false, false, true) => "unknown bits",
				(true, true, false) => "known bits",
				(false, true, true) => "all but one bit",
				(true, false, true) => "one or unknown bits",
				_ => null
			});

			return string.Join(", ", segments.Where(s => s != null));
		}

		public static IList<string> SplitFilterValues(this string text)
		{
			return text.Split('~', StringSplitOptions.RemoveEmptyEntries);
		}

		public static string JoinFilterValues(this IEnumerable<string> values)
		{
			return string.Join('~', values);
		}
	}
}
