// File: Data/SeedData.cs
//
// PURPOSE: Runs once at app startup to ensure the database has:
//   1. All roles defined in AppRoles.cs
//   2. A default Admin user so you can log in immediately
//
// HOW IT WORKS: Called from Program.cs after Database.Migrate().
// Each call is idempotent — it checks whether each role/user exists
// before creating it, so it's safe to run on every startup.

using Microsoft.AspNetCore.Identity;

namespace PuppetFestAPP.Web.Data;

/// <summary>
/// Seeds the database with required roles and a default admin user.
/// Called once at startup from Program.cs. All operations are
/// idempotent (safe to re-run without creating duplicates).
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Creates all application roles and the default admin user
    /// if they don't already exist.
    /// </summary>
    /// <param name="serviceProvider">
    /// The scoped service provider from the startup block in Program.cs.
    /// Used to resolve RoleManager and UserManager from dependency injection.
    /// </param>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        // ── Resolve the Identity managers from DI ──
        // RoleManager<IdentityRole>: creates/queries roles in AspNetRoles table
        // UserManager<ApplicationUser>: creates/queries users in AspNetUsers table
        // Both of these are registered by .AddIdentityCore() in Program.cs.
        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        // ── Step 1: Create all roles ──
        // Iterates through every role name in AppRoles.AllRoles and
        // creates it in the database if it doesn't already exist.
        foreach (var roleName in AppRoles.AllRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));

                if (result.Succeeded)
                {
                    Console.WriteLine($"  ✓ Created role: {roleName}");
                }
                else
                {
                    // Identity returns structured errors (e.g., duplicate name).
                    // Join them into a readable string for the console.
                    Console.WriteLine($"  ✗ Failed to create role {roleName}: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                Console.WriteLine($"  - Role already exists: {roleName}");
            }
        }

        // ── Step 2: Create a default Admin user ──
        // This gives you an account to log in with immediately.
        // In production, change this password after first login!
        var adminEmail = "admin@puppetfest.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // Create the user object (not yet saved to DB)
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,   // Identity uses this for login
                Email = adminEmail,
                EmailConfirmed = true    // Skip email verification for seed user
            };

            // CreateAsync hashes the password and saves to AspNetUsers
            var createResult = await userManager.CreateAsync(
                adminUser, "Admin123!");

            if (createResult.Succeeded)
            {
                // Link the user to the Admin role in AspNetUserRoles table
                await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                Console.WriteLine(
                    $"  ✓ Created admin user: {adminEmail} " +
                    $"(password: Admin123!)");
            }
            else
            {
                Console.WriteLine(
                    $"  ✗ Failed to create admin user: " +
                    string.Join(", ",
                        createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine(
                $"  - Admin user already exists: {adminEmail}");
        }
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        SeedProducts(context);

    }

     private static void SeedProducts(ApplicationDbContext context)
    {
        // 1. Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Id = 1, Name = "Apparel" },
                new Category { Id = 2, Name = "Accessories" },
                new Category { Id = 3, Name = "DrinkWare" },
                new Category { Id = 4, Name = "Books" },
                new Category { Id = 5, Name = "Puppets" }
            );
        }

        // 2. Images
        if (!context.Images.Any())
        {
            context.Images.AddRange(
                new Image
                {
                    Id = 1,
                    FileName = "La-Liga-Teatro-Elastico-Deer-T-Shirt.png",
                    AltText = "La Liga Teatro Elastico Deer T-Shirt"
                },
                new Image
                {
                     Id = 2,
                    FileName = "La-Liga-Teatro-Elastico-Wolf-Sweatshirt.png",
                    AltText = "La Liga Teatro Elastico Wolf Sweatshirt"

                },
                new Image
                {
                    Id = 3,
                    FileName = "Puppet-Fest-Logo-Black-T-Shirt.png",
                    AltText = "Puppet Fest Logo Black T-Shirt"

                },
                new Image
                {
                    Id = 4,
                    FileName = "Puppet-Fest-Logo-Orchid-T-Shirt.png",
                    AltText = "Puppet Fest Logo Orchid T-Shirt"


                },
                new Image
                {
                    Id = 5,
                    FileName = "Puppet-Fest-Logo-Purple-T-Shirt.png",
                    AltText = "Puppet Fest Logo Purple T-Shirt"
                },
                new Image
                {
                    Id = 6,
                    FileName = "Puppet-Fest-Logo-Tote-Bag.png",
                    AltText = "Puppet Fest Logo Tote Bag"
                },
                new Image
                {
                    Id = 7,
                    FileName = "Stemless-Wine-Glass.png",
                    AltText = "9oz Stemless Wine Glass"
                },
                new Image
                {
                    Id = 8,
                    FileName = "Galaxy-Of-Things-Paperback.png",
                    AltText = "Book - A Galaxy of Things (Paperback)"
                },
                new Image
                {
                    Id = 9,
                    FileName = "Boy-Puppet-Button.png",
                    AltText = "Boy Puppet Button"
                },
                new Image
                {
                    Id = 10,
                    FileName = "Die-Cut-Puppet-Stickers.png",
                    AltText = "Die Cut Puppet Stickers (Pack of 4)"
                },
                new Image
                {
                    Id = 11,
                    FileName = "Toy-Binoculars.png",
                    AltText = "DIY Toy Binoculars"
                },
                new Image 
                { 
                    Id = 12,
                    FileName = "Birch-Waffle-Cuffed-Beanie.png", 
                    AltText = "Puppet Fest Cuffed Waffle Beanie - Birch"
                },
                new Image
                { 
                    Id = 13,
                    FileName = "Camel-Waffle-Cuffed-Beanie.png",
                    AltText = "Puppet Fest Cuffed Waffle Beanie - Camel"
                },
                new Image
                { 
                    Id = 14,
                    FileName = "Puppet-Fest-Notecards.png",
                    AltText = "Puppet Fest Notecards (Pack of 8)"
                    },
                new Image
                { 
                    Id = 15, 
                    FileName = "Bottle-Green-Cuffed-Beanie.png", 
                    AltText = "Puppet Fest Sustainable Rib Cuffed Beanie - Bottle Green"
                },
                new Image
                { 
                    Id = 16, 
                    FileName = "Burgundy-Cuffed-Beanie.png", 
                    AltText = "Puppet Fest Sustainable Rib Cuffed Beanie - Burgundy"
                },
                new Image
                { 
                    Id = 17, 
                    FileName = "Puppet-Head-Button-2023.png", 
                    AltText = "Puppet Head Button 2023"
                },
                new Image
                { 
                    Id = 18, 
                    FileName = "Puppet-Head-Button-2024.png", 
                    AltText = "Puppet Head Button 2024" 
                },
                new Image
                { 
                    Id = 19, 
                    FileName = "Hare-Shadow-Puppet.png", 
                    AltText = "Shadow Puppet: The Hare"
                }

                
            );
        }

        // 3. Products
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "La Liga Teatro Elastico Deer T-Shirt",
                    Description = "La Liga Teatro Elástico's majestic deer puppet graces the front of this soft blue T. This BELLA+CANVAS shirt is retail fit, unisex, and 100% Airlume combed and ring-spun cotton.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, //THIS PRODUCT HAS MULTIPLE AVAILABLE SIZES, Xs,S,XL,XXL
                    Color = ProductColor.Blue,
                    IsActive = true,
                    CategoryId = 1,
                    ImageId = 1
                },
                new Product
                {
                   Id = 2,
                   Name = "La Liga Teatro Elastico Wolf Sweatshirt",
                   Description = "This light blue sweatshirt seems simple from the front but once you turn around, those around you are sure to be intrigued by the sight of La Liga Teatro Elástico's glorious Wolf Puppet, raised above two puppeteer's heads. This sweatshirt is a Gildan full-zip hoodie made of 50/50 cotton and polyester Heavy Blend material.",
                   Quantity = 100,
                   Price = 40.00m,
                   DateAdded = DateTime.UtcNow,
                   Size = null, //THIS PRODUCT HAS MULTIPLE AVAILABLE SIZES S,XL,XXL
                   Color = ProductColor.Blue,
                   IsActive = true,
                   CategoryId = 1,
                   ImageId = 2

                },
                new Product
                {
                    Id = 3,
                    Name = "Puppet Fest Logo Black T-Shirt",
                    Description = "Sport the Festival logo in classic puppeteer black! Most sizes come in both Gildan brand (thicker material, classic fit, 100% cotton) and Next Level brand (thinner material, 60/40 combed ring-spun cotton/polyester). Select your preferred material in the drop down while supplies last.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // THIS PRODUCT HAS MULTIPLE AVAILABLE SIZES, S, M, L, XL, XXL, XXL
                    Color = ProductColor.Black,
                    IsActive = true,
                    CategoryId = 1,
                    ImageId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Puppet Fest Logo Orchid T-Shirt",
                    Description = "\"Head\" out in this colorful Festival logo t-shirt. This soft BELLA+CANVAS shirt is retail fit, unisex, and 100% Airlume combed and ring-spun cotton.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // THIS PRODUCT HAS MULTIPLE AVAILABLE SIZES, S, M, L, XL, 
                    Color = ProductColor.Purple,
                    IsActive = true,
                    CategoryId = 1,
                    ImageId = 4

                },
                new Product
                {
                    Id = 5,
                    Name = "Puppet Fest Logo Purple T-Shirt",
                    Description = "\"Head\" out in this colorful Festival logo t-shirt. This soft BELLA+CANVAS shirt is retail fit, unisex, and 100% Airlume combed and ring-spun cotton.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null,
                    Color = ProductColor.Purple,
                    IsActive = true,
                    CategoryId = 1,
                    ImageId = 5


                },
                new Product
                {
                    Id = 6,
                    Name = "Puppet Fest Logo Tote Bag",
                    Description = "Carry your stuff while repping your favorite Puppet Fest! This 100% cotton canvas tote bag measures 15\"W x 14\"H x 3\"D and features our signature Puppet Head.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // no sizes for this product
                    Color = ProductColor.White, 
                    IsActive = true,
                    CategoryId = 2, // ACCESSORIES
                    ImageId = 6
                },
                new Product
                {
                    Id = 7,
                    Name = "9oz Stemless Wine Glass",
                    Description = "Enjoy a beverage in your own Puppet Festival 9oz, stemless glasses. Each glass measures 3.65\" tall. Grab one for your home so you can think of your favorite festival all year round. Pick up from the Fine Arts Building preferred to discourage damage during shipping.",
                    Quantity = 100,
                    Price = 20.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // no sizes 
                    Color = ProductColor.White, // closest match for clear glass
                    IsActive = true,
                    CategoryId = 3, // DRINKWARE
                    ImageId = 7
                },
                new Product
                {
                    Id = 8,
                    Name = "Book - A Galaxy of Things (Paperback)",
                    Description = "Colette Searls explores how puppets, masks, and material characters shape storytelling in Star Wars and beyond. Using iconic figures like Yoda, R2-D2, and Darth Vader, the book examines how non-human characters use distance, distillation, and duality to create meaning across decades of films and series.",
                    Quantity = 100,
                    Price = 36.16m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // no sizes for books
                    Color = ProductColor.White, // neutral choice for books
                    IsActive = true,
                    CategoryId = 4, // BOOKS
                    ImageId = 8
                },
                new Product
                {
                    Id = 9,
                    Name = "Boy Puppet Button",
                    Description = "This button features a puppet made by Scout Tran—the featured image of the 2023 Chicago International Puppet Theater Festival.",
                    Quantity = 100,
                    Price = 2.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // no sizes for buttons
                    Color = ProductColor.White,
                    IsActive = true,
                    CategoryId = 2, // ACCESSORIES
                    ImageId = 9
                },
                new Product
                {
                    Id = 10,
                    Name = "Die Cut Puppet Stickers (Pack of 4)",
                    Description = "Decorate your world with these glossy 3-inch die cut stickers. This 4-pack includes: 'Make Puppets Not War,' a black-and-white image of La Liga Teatro Elástico's Wolf puppet, a square color image of the Wolf puppets, and the Chicago Puppet Fest puppet head logo.",
                    Quantity = 100,
                    Price = 10.00m,
                    DateAdded = DateTime.UtcNow,
                    Size = null, // no sizes for stickers
                    Color = ProductColor.White,
                    IsActive = true,
                    CategoryId = 2, // ACCESSORIES
                    ImageId = 10
                },
            new Product
            {
                Id = 11,
                Name = "DIY Toy Binoculars",
                Description = "Put together your own toy wooden binoculars featuring the Chicago International Puppet Festival logo.",
                Quantity = 100,
                Price = 5.00m,
                DateAdded = DateTime.UtcNow,
                Size = null, // no sizes for this product
                Color = ProductColor.Brown,
                IsActive = true,
                CategoryId = 2, // ACCESSORIES
                ImageId = 11
            },
            new Product
            {
                Id = 12,
                Name = "Puppet Fest Cuffed Waffle Beanie - Birch",
                Description = "Stay warm and show off your Festival in this Richardson 11 1/2\" knit beanie with an adjustable cuff.",
                Quantity = 100,
                Price = 20.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.Brown,
                IsActive = true,
                CategoryId = 1,
                ImageId = 12
            },
            new Product
            {
                Id = 13,
                Name = "Puppet Fest Cuffed Waffle Beanie - Camel",
                Description = "Stay warm and show off your Festival in this Richardson 11 1/2\" knit beanie with an adjustable cuff. These were a hot seller at the 2025 Puppet Fest!",
                Quantity = 100,
                Price = 20.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.Brown,
                IsActive = true,
                CategoryId = 1,
                ImageId = 13
            },
            new Product
            {
                Id = 14,
                Name = "Puppet Fest Notecards (Pack of 8)",
                Description = "A set of 8 assorted Puppet Fest notecards and envelopes, perfect for thank-you notes, congratulations, or messages of gratitude.",
                Quantity = 100,
                Price = 15.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.White,
                IsActive = true,
                CategoryId = 2,
                ImageId = 14
            },
            new Product
            {
                Id = 15,
                Name = "Puppet Fest Sustainable Rib Cuffed Beanie - Bottle Green",
                Description = "Stay warm and show off your Festival in this Atlantis Headwear 11\" knit beanie with an adjustable cuff. Made from 100% recycled polyester.",
                Quantity = 100,
                Price = 20.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.Green,
                IsActive = true,
                CategoryId = 1,
                ImageId = 15
            },
            new Product
            {
                Id = 16,
                Name = "Puppet Fest Sustainable Rib Cuffed Beanie - Burgundy",
                Description = "Stay warm and show off your Festival in this Atlantis Headwear 11\" knit beanie with an adjustable cuff. Made from 100% recycled polyester.",
                Quantity = 100,
                Price = 20.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.Red,
                IsActive = true,
                CategoryId = 1,
                ImageId = 16
            },
            new Product
            {
                Id = 17,
                Name = "Puppet Head Button 2023",
                Description = "Each January, Festival Artists receive a button that designates them as such for the duration of the Fest. Now you can join them!",
                Quantity = 100,
                Price = 2.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.White,
                IsActive = true,
                CategoryId = 2,
                ImageId = 17
            },
            new Product
            {
                Id = 18,
                Name = "Puppet Head Button 2024",
                Description = "Each January, Festival Artists receive a button that designates them as such for the duration of the Fest. Now you can join them!",
                Quantity = 100,
                Price = 2.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.White,
                IsActive = true,
                CategoryId = 2,
                ImageId = 18
            },
            new Product
            {
                Id = 19,
                Name = "Shadow Puppet: The Hare",
                Description = "This beautiful shadow puppet was inspired by 'The Hare' from Sandglass Theater/Doppelskope's The Amazing Story Machine, featured in the 2025 Chicago Puppet Fest.",
                Quantity = 100,
                Price = 20.00m,
                DateAdded = DateTime.UtcNow,
                Size = null,
                Color = ProductColor.Black,
                IsActive = true,
                CategoryId = 5, // PUPPETS
                ImageId = 19
            }

            );
        }

        // 4. Locations
        if (!context.Locations.Any())
        {
            context.Locations.AddRange(
                new Location
                {
                    Id = 1,
                    Name = "Fine Arts Building",
                    Address = "410 S Michigan Ave"
                }
            );
        }

        // 5. ProductLocations
        if (!context.ProductLocations.Any())
        {
            context.ProductLocations.AddRange(
                new ProductLocation
                {
                    Id = 1,
                    ProductId = 1,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 2,
                    ProductId = 2,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 3,
                    ProductId = 3,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 4,
                    ProductId = 4,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 5,
                    ProductId = 5,
                    LocationId = 1

                 },
                 new ProductLocation
                {
                    Id = 6,
                    ProductId = 6,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 7,
                    ProductId = 7,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 8,
                    ProductId = 8,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 9,
                    ProductId = 9,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 10,
                    ProductId = 10,
                    LocationId = 1
                },
                new ProductLocation
                {
                    Id = 11,
                    ProductId = 11,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 12,
                    ProductId = 12,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 13,
                    ProductId = 13,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 14,
                    ProductId = 14,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 15,
                    ProductId = 15,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 16,
                    ProductId = 16,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 17,
                    ProductId = 17,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 18,
                    ProductId = 18,
                    LocationId = 1
                },
                  new ProductLocation
                {
                    Id = 19,
                    ProductId = 19,
                    LocationId = 1
                }
                
            );
        }

        context.SaveChanges();
    }

}
