/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Types;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace RaaLabs.TimeSeries.Modules.for_InputHandlers
{
    public class when_initializing_with_one_data_point_handler_in_the_system
    {
        const string input_name = "SomeInput";
        public class SomeData { }
        public class MyHandler : ICanHandleDataPoint<SomeData>
        {
            public Input Input => input_name;

            public Task Handle(DataPoint<SomeData> value)
            {
                return Task.CompletedTask;
            }
        }

        static Mock<ICommunicationClient> communication_client;
        static Mock<ITypeFinder> type_finder;
        static Mock<IContainer> container;
        static InputHandlers input_handlers;

        Establish context = () =>
        {
            communication_client = new Mock<ICommunicationClient>();

            type_finder = new Mock<ITypeFinder>();
            type_finder.Setup(_ => _.FindMultiple(typeof(ICanHandleDataPoint<>))).Returns(new [] {  typeof(MyHandler) });

            container = new Mock<IContainer>();
            container.Setup(_ => _.Get(typeof(MyHandler))).Returns(new MyHandler());

            input_handlers = new InputHandlers(
                communication_client.Object,
                type_finder.Object,
                container.Object);
        };

        Because of = () => input_handlers.Initialize();

        It should_add_a_subscription_to_the_handle_method = () => 
            communication_client.Verify(_ => _
                .SubscribeTo<DataPoint<SomeData>>(input_name, Moq.It.IsAny<Subscriber<DataPoint<SomeData>>>())
            );
    }
}