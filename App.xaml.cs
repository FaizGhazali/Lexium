using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting;
using ActiproSoftware.Windows.Controls.SyntaxEditor.Highlighting.Implementation;
using Lexium.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lexium.WordTagger;
using Lexium.Feature.WordTagger;

namespace Lexium
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);

            var style = new HighlightingStyle
            {
                Foreground =Colors.Transparent,
                Background = null
                
            };

            AmbientHighlightingStyleRegistry.Instance.Register(
                ClassificationTypesCustom.InvisibleSquiggle,
                style
            );




            var mainPage = ServiceProvider.GetRequiredService<MainWindow>();
            mainPage.Show();

        }
        public void ConfigureServices(IServiceCollection services)
        {
            //Register View
            services.AddSingleton<MainWindow>();


            
            services.AddSingleton<MainWindowVM>();
           
        }
        public App()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }
        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Debug.WriteLine($"Attempting to resolve: {args.Name}");

            // Match by simple assembly name
            if (args.Name.StartsWith("Microsoft.Xaml.Behaviors,", StringComparison.OrdinalIgnoreCase))
            {
                string assemblyPath = @"C:\libs\Microsoft.Xaml.Behaviors.dll"; // Update with your path

                Debug.WriteLine($"Looking for assembly at: {assemblyPath}");

                if (File.Exists(assemblyPath))
                {
                    try
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to load assembly: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Assembly file not found.");
                }
            }

            return null;
        }
    }

}
