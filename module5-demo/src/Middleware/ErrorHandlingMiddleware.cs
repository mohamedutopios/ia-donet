using Azure;
using System.Net;
using System.Text.Json;

namespace Module5.ApiIA.Middleware;

/// <summary>
/// Middleware global de gestion des erreurs.
/// Intercepte toutes les exceptions et retourne une réponse RFC 7807 Problem Details.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate             _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur non gérée sur {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, title, detail) = ex switch
        {
            RequestFailedException rfe when rfe.Status == 401 =>
                (HttpStatusCode.Unauthorized,
                 "Clé API invalide",
                 "La clé API Azure OpenAI est incorrecte ou expirée."),

            RequestFailedException rfe when rfe.Status == 429 =>
                (HttpStatusCode.TooManyRequests,
                 "Quota dépassé",
                 "Trop de requêtes vers Azure OpenAI. Réessayez dans quelques secondes."),

            RequestFailedException rfe when rfe.Status == 404 =>
                (HttpStatusCode.BadGateway,
                 "Déploiement introuvable",
                 $"Le déploiement Azure OpenAI configuré est introuvable : {rfe.Message}"),

            RequestFailedException rfe =>
                (HttpStatusCode.BadGateway,
                 "Erreur Azure OpenAI",
                 rfe.Message),

            OperationCanceledException =>
                (HttpStatusCode.RequestTimeout,
                 "Requête annulée",
                 "La requête a été annulée ou a expiré."),

            _ =>
                (HttpStatusCode.InternalServerError,
                 "Erreur interne",
                 "Une erreur inattendue s'est produite.")
        };

        context.Response.StatusCode  = (int)status;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type     = $"https://httpstatuses.com/{(int)status}",
            title,
            status   = (int)status,
            detail,
            instance = context.Request.Path.Value
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
