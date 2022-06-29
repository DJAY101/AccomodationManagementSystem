using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AccomodationManagementSystem.VacancyDatabaseClasses;

namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            DatabaseFacade loginDatabase = new DatabaseFacade(new LoginDataContext());
            loginDatabase.EnsureCreated();

            DatabaseFacade accomodationDatabase = new DatabaseFacade(new AccomodationContext());
            accomodationDatabase.EnsureCreated();

            using (LoginDataContext context = new LoginDataContext()) {

                bool entryExists = context.m_loginInfo.Any(loginDetails => loginDetails.user == "ADMIN");

                if (!entryExists)
                {
                    StartupUri = new Uri("/AccomodationManagementSystem;component/CreatePassword.xaml", UriKind.Relative);
                }
                else 
                {
                    StartupUri = new Uri("/AccomodationManagementSystem;component/loginScreen.xaml", UriKind.Relative);
                }
            
            }
        }



    }
}
