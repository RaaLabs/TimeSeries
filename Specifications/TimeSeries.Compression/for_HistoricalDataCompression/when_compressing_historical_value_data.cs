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

namespace RaaLabs.TimeSeries.Compression
{
    public class when_compressing_historical_value_data
    {
        private static Guid timeSeries = Guid.NewGuid();

        static IEnumerable<DataPoint<dynamic>> historicalData = Enumerable.Range(0, 100).Select(_ => new DataPoint<dynamic> { TimeSeries = timeSeries, Timestamp = (long)(_ * 100L + 100000L), Value = Math.Sin(_ * 0.1f) });
        static byte[] compressedData;
        static IEnumerable<DataPoint<dynamic>> uncompressedData;

        Establish context = () => { };

        Because of = () => {
            compressedData = HistoricalDataCompression.Compress(historicalData).ToArray();

            uncompressedData = HistoricalDataCompression.Decompress(compressedData).ToList();
        };

        It should_uncompress_the_correct_number_of_data_points = () => uncompressedData.Count().ShouldEqual(historicalData.Count());
        It should_not_corrupt_data_values_during_compression = () => uncompressedData.Zip(historicalData, (result, historical) => Math.Abs(result.Value - historical.Value) < 0.0001).All(_ => _).ShouldBeTrue();
        It should_not_corrupt_data_timestamps_during_compression = () => uncompressedData.Zip(historicalData, (result, historical) => result.Timestamp == historical.Timestamp).All(_ => _).ShouldBeTrue();
    }
}