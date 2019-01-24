using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Canvas
{
    public class CultureRouteConstraint : RegexRouteConstraint
    {
        public CultureRouteConstraint()
            : base(@"^[a-zA-Z]{2}(\-[a-zA-Z]{2})?$") { }
    }
}
