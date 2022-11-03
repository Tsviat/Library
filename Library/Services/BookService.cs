using Library.Contracts;
using Library.Data;
using Library.Data.Models;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class BookService : IBookService
    {

        private readonly LibraryDbContext context;

        public BookService(LibraryDbContext _context)
        {
            context = _context;
        }
        public async Task AddBookAsync(AddBookViewModel model)
        {
            var entity = new Book()
            {
                Author = model.Author,
                Title = model.Title,
                CategoryId = model.CategoryId,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                Rating = model.Rating
            };

            await context.Books.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task AddBookToCollectionAsync(int bookId, string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersBooks)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }
            var book = await context.Books.FirstOrDefaultAsync(m => m.Id == bookId);

            if (book == null)
            {
                throw new ArgumentException("Invalid book ID");
            }

            if (!user.ApplicationUsersBooks.Any(m => m.BookId == bookId))
            {
                user.ApplicationUsersBooks.Add(new ApplicationUserBook
                {
                    ApplicationUserId = userId,
                    BookId = bookId,
                    

                });

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<BookViewModel>> GetAllAsync()
        {
            var entities = await context.Books
                .Include(x => x.Category)
                .ToListAsync();

            return entities
                .Select(x => new BookViewModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Category = x?.Category.Name,
                    Author = x.Author,
                    ImageUrl = x.ImageUrl,
                    Rating = x.Rating,
                    Description = x.Description
                    
                });
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<BookViewModel>> GetMyBooksAsync(string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersBooks)
                .ThenInclude(u => u.Book)
                .ThenInclude(u => u.Category)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }


            return user.ApplicationUsersBooks
                .Select(x => new BookViewModel()
                {
                    Id = x.Book.Id,
                    Title = x.Book.Title,
                    Category = x?.Book.Category.Name,
                    Author = x.Book.Author,
                    ImageUrl = x.Book.ImageUrl,
                    Rating = x.Book.Rating,
                    Description = x.Book.Description

                });
        }

        public async Task RemoveBookFromMyBooksAsync(int bookId, string userId)
        {
            var user = await context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.ApplicationUsersBooks)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new ArgumentException("Invalid user ID");
            }
            var book = user.ApplicationUsersBooks.FirstOrDefault(b => b.BookId == bookId);

            if (book != null)
            {
                user.ApplicationUsersBooks.Remove(book);
                await context.SaveChangesAsync();
            }

        }
    }
}
