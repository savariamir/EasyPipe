// using System;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace EasyPipe.WebApi.Pipeline
// {
//     public class Middleware3 : IMiddleware<PipelineRequest, PipelineResponse>
//     {
//         public async Task<PipelineResponse> RunAsync(PipelineRequest request,
//             IPipelineContext context,
//             Func<Task<PipelineResponse>> next,
//             CancellationToken cancellationToken)
//         {
//             request.Count++;
//
//             PipelineResponse result = await next() ?? new PipelineResponse
//             {
//                 ForwardCount = request.Count
//             };
//
//             result.BackwardCount++;
//
//             return result;
//         }
//     }
// }