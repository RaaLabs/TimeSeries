---
title: Tag Data Points
description: What are Tag Data Points
keywords: TimeSeries
author: einari
weight: 3
aliases: /timeseries/timeseries/concepts/tag_data_points
---
A `TagDataPoint` is the type that is expected to be the type coming out of the first module
at the beginning of a [time series pipeline]({{ relref time_series_pipeline }}).
To identify the source of the sampled value, there is a property called `source` which is
combined with `tag`. The `tag` is commonly known as the actual sensor source of a signal.
This identifier is something the Dolittle time series software is in general not using,
and there is therefor an [identity mapper](/timeseries/identitymapper) that
deals with translating it into what is identified as a [time series]({{< relref time_series >}});
a ubiquitous identifier.

Typically, if consuming this type, you can expect the following payload as JSON:

```json
{
    "source": "string",
    "tag": "tag",
    "value": number | string | any,
    "timestamp": "EPOCH in milliseconds"
}
```

{{% notice information %}}
Important to notice that the timestamp is in standard UNIX EPOCH time with
milliseconds granularity.
{{% /notice %}}
