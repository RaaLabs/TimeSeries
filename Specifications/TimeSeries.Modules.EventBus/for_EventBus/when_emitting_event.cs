/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using System;
using RaaLabs.TimeSeries.Modules.EventBus.given;

namespace RaaLabs.TimeSeries.Modules.EventBus.for_EventBus
{

    public class when_emitting_event : given.an_EventBus<Emitters, Consumers>
    {
        Because of = () =>
        {
            var @event = new SomethingHappened();
            _emitters.SomethingHappenedEmitter.Trigger(@event);
            _emitters.SomethingHappenedEmitter.Trigger(@event);
            _emitters.SomethingHappenedEmitter.Trigger(@event);
        };

        It should_propagate_to_the_correct_consumers = () =>
        {
            _consumers.SomethingHappenedConsumer.Verify(_ => _.Consume(Moq.It.IsAny<SomethingHappened>()), Times.Exactly(3));
            _consumers.SomethingHappenedButOtherConsumer.Verify(_ => _.Consume(Moq.It.IsAny<SomethingHappened>()), Times.Exactly(3));
        };

        It should_not_propagate_to_the_wrong_consumers = () =>
        {
            _consumers.SomethingElseHappenedConsumer.Verify(_ => _.Consume(Moq.It.IsAny<SomethingElseHappened>()), Times.Never());
        };
    }

    public class SomethingHappenedEmitter : IEmitEvent<SomethingHappened>
    {
        public event Action<SomethingHappened> SomethingHappened;


        public void Trigger(SomethingHappened @event)
        {
            SomethingHappened(@event);
        }
    }

    public interface SomethingHappenedConsumer : IConsumeEvent<SomethingHappened> { }
    public interface SomethingHappenedButOtherConsumer : IConsumeEvent<SomethingHappened> { }
    public interface SomethingHappenedButInterfaceConsumer : IConsumeEvent<SomethingHappened> { }
    public interface SomethingElseHappenedConsumer : IConsumeEvent<SomethingElseHappened> { }

    public class SomethingHappened { }
    public class SomethingElseHappened { }

    public class Emitters : IUseEventBus<SomethingHappenedEmitter>
    {
        public SomethingHappenedEmitter SomethingHappenedEmitter { get; set; }
    }
    public class Consumers : IUseEventBus<Mock<SomethingHappenedConsumer>, Mock<SomethingElseHappenedConsumer>, Mock<SomethingHappenedButOtherConsumer>>
    {
        public Mock<SomethingHappenedConsumer> SomethingHappenedConsumer { get; set; }
        public Mock<SomethingElseHappenedConsumer> SomethingElseHappenedConsumer { get; set; }
        public Mock<SomethingHappenedButOtherConsumer> SomethingHappenedButOtherConsumer { get; set; }
    }

}