using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders
{
    internal class TupleBuilder : NonConformingBuilder
    {
        public override Type[] SupportedTypes => new Type[]
        {
            typeof(Tuple<>), typeof(ValueTuple<>),
            typeof(Tuple<,>), typeof(ValueTuple<,>),
            typeof(Tuple<,,>), typeof(ValueTuple<,,>),
            typeof(Tuple<,,,>), typeof(ValueTuple<,,,>),
            typeof(Tuple<,,,,>), typeof(ValueTuple<,,,,>),
            typeof(Tuple<,,,,,>), typeof(ValueTuple<,,,,,>),
            typeof(Tuple<,,,,,,>), typeof(ValueTuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>), typeof(ValueTuple<,,,,,,,>),
            typeof(KeyValuePair<,>)
        };

        public override int Repeat => 1;

        public override object CreateResult(Type requestType, object[][] innerResults)
        {
            return requestType.GetConstructor(requestType.GenericTypeArguments)
                    .Invoke(innerResults.First());
        }
    }
}
