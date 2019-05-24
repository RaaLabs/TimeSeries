---
title: Modules
description: The home of the Dolittle TimeSeries Module support
keywords: TimeSeries
author: einari
weight: 1
---

This section describes all the support for TimeSeries Modules. How they work,
how to build new ones and how to get them deployed.

TimeSeries modules are modules that take part in a [time series pipeline]({{< relref concepts/time_series_pipeline >}}).
It can represent the beginning of a pipeline, the thing that generated a [tag data point]({{< relref concepts/tag_data_point >}})
or take part in the pipeline doing different tasks related to the data point.
It is however important to remember that it is primarily a data point oriented and optimized pipeline
and is not meant for hosting general purpose applications. In Dolittle terms, that is where
we have our core cloud [platform](/platform) and hosting experience and also our [edge](/edge) solutions.

## Microsoft IoT Edge

At the core sits
