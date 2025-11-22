// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace EasyPipe.WebApi.PipelineWithoutRequest
// {
//     public class Middleware6 : IMiddleware< PipelineResponse2>
//     {
//         public async Task<PipelineResponse2> RunAsync(IPipelineContext context,
//                                                      Func<Task<PipelineResponse2>> next,
//                                                      CancellationToken cancellationToken)
//         {
//             PipelineResponse2 result = await next() ?? new PipelineResponse2
//             {
//             };
//
//             result.BackwardCount++;
//
//             return result;
//         }
//     }
// }