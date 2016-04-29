using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace TAClassifieds.Middleware
{
    public static class AppMiddlewareExtensions
    {
        public static void UseIncludeContextKeys(this IAppBuilder app)
        {
            //app.Use(new TestMiddleware(), null);
            app.Use(typeof(TestMiddleware));
            //return app;
        }
    }
}