using BusinessLayer.Services;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Repository;

namespace TAIMinAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<UserService>();
            services.AddScoped<UserDb>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // Endpoint to get all users
                endpoints.MapGet("/api/users", async context =>
                {
                    var userService = context.RequestServices.GetRequiredService<UserService>();
                    var users = userService.GetAllUsers();
                    await context.Response.WriteAsJsonAsync(users);
                });

                // Endpoint to get user by id
                endpoints.MapGet("/api/users/{id}", async context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());
                    var userService = context.RequestServices.GetRequiredService<UserService>();
                    var user = userService.GetUserById(id);

                    if (user != null)
                    {
                        await context.Response.WriteAsJsonAsync(user);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync($"User with ID {id} not found.");
                    }
                });

                // Endpoint to add a new user
                endpoints.MapGet("/api/users/add/{name}/{surname}",  context =>
                {
                    var name = context.Request.RouteValues["name"].ToString();
                    var surname = context.Request.RouteValues["surname"].ToString();

                    var userService = context.RequestServices.GetRequiredService<UserService>();
                    var newUser = new UserModel { Name = name, Surname = surname };
                    userService.AddUser(newUser);

                    return context.Response.WriteAsync($"User added successfully.");
                });

                // Endpoint to update a user
                endpoints.MapGet("/api/users/update/{id}/{name}/{surname}",  context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());
                    var name = context.Request.RouteValues["name"].ToString();
                    var surname = context.Request.RouteValues["surname"].ToString();

                    var userService = context.RequestServices.GetRequiredService<UserService>();
                    var existingUser = userService.GetUserById(id);

                    if (existingUser != null)
                    {
                        existingUser.Name = name;
                        existingUser.Surname = surname;
                        userService.UpdateUser(existingUser);

                        return context.Response.WriteAsync($"User with ID: {existingUser.Id} updated successfully.");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        return context.Response.WriteAsync($"User with ID {id} not found.");
                    }
                });

                // Endpoint to delete a user by id
                endpoints.MapGet("/api/users/delete/{id}",  context =>
                {
                    var id = int.Parse(context.Request.RouteValues["id"].ToString());

                    var userService = context.RequestServices.GetRequiredService<UserService>();
                    var existingUser = userService.GetUserById(id);

                    if (existingUser != null)
                    {
                        
                        userService.DeleteUser(id);
                        context.Response.StatusCode = 200;
                        return context.Response.WriteAsync($"User deleted successfully. ID: {id}");
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        return context.Response.WriteAsync($"Userxx with ID {id} not found.");
                    }
                });
            });
        }
    }
}
