using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Google.Protobuf;
using RaaLabs.TimeSeries.DataTypes;

namespace RaaLabs.TimeSeries.Compression
{
    /// <inheritdoc/>
    public static class ProtobufExtensions
    {
        /// <inheritdoc/>
        public static Protobuf.DataPoint ToProtobuf(this DataPoint<dynamic> dataPoint)
        {
            ulong timestamp = (ulong)dataPoint.Timestamp.Value;
            var timeSeries = ByteString.CopyFrom(dataPoint.TimeSeries.Value.ToByteArray());
            dynamic value = dataPoint.Value;
            Type valueType = value.GetType();
            Type coordinateType = typeof(Coordinate);

            if (dataPoint.Value is double || dataPoint.Value is float)
            {
                return new Protobuf.DataPoint { TimeSeries = timeSeries, Timestamp = timestamp, Value = (float)dataPoint.Value };
            }
            else if (dataPoint.Value is Coordinate)
            {
                Coordinate coordinate = dataPoint.Value as Coordinate;
                Protobuf.Coordinate protobufCoordinate = new Protobuf.Coordinate { Longitude = coordinate.Longitude.Value, Latitude = coordinate.Latitude.Value };
                return new Protobuf.DataPoint
                {
                    TimeSeries = timeSeries,
                    Timestamp = timestamp,
                    Coordinate = protobufCoordinate
                };
            }
            else if (dataPoint.Value is JObject)
            {
                // For now, the only compex data structure is Coordinate. For this reason, this is the only data type we try to parse as.
                JObject jObject = dataPoint.Value as JObject;
                Protobuf.Coordinate coordinate = jObject.ToObject<Protobuf.Coordinate>();
                return new Protobuf.DataPoint
                {
                    TimeSeries = timeSeries,
                    Timestamp = timestamp,
                    Coordinate = coordinate
                };
            }
            else
            {
                return new Protobuf.DataPoint { };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        public static DataPoint<dynamic> FromProtobuf(this Protobuf.DataPoint dataPoint)
        {
            if (dataPoint == null) return null;
            var timeSeries = new Guid(dataPoint.TimeSeries.ToArray());

            switch (dataPoint.MeasurementCase)
            {
                case Protobuf.DataPoint.MeasurementOneofCase.Value:
                    return new DataPoint<dynamic> { TimeSeries = timeSeries, Timestamp = (long)dataPoint.Timestamp, Value = dataPoint.Value };
                case Protobuf.DataPoint.MeasurementOneofCase.Coordinate:
                    return new DataPoint<dynamic> { TimeSeries = timeSeries, Timestamp = (long)dataPoint.Timestamp, Value = dataPoint.Coordinate };
                case Protobuf.DataPoint.MeasurementOneofCase.None:
                default:
                    return null;
            }
        }

        /// <inheritdoc/>
        public static IEnumerable<DataPoint<dynamic>> DataPoints(this Protobuf.TimeSeriesFrame frame)
        {
            var timeSeriesId = new Guid(frame.TimeSeries.ToByteArray());
            frame = frame.Uncompress();
            var timestamps = frame.UncompressedPayload.Timestep
                .Aggregate((IEnumerable<long>)new List<long> { (long)frame.Timestamp }, (offsets, step) => offsets.Append(offsets.Last() + (long)step))
                .Skip(1);
            var values = frame.UncompressedPayload.Values?.Value;
            var longitudes = frame.UncompressedPayload.Coordinates?.Longitudes;
            var latitudes = frame.UncompressedPayload.Coordinates?.Latitudes;
            var coordinates = longitudes?.Zip(latitudes, (longitude, latitude) => new Coordinate { Longitude = new Measurement<float> { Value = longitude }, Latitude = new Measurement<float> { Value = latitude } });

            IEnumerable<DataPoint<dynamic>> dataPoints;
            switch (frame.UncompressedPayload.PayloadCase)
            {
                case Protobuf.TimeSeriesPayload.PayloadOneofCase.Values:
                    dataPoints = timestamps.Zip(values, (timestamp, value) => new DataPoint<dynamic> { TimeSeries = timeSeriesId, Timestamp = timestamp, Value = value });
                    break;
                case Protobuf.TimeSeriesPayload.PayloadOneofCase.Coordinates:
                    dataPoints = timestamps.Zip(coordinates, (timestamp, coordinate) => new DataPoint<dynamic> { TimeSeries = timeSeriesId, Timestamp = timestamp, Value = coordinate });
                    break;
                default:
                    throw new Exception();
            }

            return dataPoints;
        }

        /// <inheritdoc/>
        public static Protobuf.TimeSeriesPayload Append(this Protobuf.TimeSeriesPayload payload, IEnumerable<Protobuf.DataPoint> dataPoints)
        {
            payload = new Protobuf.TimeSeriesPayload(payload);
            var originTimestamp = (payload.Timestep.Count > 0) ? payload.LastTimestamp : dataPoints.First().Timestamp;
            var timeOffsets = dataPoints.Select(_ => _.Timestamp).TimestampToStepLength(originTimestamp).ToList();
            payload.Timestep.AddRange(timeOffsets);

            var values = dataPoints.Where(_ => _.MeasurementCase == Protobuf.DataPoint.MeasurementOneofCase.Value).Select(_ => _.Value);
            var longitudes = dataPoints.Where(_ => _.MeasurementCase == Protobuf.DataPoint.MeasurementOneofCase.Coordinate).Select(_ => _.Coordinate.Longitude);
            var latitudes = dataPoints.Where(_ => _.MeasurementCase == Protobuf.DataPoint.MeasurementOneofCase.Coordinate).Select(_ => _.Coordinate.Latitude);

            if (payload.PayloadCase == Protobuf.TimeSeriesPayload.PayloadOneofCase.None)
            {
                switch (dataPoints.First().MeasurementCase)
                {
                    case Protobuf.DataPoint.MeasurementOneofCase.Value:
                        payload.Values = new Protobuf.TimeSeriesPayload.Types.Values();
                        break;
                    case Protobuf.DataPoint.MeasurementOneofCase.Coordinate:
                        payload.Coordinates = new Protobuf.TimeSeriesPayload.Types.Coordinates();
                        break;
                }
            }

            payload.Values?.Value?.AddRange(values);
            payload.Coordinates?.Longitudes?.AddRange(longitudes);
            payload.Coordinates?.Latitudes?.AddRange(latitudes);
            payload.LastTimestamp = dataPoints.Last().Timestamp;

            return payload;
        }

        /// <inheritdoc/>
        public static IEnumerable<UInt64> TimestampToStepLength(this IEnumerable<ulong> times, ulong start)
        {
            ulong previous = start;
            while (times.Any())
            {
                var thisTime = times.First();
                var thisOffset = thisTime - previous;
                previous = thisTime;
                times = times.Skip(1);

                yield return thisOffset;
            }
        }

        /// <inheritdoc/>
        public static Protobuf.TimeSeriesFrame Uncompress(this Protobuf.TimeSeriesFrame frame)
        {
            Protobuf.TimeSeriesPayload payload;
            switch (frame.PayloadCase)
            {
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.CompressedPayload:
                    payload = Protobuf.TimeSeriesPayload.Parser.ParseFrom(Ionic.Zlib.ZlibStream.UncompressBuffer(frame.CompressedPayload.ToArray()));
                    break;
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.UncompressedPayload:
                    payload = frame.UncompressedPayload;
                    break;
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.None:
                    payload = null;
                    break;
                default:
                    throw new Exception();
            }

            return new Protobuf.TimeSeriesFrame(frame)
            {
                UncompressedPayload = payload
            };
        }

        /// <inheritdoc/>
        public static Protobuf.TimeSeriesFrame Compress(this Protobuf.TimeSeriesFrame frame)
        {
            ByteString payload;
            switch (frame.PayloadCase)
            {
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.UncompressedPayload:
                    payload = ByteString.CopyFrom(Ionic.Zlib.ZlibStream.CompressBuffer(frame.UncompressedPayload.ToByteArray()));
                    break;
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.CompressedPayload:
                    payload = frame.CompressedPayload;
                    break;
                case Protobuf.TimeSeriesFrame.PayloadOneofCase.None:
                    payload = null;
                    break;
                default:
                    throw new Exception();
            }

            return new Protobuf.TimeSeriesFrame(frame)
            {
                CompressedPayload = payload
            };
        }
    }
}
