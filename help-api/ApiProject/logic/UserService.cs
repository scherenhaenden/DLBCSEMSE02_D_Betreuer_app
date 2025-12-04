using ApiProject.Db;

namespace ApiProject.Logic;

public sealed class UserService : IUserService
{
    private readonly List<User> _users = new();
    private readonly List<Role> _roles = new();

    public UserService()
    {
        // Seed known roles
        EnsureRole("STUDENT");
        EnsureRole("TUTOR");
        EnsureRole("SECOND_SUPERVISOR");
    }

    public IReadOnlyCollection<User> GetAll()
    {
        return _users.AsReadOnly();
    }

    public User? GetById(Guid id)
    {
        return _users.SingleOrDefault(u => u.Id == id);
    }

    public Role EnsureRole(string roleName)
    {
        var normalized = roleName.Trim().ToUpperInvariant();

        var existing = _roles.SingleOrDefault(r => r.Name == normalized);
        if (existing is not null)
        {
            return existing;
        }

        var role = new Role
        {
            Name = normalized
        };

        _roles.Add(role);
        return role;
    }

    public bool UserHasRole(Guid userId, string roleName)
    {
        var user = GetById(userId);
        if (user is null)
        {
            return false;
        }

        var normalized = roleName.Trim().ToUpperInvariant();

        return user.UserRoles.Any(ur =>
            ur.Role is not null &&
            ur.Role.Name == normalized);
    }

    public User CreateUser(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        IEnumerable<string> roleNames)
    {
        if (_users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A user with this e-mail already exists.");
        }

        var user = new User
        {
            FirstName    = firstName.Trim(),
            LastName     = lastName.Trim(),
            Email        = email.Trim(),
            PasswordHash = passwordHash // assume already hashed
        };

        foreach (var roleName in roleNames)
        {
            var role = EnsureRole(roleName);

            var userRole = new UserRole
            {
                User   = user,
                UserId = user.Id,
                Role   = role,
                RoleId = role.Id
            };

            user.UserRoles.Add(userRole);
            role.UserRoles.Add(userRole);
        }

        _users.Add(user);
        return user;
    }
}

