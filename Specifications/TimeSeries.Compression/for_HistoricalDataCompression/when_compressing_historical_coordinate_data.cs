/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using Machine.Specifications;
using System.IO;
using Moq;
using It = Machine.Specifications.It;
using System.Text;
using System.Collections.Generic;
using System;
using RaaLabs.TimeSeries.DataTypes;

namespace RaaLabs.TimeSeries.Compression
{
    public class when_compressing_historical_coordinate_data
    {
        private static Guid timeSeries = Guid.NewGuid();

        static IEnumerable<DataPoint<Coordinate>> historicalData = Enumerable.Range(0, 100).Select(_ => new DataPoint<Coordinate> { TimeSeries = timeSeries, Timestamp = (long)(_ * 100L + 100000L), Value = new Coordinate { Longitude = new Measurement<float> { Value = MathF.Sin(_ * 0.1f) }, Latitude = new Measurement<float> { Value = MathF.Cos(_ * 0.1f) }  } });
        static byte[] compressedData;
        static IEnumerable<DataPoint<Coordinate>> uncompressedData;

        Establish context = () => { };

        Because of = () => {
            compressedData = HistoricalDataCompression.Compress(historicalData.Select(_ => new DataPoint<dynamic> { Value = _.Value, TimeSeries = _.TimeSeries, Timestamp = _.Timestamp })).ToArray();

            uncompressedData = HistoricalDataCompression.Decompress(compressedData)
                .Where(_ => _.Value is Coordinate)
                .Select(_ => new DataPoint<Coordinate> { Value = _.Value as Coordinate, TimeSeries = _.TimeSeries, Timestamp = _.Timestamp })
                .ToList();
        };

        It should_uncompress_the_correct_number_of_data_points = () => uncompressedData.Count().ShouldEqual(historicalData.Count());

        It should_not_corrupt_longitudes_during_compression = () => uncompressedData.Zip(historicalData, (result, historical) => Math.Abs(result.Value.Longitude.Value - historical.Value.Longitude.Value) < 0.0001).All(_ => _).ShouldBeTrue();

        It should_not_corrupt_latitudes_during_compression = () => uncompressedData.Zip(historicalData, (result, historical) => Math.Abs(result.Value.Latitude.Value - historical.Value.Latitude.Value) < 0.0001).All(_ => _).ShouldBeTrue();
    }
}