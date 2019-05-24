---
title: Data Points
description: What are Data Points
keywords: TimeSeries
author: einari
weight: 4
---
A `DataPoint` is an identified data point. Usually this is the result of
having a [†ag data point]({{< relref tag_data_point >}}) being processed by
a our [identity mapper](/timeseries/identitymapper) and coming out as this.

The purpose of this new representation is to make the data point more focused
and unambiguous in the system with the unique identifier that represents the
[time series]({{< relref time_series >}}).

Typically, if consuming this type, you can expect the following payload as JSON:

```json
{
    "timeSeries": "GUID",
    "value": number | string | any,
    "timestamp": "EPOCH in milliseconds"
}
```

{{% notice information %}}
Important to notice that the timestamp is in standard UNIX EPOCH time with
milliseconds granularity.
{{% /notice %}}
