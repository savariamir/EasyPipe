// using System.Threading.Tasks;
// using EasyPipe.WebApi.Pipeline;
// using EasyPipe.WebApi.PipelineWithoutRequest;
// using Microsoft.AspNetCore.Mvc;
//
// namespace EasyPipe.WebApi.Controllers
// {
//     [ApiController]
//     [Route("[controller]")]
//     public class PipelineController : ControllerBase
//     {
//         private readonly IPipeline<PipelineRequest, PipelineResponse> _pipeline;
//         private readonly IPipeline<PipelineResponse2> _pipeline2;
//
//         public PipelineController(IPipeline<PipelineRequest, PipelineResponse> pipeline, IPipeline<PipelineResponse2> pipeline2)
//         {
//             _pipeline = pipeline;
//             _pipeline2 = pipeline2;
//         }
//
//         [HttpGet]
//         public Task<PipelineResponse> RunPipeline()
//         {
//             return _pipeline.RunAsync(new PipelineRequest());
//         }
//         
//         
//         [HttpGet("pipeline-without-request")]
//         public Task<PipelineResponse2> RunPipeline2()
//         {
//             return _pipeline2.RunAsync();
//         }
//     }
// }