using CLIESC_EurekaBank_SOAP_GR04;
using ec.edu.monster.controllers;
using ec.edu.monster.services;
using ec.edu.monster.views;
using System.Windows;

namespace EurekaBank.WpfClient
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Instancia del API SOAP (ajusta URL abajo si hace falta)
            var api = new SoapEurekaApi("http://localhost:51640/ec.edu.monster.ws/WSEureka.asmx");

            var login = new LoginWindow();
            var ctrl = new LoginController(login, api);
            MainWindow = login;
            login.Show();
        }
    }
}
