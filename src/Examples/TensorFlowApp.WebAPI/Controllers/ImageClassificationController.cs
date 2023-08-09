using ImageClassification.Domain.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TensorFlowApp.WebAPI.Controllers.Requests;
using TensorFlowApp.WebAPI.Domain.Services;

namespace TensorFlowApp.WebAPI.Controllers;

/// <summary>
/// ML TensorFlow Image Classification
/// </summary>
[ApiVersion("1")]
[ApiExplorerSettings(GroupName = "v1")]
[ApiController]
[Route("api/imageclassification")]
//[Authorize]
[AllowAnonymous]
public class ImageClassificationController : ControllerBase
{
    //private readonly IUserIdProvider userProvider;
    private readonly IImageClassifierService domainService;
    private readonly ILogger logger;

    /// <summary>
    /// Controller for Image Classification
    /// </summary>
    /// <param name="domainService"></param>
    /// <param name="logger"></param>
    public ImageClassificationController(IImageClassifierService domainService,/* IUserIdProvider userProvider,*/ ILogger logger)
    {
        this.domainService = domainService;
        this.logger = logger;
        //this.userProvider = userProvider;
    }

    /// <summary>
    /// Invokes a classification of the uploaded Image
    /// </summary>
    /// <param name="file"></param>
    /// <param name="dataSet"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload/{dataSet}")]
    public async Task<IActionResult> Upload([FromForm] IFormFile? file, string dataSet, CancellationToken cancellationToken)
    {
        // UserId = userProvider.UserId
        if (file is null || file.Length == 0)
            return BadRequest("file not selected");

        if (file.Length > int.MaxValue)
            return BadRequest("file too large");

        var ms = new MemoryStream((int)file.Length);

        await file.CopyToAsync(ms, cancellationToken);
        var image = new InMemoryImage(ms.ToArray());
        var request = new ImageClassificationRequest()
        {
            DataSet = dataSet
        };
        return Ok(await domainService.Classify(image, request.DataSet, cancellationToken));
    }


    [HttpGet("models")]
    public string[] GetModels(CancellationToken cancellationToken)
    {
        return domainService.GetModels(cancellationToken);
    }
}
