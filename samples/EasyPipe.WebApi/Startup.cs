using EasyPipe.WebApi.Pipeline;
using EasyPipe.Extensions.MicrosoftDependencyInjection;
using EasyPipe.WebApi.PipelineWithoutRequest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace EasyPipe.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                             new OpenApiInfo {Title = "EasyPipe.WebApi", Version = "v1"});
            });

            // services.AddPipeline<PipelineRequest, PipelineResponse>()
            //         .WithMiddleware<Middleware1>()
            //         .WithMiddleware<Middleware2>()
            //         .WithMiddleware<Middleware3>();
            //
            // services.AddPipeline<PipelineResponse2>()
            //         .WithMiddleware<Middleware4>()
            //         .WithMiddleware<Middleware5>()
            //         .WithMiddleware<Middleware6>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyPipe.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}