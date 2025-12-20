using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FitnessAssistant.Api.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this WebApplication app)
    {
        await app.MigrateDbAsync();
        await app.SeedDbAsync();
        app.Logger.LogInformation(18, "The Database is ready!");
    }
    private static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        FitnessAssistantContext dbContext = scope.ServiceProvider.GetRequiredService<FitnessAssistantContext>();

        await dbContext.Database.MigrateAsync();
    }

    private static async Task SeedDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        FitnessAssistantContext dbContext = scope.ServiceProvider.GetRequiredService<FitnessAssistantContext>();

     
        if (!dbContext.FoodGroups.Any())
        {
            dbContext.FoodGroups.AddRange(
            new FoodGroup { Name = "Dairy" },
            new FoodGroup { Name = "Protein Source" },
            new FoodGroup { Name = "Fruit" },
            new FoodGroup { Name = "Vegetable" },
            new FoodGroup { Name = "Grain" },
            new FoodGroup { Name = "Fat" },
            new FoodGroup { Name = "Sugar" },
            new FoodGroup { Name = "Spices" }
            );
        }

        if (!dbContext.MealCategories.Any())
        {
            dbContext.MealCategories.AddRange(
                new MealCategory { Name = "Breakfast" },
                new MealCategory { Name = "Lunch" },
                new MealCategory { Name = "Dinner" },
                new MealCategory { Name = "Desert" }
            );
        }

        if (!dbContext.NutritionContributors.Any())
        {
            dbContext.NutritionContributors.Add(
                new NutritionContributor { Name = "Test Contributor", ProfileImageUri = "https://placeholder.co/100" }
            );
        }

        // if (!dbContext.RawIngredients.Any())
        // {
        //     dbContext.RawIngredients.AddRange(
        //     new RawIngredient
        //     {
        //         Name = "Carrot",
        //         CaloriesPerUnit = 64,
        //         ProteinPerUnit = 1.3,
        //         UnitOfMeasurement = UnitOfMeasurement.Gram,
        //         BaseLineMeasurement = 100,
        //         FoodGroupId = new Guid("6F5EDE71-D49B-4438-8676-8DD3B95D7E93"),
        //         SubmittedById = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348")
        //     },
        //     new RawIngredient
        //     {
        //         Name = "Beef",
        //         CaloriesPerUnit = 185,
        //         ProteinPerUnit = 18.2,
        //         UnitOfMeasurement = UnitOfMeasurement.Gram,
        //         BaseLineMeasurement = 100,
        //         FoodGroupId = new Guid("B0E865ED-ADFD-4DF8-9DFE-DF5D8B19822E"),
        //         SubmittedById = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348")
        //     },
        //     new RawIngredient
        //     {
        //         Name = "Chicken Breast",
        //         CaloriesPerUnit = 158,
        //         ProteinPerUnit = 32.1,
        //         UnitOfMeasurement = UnitOfMeasurement.Gram,
        //         BaseLineMeasurement = 100,
        //         FoodGroupId = new Guid("B0E865ED-ADFD-4DF8-9DFE-DF5D8B19822E"),
        //         SubmittedById = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348")
        //     },
        //     new RawIngredient
        //     {
        //         Name = "Brazil Nuts",
        //         CaloriesPerUnit = 664,
        //         ProteinPerUnit = 15,
        //         UnitOfMeasurement = UnitOfMeasurement.Gram,
        //         BaseLineMeasurement = 100,
        //         FoodGroupId = new Guid("CD25BBC3-F650-4C6D-9069-997E3599CB8E"),
        //         SubmittedById = new Guid("BDC3E978-10F3-42C0-A43C-353F8CB6B348")
        //     });
        // }

        await dbContext.SaveChangesAsync();
    }
}
