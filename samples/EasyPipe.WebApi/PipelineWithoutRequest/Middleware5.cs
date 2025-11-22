// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace EasyPipe.WebApi.PipelineWithoutRequest
// {
//     public class Middleware5 : IMiddleware<PipelineResponse2>
//     {
//         public async Task<PipelineResponse2> RunAsync(
//             IPipelineContext context,
//             Func<Task<PipelineResponse2>> next,
//             CancellationToken cancellationToken)
//         {
//             var result = await next();
//
//             result.BackwardCount++;
//
//             return result;
//         }
//     }
// }