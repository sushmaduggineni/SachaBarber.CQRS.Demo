﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CQRSalad.EventSourcing.Specifications
{
    public abstract class AggregateSpecification<TAggregate> where TAggregate : IAggregateRoot, new()
    {
        public virtual IEnumerable<IEvent> Given()
        {
            return new IEvent[0];
        }

        public abstract ICommand When();

        public abstract IEnumerable<IEvent> Expected();

        public SpecificationResult Verify()
        {
            var aggregate = new TAggregate();

            List<IEvent> givenEvents = Given().ToList();
            if (givenEvents.Count > 0)
            {
                aggregate.Version = givenEvents.Count;
                aggregate.Reel(givenEvents);
            }

            ICommand command = When();
            if (command == null)
            {
                throw new InvalidOperationException("No command provided.");
            }

            aggregate.Perform(command);
            var obtainedEvents = aggregate.Changes;

            List<IEvent> expectedEvents = Expected().ToList();
            if (expectedEvents == null || expectedEvents.Count < 1)
            {
                throw new InvalidOperationException("No expected events provided.");
            }

            return new SpecificationResult
            {
                Given = givenEvents,
                Expected = expectedEvents,
                Obtained = obtainedEvents
            };
        }
    }

    public sealed class SpecificationResult
    {
        public List<IEvent> Given { get; set; }
        public List<IEvent> Expected { get; set; }
        public List<IEvent> Obtained { get; set; }
    }
}
