// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace EasyPipe
// {
//     public interface IMiddleware<TResponse>
//     {
//         Task<TResponse> RunAsync(IPipelineContext context,
//                                  Func<Task<TResponse>> next,
//                                  CancellationToken cancellationToken);
//     }
// }