using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FitnessAssistant.Api.Data;
using FitnessAssistant.Api.Features.Users.Constants;
using FitnessAssistant.Api.Shared.Authorization;
using FitnessAssistant.Api.Shared.FileUpload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class UserEndpoints
{
    private const string DefaultProfileImageUri = "https://placehold.co/100";
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        //get all users
        app.MapGet("/", async (FitnessAssistantContext dbContext, [AsParameters] GetUsersDto queryRequest) =>
        {
            var skipCount = (queryRequest.pageNumber - 1) * queryRequest.pageSize;

            var filteredGames = dbContext.Users.Where(user => string.IsNullOrWhiteSpace(queryRequest.searchName) || EF.Functions.Like(user.Name, $"%{queryRequest.searchName}%"));

            var usersOnPage = await filteredGames.OrderBy(user => user.Name)
                                    .Skip(skipCount)
                                    .Take(queryRequest.pageSize)
                                        .Select(user => new UsersDetailResponseDto(user.Id, user.Name, user.ProfileImageUri, user.LastUpdatedBy))
                                        .AsNoTracking()
                                        .ToListAsync();

            var totalUser = await filteredGames.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUser / (double)queryRequest.pageSize);

            return new UserPageDto(totalPages, usersOnPage);
        }).AllowAnonymous();

        //get single user
        app.MapGet("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            User? foundUser = await dbContext.Users.FindAsync(id);

            return foundUser is null ? Results.NotFound() : Results.Ok(
                new UserSummaryResponseDto(foundUser.Id, foundUser.Name, foundUser.ProfileImageUri, foundUser.LastUpdatedBy)
            );
        }).WithName(UserEndpointConstants.GetUser).AllowAnonymous();




        //POST /user
        app.MapPost("/", async ([FromForm] CreateUserDto createUserDto, FitnessAssistantContext dbContext, ILoggerFactory loggerFactory,
         FileUploader fileUploader,
        ClaimsPrincipal userClaim) =>
        {
            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }

            var imageUri = DefaultProfileImageUri;
            if (createUserDto.ImageFile is not null)
            {
                var fileUploadResult = await fileUploader.UploadFileAsync(createUserDto.ImageFile, StorageNames.UserProfileImageFolder);
                if (!fileUploadResult.IsSuccess)
                {
                    return Results.BadRequest(new { message = fileUploadResult.ErrorMessage });
                }
                imageUri = fileUploadResult.FileUrl;
            }

            User createdUser = new User()
            {
                Name = createUserDto.Name,
                ProfileImageUri = imageUri!,
                LastUpdatedBy = currentUserId
            };

            dbContext.Users.Add(createdUser);
            await dbContext.SaveChangesAsync();

            var logger = loggerFactory.CreateLogger("User");
            logger.LogInformation("User {UserName} added to the database", createdUser.Name);

            return Results.CreatedAtRoute(UserEndpointConstants.GetUser, new { id = createdUser.Id }, new UsersDetailResponseDto(createdUser.Id, createdUser.Name, createdUser.ProfileImageUri, createdUser.LastUpdatedBy));

        }).WithParameterValidation().DisableAntiforgery().RequireAuthorization(Policies.AdminAccess);

        //update user
        app.MapPut("/{id}", async (Guid id, [FromForm] UpdateUserDto userUpdateDto, FitnessAssistantContext dbContext, ILogger<Program> logger, FileUploader fileUploader,
        ClaimsPrincipal userClaim) =>
        {

            if (!userClaim?.Identity?.IsAuthenticated == true) { return Results.Unauthorized(); }

            var currentUserId = userClaim?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? userClaim?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(currentUserId)) { return Results.Unauthorized(); }


            var existingUser = await dbContext.Users.FindAsync(id);

            if (existingUser is null)
            {
                return Results.NotFound();
            }

            if (userUpdateDto.ImageFile is not null)
            {
                var fileUploadResult = await fileUploader.UploadFileAsync(userUpdateDto.ImageFile, StorageNames.UserProfileImageFolder);
                if (!fileUploadResult.IsSuccess)
                {
                    return Results.BadRequest(new { message = fileUploadResult.ErrorMessage });
                }
                existingUser.ProfileImageUri = fileUploadResult.FileUrl!;
            }

            existingUser.Name = userUpdateDto.Name;
            existingUser.LastUpdatedBy = currentUserId;


            await dbContext.SaveChangesAsync();
            logger.LogInformation("User {UserName} updated in the database", existingUser.Name);

            return Results.NoContent();
        }).WithParameterValidation().DisableAntiforgery().RequireAuthorization(Policies.AdminAccess);

        //delete user
        app.MapDelete("/{id}", async (Guid id, FitnessAssistantContext dbContext) =>
        {
            await dbContext.Users.Where(user => user.Id == id).ExecuteDeleteAsync();
            return Results.NoContent();
        }).RequireAuthorization(Policies.AdminAccess);
    }
}