using Microsoft.AspNetCore.Mvc;
using Module5.ApiIA.Models;
using Module5.ApiIA.Services;

namespace Module5.ApiIA.Controllers;

[ApiController]
[Route("api/ai")]
[Produces("application/json")]
public class AiController : ControllerBase
{
    private readonly IAiService          _ai;
    private readonly ILogger<AiController> _logger;

    public AiController(IAiService ai, ILogger<AiController> logger)
    {
        _ai     = ai;
        _logger = logger;
    }

    // ── POST /api/ai/chat ─────────────────────────────────────────────────────

    /// <summary>Envoie un message à l'IA et retourne sa réponse.</summary>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ChatResponse),            StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        _logger.LogInformation("POST /api/ai/chat — {Chars} chars", request.Message.Length);

        var response = await _ai.ChatAsync(request);
        return Ok(response);
    }

    // ── POST /api/ai/summarize ────────────────────────────────────────────────

    /// <summary>Résume un texte en bullets, paragraphe ou JSON.</summary>
    [HttpPost("summarize")]
    [ProducesResponseType(typeof(SummarizeResponse),        StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Summarize([FromBody] SummarizeRequest request)
    {
        _logger.LogInformation("POST /api/ai/summarize — {Chars} chars, format={Format}",
            request.Text.Length, request.Format);

        var response = await _ai.SummarizeAsync(request);
        return Ok(response);
    }

    // ── GET /api/ai/health ────────────────────────────────────────────────────

    /// <summary>Vérifie que l'API est opérationnelle.</summary>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() =>
        Ok(new { status = "ok", timestamp = DateTime.UtcNow });
}
