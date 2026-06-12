using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Walmart.Domain.Entities;

namespace Walmart.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private static readonly byte[] PlaceholderImage = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+X2ioAAAAASUVORK5CYII=");

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();

            var hasProducts = await _context.Set<Product>().AnyAsync();
            if (!hasProducts)
            {
                await SeedCategoriesAsync();
                await SeedProductsAsync();
                await SeedAddressesAsync();
                await SeedOrdersAsync();
            }

            await EnsureProfilePicturesAsync();
            await EnsureProductPicturesAsync();

            await _context.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Client", "StoreManager", "SupportAgent" };

            foreach (var roleName in roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            
            var adminEmail = "admin@walmart.com";
            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            
            var customers = new[]
            {
                new { Email = "john.doe@email.com", FirstName = "John", LastName = "Doe" },
                new { Email = "jane.smith@email.com", FirstName = "Jane", LastName = "Smith" },
                new { Email = "bob.wilson@email.com", FirstName = "Bob", LastName = "Wilson" },
                new { Email = "alice.brown@email.com", FirstName = "Alice", LastName = "Brown" },
                new { Email = "charlie.davis@email.com", FirstName = "Charlie", LastName = "Davis" },
                new { Email = "linda.evans@email.com", FirstName = "Linda", LastName = "Evans" },
                new { Email = "michael.clark@email.com", FirstName = "Michael", LastName = "Clark" }
            };

            foreach (var customer in customers)
            {
                if (await _userManager.FindByEmailAsync(customer.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = customer.Email,
                        Email = customer.Email,
                        EmailConfirmed = true,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        PhoneNumber = "+1-555-01" + Random.Shared.Next(10, 99)
                    };

                    var result = await _userManager.CreateAsync(user, "Customer@123");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Client");
                    }
                }
            }

            await EnsureRoleUserAsync("StoreManager", "manager@walmart.com", "Store", "Manager", "Manager@123");
            await EnsureRoleUserAsync("SupportAgent", "support@walmart.com", "Support", "Agent", "Support@123");
        }

        private async Task SeedCategoriesAsync()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Home & Garden" },
                new Category { Id = 4, Name = "Sports & Outdoors" },
                new Category { Id = 5, Name = "Books" },
                new Category { Id = 6, Name = "Toys & Games" },
                new Category { Id = 7, Name = "Food & Grocery" }
            };

            await _context.Set<Category>().AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            
            var subcategories = new List<Category>
            {
                new Category { Name = "Smartphones", ParentCategoryId = 1 },
                new Category { Name = "Laptops", ParentCategoryId = 1 },
                new Category { Name = "TVs", ParentCategoryId = 1 },
                new Category { Name = "Men's Clothing", ParentCategoryId = 2 },
                new Category { Name = "Women's Clothing", ParentCategoryId = 2 },
                new Category { Name = "Furniture", ParentCategoryId = 3 },
                new Category { Name = "Garden Tools", ParentCategoryId = 3 }
            };

            await _context.Set<Category>().AddRangeAsync(subcategories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedProductsAsync()
        {
            var products = new List<Product>
            {
                
                new Product
                {
                    Name = "iPhone 15 Pro Max",
                    Sku = "IPHONE-15-PRO-MAX",
                    Price = 1199.99m,
                    StockQuantity = 50,
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Samsung Galaxy S24 Ultra",
                    Sku = "SAMSUNG-S24-ULTRA",
                    Price = 1099.99m,
                    StockQuantity = 35,
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-28),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "MacBook Pro 16\"",
                    Sku = "MBP-16-2024",
                    Price = 2499.99m,
                    StockQuantity = 20,
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Dell XPS 15",
                    Sku = "DELL-XPS-15",
                    Price = 1799.99m,
                    StockQuantity = 15,
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Sony 65\" 4K TV",
                    Sku = "SONY-TV-65",
                    Price = 899.99m,
                    StockQuantity = 8,
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "Men's Levi's Jeans",
                    Sku = "LEVIS-JEANS-M",
                    Price = 59.99m,
                    StockQuantity = 100,
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Nike Running Shoes",
                    Sku = "NIKE-RUN-SHOES",
                    Price = 129.99m,
                    StockQuantity = 75,
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Women's Summer Dress",
                    Sku = "DRESS-SUMMER-W",
                    Price = 49.99m,
                    StockQuantity = 60,
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Adidas Sports T-Shirt",
                    Sku = "ADIDAS-TSHIRT",
                    Price = 29.99m,
                    StockQuantity = 120,
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "IKEA Office Desk",
                    Sku = "IKEA-DESK-01",
                    Price = 299.99m,
                    StockQuantity = 25,
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Garden Tool Set",
                    Sku = "GARDEN-TOOLS-SET",
                    Price = 79.99m,
                    StockQuantity = 40,
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "LED Desk Lamp",
                    Sku = "LED-LAMP-DESK",
                    Price = 39.99m,
                    StockQuantity = 85,
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "Yoga Mat Premium",
                    Sku = "YOGA-MAT-PRO",
                    Price = 34.99m,
                    StockQuantity = 90,
                    CategoryId = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Camping Tent 4-Person",
                    Sku = "TENT-CAMP-4P",
                    Price = 159.99m,
                    StockQuantity = 12,
                    CategoryId = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Basketball Official Size",
                    Sku = "BASKETBALL-OFF",
                    Price = 24.99m,
                    StockQuantity = 55,
                    CategoryId = 4,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "Clean Code by Robert Martin",
                    Sku = "BOOK-CLEAN-CODE",
                    Price = 39.99m,
                    StockQuantity = 45,
                    CategoryId = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "The Pragmatic Programmer",
                    Sku = "BOOK-PRAGMATIC",
                    Price = 44.99m,
                    StockQuantity = 32,
                    CategoryId = 5,
                    CreatedAt = DateTime.UtcNow.AddHours(-20),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "LEGO Star Wars Set",
                    Sku = "LEGO-SW-2024",
                    Price = 89.99m,
                    StockQuantity = 28,
                    CategoryId = 6,
                    CreatedAt = DateTime.UtcNow.AddHours(-15),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "PlayStation 5",
                    Sku = "PS5-CONSOLE",
                    Price = 499.99m,
                    StockQuantity = 5,
                    CategoryId = 6,
                    CreatedAt = DateTime.UtcNow.AddHours(-10),
                    ProductPicture = PlaceholderImage
                },

                
                new Product
                {
                    Name = "Organic Coffee Beans 1kg",
                    Sku = "COFFEE-ORG-1KG",
                    Price = 19.99m,
                    StockQuantity = 150,
                    CategoryId = 7,
                    CreatedAt = DateTime.UtcNow.AddHours(-5),
                    ProductPicture = PlaceholderImage
                },
                new Product
                {
                    Name = "Premium Olive Oil 500ml",
                    Sku = "OLIVE-OIL-500",
                    Price = 14.99m,
                    StockQuantity = 200,
                    CategoryId = 7,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    ProductPicture = PlaceholderImage
                }
            };

            await _context.Set<Product>().AddRangeAsync(products);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAddressesAsync()
        {
            var users = await _userManager.Users.Where(u => u.Email != "admin@walmart.com").ToListAsync();
            var addresses = new List<Address>();

            foreach (var user in users.Take(3))
            {
                addresses.Add(new Address
                {
                    UserId = user.Id,
                    Street = "123 Main Street",
                    City = "New York",
                    Country = "USA",
                    ZIP = "10001",
                    IsDefault = true
                });

                addresses.Add(new Address
                {
                    UserId = user.Id,
                    Street = "456 Second Avenue",
                    City = "Los Angeles",
                    Country = "USA",
                    ZIP = "90001",
                    IsDefault = false
                });
            }

            await _context.Set<Address>().AddRangeAsync(addresses);
            await _context.SaveChangesAsync();
        }

        private async Task SeedOrdersAsync()
        {
            var users = await _userManager.Users.Where(u => u.Email != "admin@walmart.com").ToListAsync();
            var addresses = await _context.Set<Address>().ToListAsync();
            var products = await _context.Set<Product>().ToListAsync();

            if (!users.Any() || !addresses.Any() || !products.Any())
                return;

            var orders = new List<Order>();

            
            var order1 = new Order
            {
                UserId = users[0].Id,
                ShippingAddressId = addresses.First(a => a.UserId == users[0].Id).Id,
                OrderNumber = "WM-2024-0001",
                Status = 3, 
                OrderDate = DateTime.UtcNow.AddDays(-15),
                TotalAmount = 1329.98m
            };
            orders.Add(order1);

            
            var order2 = new Order
            {
                UserId = users[0].Id,
                ShippingAddressId = addresses.First(a => a.UserId == users[0].Id).Id,
                OrderNumber = "WM-2024-0002",
                Status = 2, 
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TotalAmount = 189.98m
            };
            orders.Add(order2);

            
            Order? order3 = null;
            if (users.Count > 1)
            {
                order3 = new Order
                {
                    UserId = users[1].Id,
                    ShippingAddressId = addresses.First(a => a.UserId == users[1].Id).Id,
                    OrderNumber = "WM-2024-0003",
                    Status = 1, 
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    TotalAmount = 2499.99m
                };
                orders.Add(order3);
            }

            
            Order? order4 = null;
            if (users.Count > 1)
            {
                order4 = new Order
                {
                    UserId = users[1].Id,
                    ShippingAddressId = addresses.First(a => a.UserId == users[1].Id).Id,
                    OrderNumber = "WM-2024-0004",
                    Status = 0, 
                    OrderDate = DateTime.UtcNow.AddHours(-6),
                    TotalAmount = 674.97m
                };
                orders.Add(order4);
            }

            
            Order? order5 = null;
            if (users.Count > 2)
            {
                order5 = new Order
                {
                    UserId = users[2].Id,
                    ShippingAddressId = addresses.First(a => a.UserId == users[2].Id).Id,
                    OrderNumber = "WM-2024-0005",
                    Status = 2, 
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    TotalAmount = 139.97m
                };
                orders.Add(order5);
            }

            await _context.Set<Order>().AddRangeAsync(orders);
            await _context.SaveChangesAsync();

            
            var orderProducts = new List<OrderProduct>();

            
            orderProducts.Add(new OrderProduct { OrderId = order1.Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = products[0].Price, LineTotal = products[0].Price }); 
            orderProducts.Add(new OrderProduct { OrderId = order1.Id, ProductId = products[6].Id, Quantity = 1, UnitPrice = products[6].Price, LineTotal = products[6].Price }); 

            
            orderProducts.Add(new OrderProduct { OrderId = order2.Id, ProductId = products[12].Id, Quantity = 1, UnitPrice = products[12].Price, LineTotal = products[12].Price }); 
            orderProducts.Add(new OrderProduct { OrderId = order2.Id, ProductId = products[8].Id, Quantity = 1, UnitPrice = products[8].Price, LineTotal = products[8].Price }); 

            
            if (order3 != null)
            {
                orderProducts.Add(new OrderProduct { OrderId = order3.Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = products[2].Price, LineTotal = products[2].Price }); 
            }

            
            if (order4 != null)
            {
                orderProducts.Add(new OrderProduct { OrderId = order4.Id, ProductId = products[17].Id, Quantity = 2, UnitPrice = products[17].Price, LineTotal = products[17].Price * 2 }); 
                orderProducts.Add(new OrderProduct { OrderId = order4.Id, ProductId = products[9].Id, Quantity = 1, UnitPrice = products[9].Price, LineTotal = products[9].Price }); 
            }

            
            if (order5 != null)
            {
                orderProducts.Add(new OrderProduct { OrderId = order5.Id, ProductId = products[11].Id, Quantity = 1, UnitPrice = products[11].Price, LineTotal = products[11].Price }); 
                orderProducts.Add(new OrderProduct { OrderId = order5.Id, ProductId = products[15].Id, Quantity = 1, UnitPrice = products[15].Price, LineTotal = products[15].Price }); 
                orderProducts.Add(new OrderProduct { OrderId = order5.Id, ProductId = products[5].Id, Quantity = 1, UnitPrice = products[5].Price, LineTotal = products[5].Price }); 
            }

            await _context.Set<OrderProduct>().AddRangeAsync(orderProducts);
            await _context.SaveChangesAsync();
        }

        private async Task EnsureRoleUserAsync(string role, string email, string firstName, string lastName, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    ProfilePicture = PlaceholderImage
                };

                var create = await _userManager.CreateAsync(user, password);
                if (!create.Succeeded)
                    return;
            }

            if (!await _userManager.IsInRoleAsync(user, role))
                await _userManager.AddToRoleAsync(user, role);
        }

        private async Task EnsureProfilePicturesAsync()
        {
            var users = await _userManager.Users.Where(u => u.ProfilePicture == null).ToListAsync();
            if (!users.Any())
                return;

            foreach (var user in users)
                user.ProfilePicture = PlaceholderImage;

            _context.UpdateRange(users);
        }

        private async Task EnsureProductPicturesAsync()
        {
            var productsWithoutImage = await _context.Set<Product>()
                .Where(p => p.ProductPicture == null)
                .ToListAsync();

            if (!productsWithoutImage.Any())
                return;

            foreach (var product in productsWithoutImage)
                product.ProductPicture = PlaceholderImage;

            _context.UpdateRange(productsWithoutImage);
        }
    }
}
