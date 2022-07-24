# Implementation notes

* This project provides a REST API reader implementation of the [`IReportReader`](../Entities/IReportReader.cs) interface. It loads benchmark reports from a REST API that's been developed and published for this purpose. 
* The reader lazily loads summaries (`Sessions` in the context of the API) as requested by its invoker.
* The API client is generated from the OpenAPI specification that is published by and for the API.
* The reader uses one configuration setting, which specifies the base URI of the API.