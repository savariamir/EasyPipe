// using System;
// using System.Collections.Generic;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace EasyPipe.Extensions.MicrosoftDependencyInjection
// {
//     public class PipelineBuilder<TResponse>
//     {
//         private readonly IServiceCollection _services;
//
//         internal List<Type> Middlewares { get; }
//
//         public PipelineBuilder(IServiceCollection services)
//         {
//             _services = services;
//             Middlewares = [];
//         }
//
//         public PipelineBuilder<TResponse> WithMiddleware<TMiddleware>()
//             where TMiddleware : class, IMiddleware<TResponse>
//         {
//             _services.AddTransient<TMiddleware>();
//             Middlewares.Add(typeof(TMiddleware));
//
//             return this;
//         }
//     }
// }