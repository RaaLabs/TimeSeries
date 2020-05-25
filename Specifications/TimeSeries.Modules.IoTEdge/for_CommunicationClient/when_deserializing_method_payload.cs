/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using Dolittle.Serialization.Json;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using System.Text;

namespace RaaLabs.TimeSeries.Modules.IoTEdge.for_CommunicationClient
{
    public class when_deserializing_method_payload
    {
        static byte[] payload = Encoding.UTF8.GetBytes("{\"payload\": \"hello\"}");

        public class SomeData
        {
            public string payload;

            public virtual bool Equals(SomeData obj)
            {
                return payload == obj.payload;
            }
        }

        static Mock<ISerializer> serializer;

        static object[] argumentsForSomeDataPayload;
        static object[] argumentsForNullPayload;

        Establish context = () =>
        {
            serializer = new Mock<ISerializer>();
            serializer.Setup(_ => _.FromJson(typeof(SomeData), Encoding.UTF8.GetString(payload), SerializationOptions.CamelCase)).Returns(new SomeData { payload = "hello" });

            argumentsForSomeDataPayload = CommunicationClient.DeserializeMethodPayload(typeof(SomeData), payload, serializer.Object).ToArray();
            argumentsForNullPayload = CommunicationClient.DeserializeMethodPayload(null, payload, serializer.Object).ToArray();
        };

        It should_deserialize_to_type_if_not_null = () => argumentsForSomeDataPayload.Select(_ => ((SomeData)_).payload).ShouldEqual(new string[] { "hello" });

        It should_deserialize_to_empty_array_if_null = () => argumentsForNullPayload.ShouldEqual(new object[] { });
    }
}