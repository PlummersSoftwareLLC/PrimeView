﻿using PrimeView.Entities;
using PrimeView.Frontend.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeView.Frontend.Tools
{
	public static class FilterExtensions
	{
		public static IEnumerable<Result> ApplyFilters(this IEnumerable<Result> source, ReportDetails page)
		{
			var filterImplementations = page.FilterImplementations;

			if (filterImplementations.Count > 0)
				source = source.Where(r => filterImplementations.Contains(r.Implementation));

			return source.Where(r => 
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
		}
	}
}