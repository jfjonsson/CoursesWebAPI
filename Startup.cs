using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Framework.DependencyInjection;

namespace CoursesWebAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();
            
            //Add Cors support to the service
            services.AddCors();
    
            var policy = new Microsoft.AspNet.Cors.Core.CorsPolicy();
    
            policy.Headers.Add("*");    
            policy.Methods.Add("*");          
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;
    
            services.ConfigureCors(x=>x.AddPolicy("mypolicy", policy));
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {   
            // Configure the HTTP request pipeline.
            //app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            
            app.UseWelcomePage();
        }
    }
}
