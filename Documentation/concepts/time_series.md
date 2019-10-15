---
title: Time Series
description: What are time series
keywords: TimeSeries
author: einari
weight: 1
aliases: /timeseries/timeseries/concepts/time_series
---
Time series is a series of data points listed in time order. Often, you'll find it to be a sequence
sampled at equally spaced points in time, often referred to as Hz or resolution of sampling.

A time series represent one source and one type of value, often referred to as a signal within the time series.
In many domains, these sources are identified as sensors that sample something and generate a
[data point]({{< relref data_points >}}). The data point has the value and time on it to
represent the value at the time it was sampled.

When sampled and sent through the [pipeline]({{< relref time_series_pipeline >}}) the shape will shift.
The goal of the pipeline is to eventually store the finished data point and be able to visualize on the
individual or combines time series and then gain insight and perform advanced analytics like machine
learning and similar.

Time series are uniquely identified by a [Guid](https://en.wikipedia.org/wiki/Universally_unique_identifier).
In the real world the senor, instrument or apparatus generating the data point needs to classified and
are often referred to as assets in an asset hierarchy. In Dolittle we call this a [topology]({{< relref topology >}})
and is not a direct hierarchy, but a graph.
