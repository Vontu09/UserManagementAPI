using System.ComponentModel.DataAnnotations;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return Task.FromResult(_users.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving users", ex);
            }
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than 0", nameof(id));
                }

                var user = _users.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving user with ID {id}", ex);
            }
        }

        public Task<User> CreateUserAsync(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                // Validate user data
                ValidateUser(user);

                // Check for duplicate email
                if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"User with email '{user.Email}' already exists");
                }

                user.Id = _nextId++;
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
                _users.Add(user);

                return Task.FromResult(user);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the user", ex);
            }
        }

        public Task<User?> UpdateUserAsync(int id, User user)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than 0", nameof(id));
                }

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User cannot be null");
                }

                var existingUser = _users.FirstOrDefault(u => u.Id == id);
                if (existingUser == null)
                {
                    return Task.FromResult<User?>(null);
                }

                // Validate user data
                ValidateUser(user);

                // Check for duplicate email (excluding current user)
                if (_users.Any(u => u.Id != id && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"User with email '{user.Email}' already exists");
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.UpdatedAt = DateTime.UtcNow;

                return Task.FromResult<User?>(existingUser);
            }
            catch (ArgumentException)
            {
                throw;
            }
          
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while updating user with ID {id}", ex);
            }
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("User ID must be greater than 0", nameof(id));
                }

                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    return Task.FromResult(false);
                }

                _users.Remove(user);
                return Task.FromResult(true);
            }
           
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting user with ID {id}", ex);
            }
        }

        /// <summary>
        /// Validates a user object against data annotations
        /// </summary>
        private void ValidateUser(User user)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user, serviceProvider: null, items: null);

            if (!Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true))
            {
                var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                throw new ArgumentException($"User validation failed: {errors}");
            }
        }
    }
}