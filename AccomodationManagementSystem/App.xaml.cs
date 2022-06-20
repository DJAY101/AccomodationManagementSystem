using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace AccomodationManagementSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            DatabaseFacade fascade = new DatabaseFacade(new LoginDataContext());
            fascade.EnsureCreated();
            using(LoginDataContext context = new LoginDataContext()) {

                bool entryExists = context.m_loginInfo.Any(loginDetails => loginDetails.user == "og");

                if (!entryExists) {
                    loginInfo temp = new loginInfo();
                    temp.Password = "1234";
                    temp.user = "og";
                    context.m_loginInfo.Add(temp);
                    context.SaveChanges();
                }
            
            }
        }



    }
}
