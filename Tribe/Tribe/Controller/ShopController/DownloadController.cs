using Microsoft.AspNetCore.Mvc;
using Tribe.Data;
using Tribe.Controller.Services;

namespace Tribe.Controller.ShopController
{
    [ApiController]
    [Route("api/shop/download")]
    public class DownloadController : ControllerBase
    {
        private readonly IDownloadService _downloadService;
        private readonly ILogger<DownloadController> _logger;

        public DownloadController(IDownloadService downloadService, ILogger<DownloadController> logger)
        {
            _downloadService = downloadService;
            _logger = logger;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> Download(string token)
        {
            var (success, url, reason) = await _downloadService.ValidateAndGetDownloadUrlAsync(token);
            if (!success)
            {
                _logger.LogWarning("Download token validation failed: {Reason}", reason);
                return BadRequest(new { message = reason });
            }

            // For simplicity, redirect to the real file URL (could be pre-signed S3 URL, CDN, etc.)
            return Redirect(url);
        }
    }
}
