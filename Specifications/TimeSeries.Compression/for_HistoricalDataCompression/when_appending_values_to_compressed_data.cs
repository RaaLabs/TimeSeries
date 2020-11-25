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
    public class when_appending_values_to_compressed_data
    {
        private static Guid timeSeries = Guid.NewGuid();

        static IEnumerable<DataPoint<dynamic>> historicalData = Enumerable.Range(0, 100).Select(_ => new DataPoint<dynamic> { TimeSeries = timeSeries, Timestamp = (long)(_ * 100L + 100000L), Value = Math.Sin(_ * 0.1f) });
        static IEnumerable<byte> compressedDataSingleFrame;
        static IEnumerable<byte> compressedDataMultipleFrames;
        static IEnumerable<DataPoint<dynamic>> uncompressedDataSingleFrame;
        static IEnumerable<DataPoint<dynamic>> uncompressedDataMultipleFrames;

        static IEnumerable<int> frameSizesForSingleFrame;
        static IEnumerable<int> frameSizesForMultipleFrames;

        Establish context = () => { };

        Because of = () => {
            var perBatch = 20;
            var firstBatch = historicalData.Take(perBatch);
            var secondBatch = historicalData.Skip(perBatch).Take(perBatch);
            var thirdBatch = historicalData.Skip(2*perBatch).Take(perBatch);
            var fourthBatch = historicalData.Skip(3*perBatch).Take(perBatch);
            var fifthBatch = historicalData.Skip(4*perBatch);

            compressedDataSingleFrame = HistoricalDataCompression.Compress(firstBatch);
            compressedDataSingleFrame = HistoricalDataCompression.Append(compressedDataSingleFrame, secondBatch, -1);
            compressedDataSingleFrame = HistoricalDataCompression.Append(compressedDataSingleFrame, thirdBatch, -1);
            compressedDataSingleFrame = HistoricalDataCompression.Append(compressedDataSingleFrame, fourthBatch, -1);
            compressedDataSingleFrame = HistoricalDataCompression.Append(compressedDataSingleFrame, fifthBatch, -1);

            var firstBatchFrameSize = HistoricalDataCompression.Compress(firstBatch).Count();
            var secondBatchFrameSize = HistoricalDataCompression.Compress(secondBatch).Count();
            var thirdBatchFrameSize = HistoricalDataCompression.Compress(thirdBatch).Count();
            var fourthBatchFrameSize = HistoricalDataCompression.Compress(fourthBatch).Count();
            var fifthBatchFrameSize = HistoricalDataCompression.Compress(fifthBatch).Count();

            compressedDataMultipleFrames = HistoricalDataCompression.Compress(firstBatch);
            compressedDataMultipleFrames = HistoricalDataCompression.Append(compressedDataMultipleFrames, secondBatch, firstBatchFrameSize - 5);
            compressedDataMultipleFrames = HistoricalDataCompression.Append(compressedDataMultipleFrames, thirdBatch, secondBatchFrameSize - 5);
            compressedDataMultipleFrames = HistoricalDataCompression.Append(compressedDataMultipleFrames, fourthBatch, thirdBatchFrameSize - 5);
            compressedDataMultipleFrames = HistoricalDataCompression.Append(compressedDataMultipleFrames, fifthBatch, fourthBatchFrameSize - 5);

            compressedDataSingleFrame = compressedDataSingleFrame.ToArray();
            compressedDataMultipleFrames = compressedDataMultipleFrames.ToArray();

            uncompressedDataSingleFrame = HistoricalDataCompression.Decompress(compressedDataSingleFrame).ToList();
            uncompressedDataMultipleFrames = HistoricalDataCompression.Decompress(compressedDataMultipleFrames).ToList();

            frameSizesForSingleFrame = FrameSizes(compressedDataSingleFrame).ToList();
            frameSizesForMultipleFrames = FrameSizes(compressedDataMultipleFrames).ToList();
        };

        It should_not_lose_data_when_appending_to_frame = () => uncompressedDataSingleFrame.Count().ShouldEqual(historicalData.Count());
        It should_not_lose_data_when_creating_new_frames = () => uncompressedDataMultipleFrames.Count().ShouldEqual(historicalData.Count());

        It should_keep_appending_to_the_same_frame_for_single_frame = () => frameSizesForSingleFrame.ShouldEqual(new List<int> { compressedDataSingleFrame.Count() - 4 });
        It should_create_five_frames_for_multiple_frames = () => frameSizesForMultipleFrames.Count().ShouldEqual(5);

        private static IEnumerable<int> FrameSizes(IEnumerable<byte> payload)
        {
            while (payload.Any())
            {
                int frameSize = BitConverter.ToInt32(payload.Take(4).ToArray(), 0);
                yield return frameSize;

                payload = payload.Skip(4 + frameSize);
            }
        }
    }
}