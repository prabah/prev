using System;
using System.Collections.Generic;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SqlInjection.Prevention.API.Startup))]

namespace SqlInjection.Prevention.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
