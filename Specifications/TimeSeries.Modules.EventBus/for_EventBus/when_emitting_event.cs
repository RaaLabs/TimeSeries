/*---------------------------------------------------------------------------------------------
 *  Copyright (c) RaaLabs. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;
using System.Collections.Generic;
using Dolittle.Serialization.Json;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;
using System.Text;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Types;
using System.Collections;
using Dolittle.Lifecycle;
using System;

namespace RaaLabs.TimeSeries.Modules.EventBus.for_EventBus
{

    public class when_emitting_event : given.an_EventBus<Emitters<SomethingHappenedEmitter>, Consumers<SomethingHappenedConsumer, SomethingElseHappenedConsumer, SomethingHappenedButOtherConsumer>>
    {
        Because of = () =>
        {
            var @event = new SomethingHappened();
            _emitters.SomethingHappenedEmitter.Trigger(@event);
        };

        It should_propagate_to_the_correct_consumers = () =>
        {
            _consumers.SomethingHappenedConsumer.SomethingHappenedCalled.ShouldEqual(1);
            _consumers.SomethingHappenedButOtherConsumer.SomethingHappenedCalled.ShouldEqual(1);
        };

        It should_not_propagate_to_the_wrong_consumers = () =>
        {
            _consumers.SomethingElseHappenedConsumer.SomethingElseHappenedCalled.ShouldEqual(0);
        };
    }

    [Singleton]
    public class SomethingHappenedEmitter : IEmitEvent<SomethingHappened>
    {
        public event Action<SomethingHappened> SomethingHappened;


        public void Trigger(SomethingHappened @event)
        {
            SomethingHappened(@event);
        }
    }

    [Singleton]
    public class SomethingHappenedConsumer : IConsumeEvent<SomethingHappened>
    {
        public int SomethingHappenedCalled { get; set; }

        public async Task Consume(SomethingHappened data)
        {
            SomethingHappenedCalled++;
            await Task.CompletedTask;
        }
    }

    [Singleton]
    public class SomethingElseHappenedConsumer: IConsumeEvent<SomethingElseHappened>
    {
        public int SomethingElseHappenedCalled { get; set; }

        public async Task Consume(SomethingElseHappened data)
        {
            SomethingElseHappenedCalled++;
            await Task.CompletedTask;
        }
    }

    [Singleton]
    public class SomethingHappenedButOtherConsumer : IConsumeEvent<SomethingHappened>
    {
        public int SomethingHappenedCalled { get; set; }

        public async Task Consume(SomethingHappened data)
        {
            SomethingHappenedCalled++;
            await Task.CompletedTask;
        }
    }


    public class SomethingHappened { }
    public class SomethingElseHappened { }

    public class Emitters<T>
    {
        public T SomethingHappenedEmitter { get; set; }
    }
    public class Consumers<T, U, V>
    {
        public T SomethingHappenedConsumer { get; set; }
        public U SomethingElseHappenedConsumer { get; set; }
        public V SomethingHappenedButOtherConsumer { get; set; }
    }

}