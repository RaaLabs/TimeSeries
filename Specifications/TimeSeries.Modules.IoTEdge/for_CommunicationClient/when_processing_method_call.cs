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
using Microsoft.Azure.Devices.Client;

namespace RaaLabs.TimeSeries.Modules.IoTEdge.for_CommunicationClient
{
    public class when_processing_method_call
    {
        static byte[] payload = Encoding.UTF8.GetBytes("{\"payload\": \"hello\"}");
        static MethodRequest requestWithPayload = new MethodRequest("", payload);
        static MethodRequest requestWithoutPayload = new MethodRequest("");

        public class SomeData
        {
            public string payload;

            public virtual bool Equals(SomeData obj)
            {
                return payload == obj.payload;
            }
        }

        static Mock<ISerializer> serializer;
        static Mock<FunctionHandler<SomeData, SomeData>> processingFunctionWithParameter;
        static Mock<FunctionHandler<SomeData>> processingFunctionWithoutParameter;
        static Mock<ActionHandler> processingActionWithoutParameter;
        static Mock<ActionHandler<SomeData>> processingActionWithParameter;

        static MethodResponse functionResponseWithParameter;
        static MethodResponse functionResponseWithoutParameter;
        static MethodResponse actionResponseWithParameter;
        static MethodResponse actionResponseWithoutParameter;

        Establish context = () =>
        {
            serializer = new Mock<ISerializer>();
            serializer.Setup(_ => _.FromJson(typeof(SomeData), Encoding.UTF8.GetString(payload), SerializationOptions.CamelCase)).Returns(new SomeData() { payload = "hello" });
            serializer.Setup(_ => _.ToJson(Moq.It.IsAny<SomeData>(), SerializationOptions.CamelCase)).Returns("{\"payload\": \"Done!\"}");

            processingFunctionWithParameter = new Mock<FunctionHandler<SomeData, SomeData>>();
            processingFunctionWithoutParameter = new Mock<FunctionHandler<SomeData>>();
            processingActionWithParameter = new Mock<ActionHandler<SomeData>>();
            processingActionWithoutParameter = new Mock<ActionHandler>();

            processingFunctionWithParameter.Setup(_ => _(Moq.It.IsAny<SomeData>())).ReturnsAsync(new SomeData { payload = "Done!" });
            processingFunctionWithoutParameter.Setup(_ => _()).ReturnsAsync(new SomeData { payload = "Done!" });
        };

        Because of = () =>
        {
            functionResponseWithParameter = CommunicationClient.ProcessMethod(requestWithPayload, processingFunctionWithParameter.Object, serializer.Object).Await();
            functionResponseWithoutParameter = CommunicationClient.ProcessMethod(requestWithoutPayload, processingFunctionWithoutParameter.Object, serializer.Object).Await();

            actionResponseWithParameter = CommunicationClient.ProcessMethod(requestWithPayload, processingActionWithParameter.Object, serializer.Object).Await();
            actionResponseWithoutParameter = CommunicationClient.ProcessMethod(requestWithoutPayload, processingActionWithoutParameter.Object, serializer.Object).Await();
        };

        It should_return_a_valid_response_for_functions_with_parameters = () => functionResponseWithParameter.ShouldMatch(_ => _.Status == 200 && Encoding.UTF8.GetString(_.Result).Contains("Done!"));

        It should_return_a_valid_response_for_functions_without_parameters = () => functionResponseWithoutParameter.ShouldMatch(_ => _.Status == 200 && Encoding.UTF8.GetString(_.Result).Contains("Done!"));

        It should_return_a_valid_response_for_actions_with_parameters = () => actionResponseWithParameter.ShouldMatch(_ => _.Status == 200 && _.Result == null);

        It should_return_a_valid_response_for_actions_without_parameters = () => actionResponseWithoutParameter.ShouldMatch(_ => _.Status == 200 && _.Result == null);
    }
}