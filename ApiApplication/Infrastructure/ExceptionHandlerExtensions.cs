using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiApplication
{
    internal static class ExceptionHandlerExtensions
    {
        public static void AddExceptionHandlers(this IServiceCollection services)
        {
            var handlerTypes = Assembly.GetExecutingAssembly().DefinedTypes.Where(i => i.IsClass && i.IsAssignableTo(typeof(IExceptionHandler)));
            foreach (var handlerType in handlerTypes)
                services.AddSingleton(typeof(IExceptionHandler), handlerType);
        }

        public static ExceptionHandlerOptions CreateExceptionHandlerOptions(this IApplicationBuilder app)
        {
            return new ExceptionHandlerOptions()
            {
                ExceptionHandler = async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var controller = (string)feature.RouteValues["controller"];

                    var handled = false;

                    // Chain of responsibility would be more beautiful here.

                    DefaultExceptionHandler defaultHandler = null;
                    var handlers = app.ApplicationServices.GetRequiredService<IEnumerable<IExceptionHandler>>();
                    foreach (var handler in handlers)
                        if (handler is DefaultExceptionHandler)
                            defaultHandler = (DefaultExceptionHandler) handler;
                        else 
                            if (handled = await handler.HandleAsync(controller, feature.Error, context))
                                break;

                    if (!handled)
                        if (defaultHandler == null)
                            throw new ApplicationException($"The {nameof(DefaultExceptionHandler)} is not registered.");
                        else
                            await defaultHandler.HandleAsync(controller, feature.Error, context);
                }
            };
        }
    }
}
