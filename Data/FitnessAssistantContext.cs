using System;
using Microsoft.EntityFrameworkCore;

namespace FitnessAssistant.Api.Data;

public class FitnessAssistantContext(DbContextOptions<FitnessAssistantContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<NutritionContributor> NutritionContributors => Set<NutritionContributor>();


    public DbSet<FoodMeal> FoodMeals => Set<FoodMeal>();

    public DbSet<MealIngredient> MealIngredients => Set<MealIngredient>();
    public DbSet<RawIngredient> RawIngredients => Set<RawIngredient>();


    public DbSet<FoodGroup> FoodGroups => Set<FoodGroup>();
    public DbSet<MealCategory> MealCategories => Set<MealCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FoodMeal>()
            .HasMany(p=>p.MealIngredients)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }


}
