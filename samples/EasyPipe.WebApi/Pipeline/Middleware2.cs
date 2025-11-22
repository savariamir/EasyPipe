// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace EasyPipe.WebApi.Pipeline
// {
//     public class Middleware2 : IMiddleware<PipelineRequest, PipelineResponse>
//     {
//         public async Task<PipelineResponse> RunAsync(PipelineRequest request,
//             IPipelineContext context,
//             Func<Task<PipelineResponse>> next,
//             CancellationToken cancellationToken)
//         {
//             request.Count++;
//
//             var result = await next();
//
//             result.BackwardCount++;
//
//             return result;
//         }
//     }
// }