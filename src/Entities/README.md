# Implementation notes

* The [`IReportReader`](IReportReader.cs) interface defines the "contract" between the web front-end (as implemented in the [Frontend](../Frontend) project), and the back-ends that collect the actual data to show (as currently implemented by the [JsonFileReader](../JsonFileReader) project).
* The root classes used within `IReportReader` are [`ReportSummary`](ReportSummary.cs) and [`Report`](Report.cs) for the report overview and report detail pages, respectively. All other classes are embedded within `Report` instances.