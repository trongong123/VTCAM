using Microsoft.Extensions.DependencyInjection;

namespace EQX.UIDemo.StartupHelpers
{
    public static class ServicesExtensions
    {
        public static void AddFormFactory<TForm>(this IServiceCollection services) where TForm : class
        {
            services.AddTransient<TForm>();
            services.AddSingleton<Func<TForm>>(x => () => x.GetService<TForm>()!);
        } 
    }
}
