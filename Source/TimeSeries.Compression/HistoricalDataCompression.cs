using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Google.Protobuf;
using System.Threading.Tasks;

namespace RaaLabs.TimeSeries.Compression
{
    /// <summary>
    /// 
    /// </summary>
    public class HistoricalDataCompression
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static IEnumerable<byte> Compress(IEnumerable<DataPoint<dynamic>> dataPoints)
        {
            if (dataPoints.Select(_ => _.TimeSeries).Distinct().Count() > 1)
            {
                throw new Exception("Unable to compress data points: All data points must come from the same time series.");
            }

            var timeSeries = ByteString.CopyFrom(dataPoints.First().TimeSeries.Value.ToByteArray());
            var firstTimestamp = dataPoints.First().Timestamp.Value;
            var protobufDataPoints = dataPoints.Select(_ => _.ToProtobuf()).ToArray();

            var frame = new Protobuf.TimeSeriesFrame { TimeSeries = timeSeries, Timestamp = (ulong) firstTimestamp };

            var payload = new Protobuf.TimeSeriesPayload();
            payload = payload.Append(protobufDataPoints);
            frame.UncompressedPayload = payload;
            frame = frame.Compress();
            var encoded = frame.ToByteArray();

            byte[] length = BitConverter.GetBytes((int)encoded.Length);

            return length.Concat(encoded);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="dataPoints"></param>
        /// <param name="maxFrameSize"></param>
        /// <returns></returns>
        public static IEnumerable<byte> Append(IEnumerable<byte> payload, IEnumerable<DataPoint<dynamic>> dataPoints, int maxFrameSize = 100_000)
        {
            var distinctTimeSeries = dataPoints.Select(_ => _.TimeSeries).Distinct();
            if (distinctTimeSeries.Count() > 1)
            {
                throw new Exception("Unable to compress data points: All data points must come from the same time series.");
            }
            var timeSeries = distinctTimeSeries.First();

            Protobuf.TimeSeriesFrame parsedFrame;

            var lastFrame = LastFrame(payload, out int offset);
            var lastFrameSize = BitConverter.ToInt32(lastFrame.Take(4).ToArray(), 0);
            if (lastFrameSize < maxFrameSize || maxFrameSize < 0)
            {
                parsedFrame = Protobuf.TimeSeriesFrame.Parser.ParseFrom(lastFrame.Skip(4).ToArray()).Uncompress();
                if (timeSeries.Value != new Guid(parsedFrame.TimeSeries.ToByteArray()))
                {
                    throw new Exception("Unable to compress data points: New and old data points are not from the same source.");
                }

                parsedFrame.UncompressedPayload = parsedFrame.UncompressedPayload.Append(dataPoints.Select(_ => _.ToProtobuf()));

                var newLastFrame = parsedFrame.Compress().ToByteArray();
                return payload.Take(offset)
                    .Concat(BitConverter.GetBytes(newLastFrame.Length))
                    .Concat(newLastFrame);
            }
            else
            {
                var newFrame = Compress(dataPoints);
                return payload
                    .Concat(newFrame);
            }
        }

        /// <summary>
        /// Decompress a byte array into its individual data points.
        /// </summary>
        /// <param name="payload">the byte array containing data points</param>
        /// <returns>A list of the data points in the byte array</returns>
        public static IEnumerable<DataPoint<dynamic>> Decompress(IEnumerable<byte> payload)
        {
            var dataFrames = UnpackedMessages(payload);
            var dataPoints = dataFrames
                .Select(frame => Protobuf.TimeSeriesFrame.Parser.ParseFrom(frame))
                .Select(compressedFrame => compressedFrame.Uncompress())
                .SelectMany(frame => frame.DataPoints());

            return dataPoints;
        }

        /// <summary>
        /// Split data point byte array into data frames.
        /// 
        /// Data point byte arrays are packed into one big byte array, encoded in the following manner:
        /// [frame_size_0 (4 bytes)][payload_0 (frame_size_0 bytes)][frame_size_1 (4 bytes)][payload_1 (frame_size_1 bytes)][...]
        /// </summary>
        /// <param name="packedMessages"></param>
        /// <returns></returns>
        private static IEnumerable<byte[]> UnpackedMessages(IEnumerable<byte> packedMessages)
        {
            while (packedMessages.Any())
            {
                var messageLength = BitConverter.ToInt32(packedMessages.Take(4).ToArray(), 0);
                if (messageLength == 0) throw new Exception("Message length was 0, which should never happen.");
                var message = packedMessages.Skip(4).Take(messageLength).ToArray();
                packedMessages = packedMessages.Skip(4 + messageLength);

                yield return message;
            }
        }

        /// <summary>
        /// Get the last frame of the payload
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static IEnumerable<byte> LastFrame(IEnumerable<byte> payload, out int offset)
        {
            var current = payload;
            offset = 0;
            while (true)
            {
                var frameSize = BitConverter.ToInt32(current.Take(4).ToArray(), 0);
                var newCurrent = current.Skip(frameSize + 4);
                if (newCurrent.Any())
                {
                    current = newCurrent;
                    offset += frameSize + 4;
                }
                else
                {
                    return current;
                }
            }
        }
    }
}
