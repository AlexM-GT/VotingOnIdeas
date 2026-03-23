using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VotingOnIdeas.Application.Auth;
using VotingOnIdeas.Application.Ideas;

namespace VotingOnIdeas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();

        services.AddScoped<GetIdeasUseCase>();
        services.AddScoped<GetIdeaByIdUseCase>();
        services.AddScoped<CreateIdeaUseCase>();
        services.AddScoped<UpdateIdeaUseCase>();
        services.AddScoped<DeleteIdeaUseCase>();
        services.AddScoped<RateIdeaUseCase>();

        return services;
    }
}
