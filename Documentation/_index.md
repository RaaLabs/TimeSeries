---
title: Modules
description: The home of the Dolittle TimeSeries Module support
keywords: TimeSeries
author: einari
weight: 1
---

This section describes all the support for TimeSeries Modules. How they work,
how to build new ones and how to get them deployed.

TimeSeries modules are modules that take part in a [time series pipeline]({{< relref "concepts/time_series_pipeline" >}}).
It can represent the beginning of a pipeline, the thing that generated a [tag data point]({{< relref "concepts/tag_data_points" >}})
or take part in the pipeline doing different tasks related to the data point.
It is however important to remember that it is primarily a data point oriented and optimized pipeline
and is not meant for hosting general purpose applications. In Dolittle terms, that is where
we have our core cloud [platform](/platform) and hosting experience and also our [edge](/edge) solutions.

## Who is this for

The Dolittle TimeSeries Module provide a consistent development experience in how we see software development, with
high focus on our [core principles](/contributing/guidelines/core_principles/) and [development principles](/contributing/guidelines/development_principles/).
It also follow 

## Microsoft IoT Edge

Dolittle TimeSeries modules are a thin layer on top of [Microsoft IoT Edge modules](https://docs.microsoft.com/en-us/azure/iot-edge/iot-edge-modules).
It provides an abstraction and simplification of how to work with time series which is somewhat more
opinionated with the goal of giving more consistency and be more predictable as a result.

## Standard data point shapes
