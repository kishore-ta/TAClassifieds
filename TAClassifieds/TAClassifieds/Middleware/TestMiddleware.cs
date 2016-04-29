using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace TAClassifieds.Middleware
{
    public class TestMiddleware:OwinMiddleware
    {
        public TestMiddleware(OwinMiddleware next)
            : base(next)
        {

        }

        public async override System.Threading.Tasks.Task Invoke(IOwinContext context)
        {
            Task<IFormCollection> v= context.Request.ReadFormAsync();
            if(v.Result.Get("UserName")=="kishore")
             context.Response.Redirect("http://www.xyz.com");
            else
            await Next.Invoke(context);
        }
    }
}