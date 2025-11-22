// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using EasyPipe.Abstractions;
// using EasyPipe.Exceptions;
//
// namespace EasyPipe.Internal
// {
//     internal class LazyPipeline<TRequest, TResponse> : IPipeline<TRequest, TResponse>
//     {
//         private readonly Type[] _middlewareTypes;
//         private readonly IServiceProvider _serviceProvider;
//
//         public LazyPipeline(IServiceProvider serviceProvider, IEnumerable<Type> middlewareTypes)
//         {
//             _serviceProvider = serviceProvider;
//             _middlewareTypes = middlewareTypes.ToArray();
//         }
//
//         public Task<TResponse> RunAsync(TRequest request, CancellationToken cancellationToken)
//         {
//             Func<Task<TResponse>> middlewareDelegate = null;
//             IPipelineContext context = new PipelineContext(); 
//             
//             var index = 0;
//
//             middlewareDelegate = () =>
//             {
//                 if (_middlewareTypes.Length == index)
//                 {
//                     return Task.FromResult<TResponse>(default);
//                 }
//
//                 var middlewareType = _middlewareTypes[index++];
//                 
//                 var middleware = _serviceProvider.GetService(middlewareType) as IMiddleware<TRequest, TResponse>;
//
//                 if (middleware is null)
//                 {
//                     throw new MiddlewareNotResolvedException(middlewareType);
//                 }
//                 
//                 return middleware.RunAsync(request, context, middlewareDelegate, cancellationToken);
//             };
//
//             return middlewareDelegate();
//         }
//     }
// }