using crossblog.Domain;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace crossblog.tests.Controllers
{
    public static class OperableExtensions
    {
        public static IOperable<Comment> CreateTitles(this IOperable<Comment> operable)
        {
            ((IDeclaration<Comment>)operable).ObjectBuilder.With(x => x.Title = Constants.SampleTitle);
            return operable;
        }
    }
}
