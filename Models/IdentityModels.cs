using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;

//Creating authentication with MySQL on Azure instead of any SQLSERVER LOCAL DB using this for guidance - 
//www.asp.net/identity/overview/extensibility/implementing-a-custom-mysql-aspnet-identity-storage-provider

namespace EPMSAppDemo.Models
{
    //Below code in this model generated using AspNet.Identity.MySQL to allow me to store all login information in the MySQL db on Azure
    // You can add profile data for the user by adding more properties to your ApplicationUser class, -> http://go.microsoft.com/fwlink/?LinkID=317594 to see more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add the user's identity
            return userIdentity;
        }
    }

    public class ApplicationDbContext : MySQLDatabase
    {
        //Creating the connection string for the database - See web.config for full connection string for the azure MySQL db
        public ApplicationDbContext(string connectionName)
            : base(connectionName)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext("DefaultConnection");
        }
    }
}