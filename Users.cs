using BC = BCrypt.Net.BCrypt;

/// <summary>
/// Interface for user management in api.
/// </summary>
public interface IUsers {
    /// <summary>
    /// Validate if user is a valid user and has a valid passowrd.
    /// </summary>
    /// <param name="userName">User name of user</param>
    /// <param name="userPassword">Password of user</param>
    /// <returns>True if user exists and password match, else false</returns>
    public bool ValidateUser(string userName, string userPassword);
}

/// <summary>
/// Implementation of user management backed by file.
/// </summary>
/// <remarks>
/// The location of file is specified in configuration file with name <c>UserFile</c>
///
/// The format of the file is username tab, then BCrypt hashed password.
///
/// All users need to be unique.
/// </remarks>
public class Users : IUsers
{
    private Dictionary<string, string> users = new Dictionary<string, string>();

    /// <summary>
    /// Reads in <c>UserFile</c> location.
    /// </summary>
    public Users(ILogger<Users> logger, IConfiguration configuration) {
        string userFile = configuration["UsersFile"];
        logger.LogInformation($"Start reading in users from file {userFile}");

        using (var sr = new StreamReader(userFile)) {
            string? line;
            int lineNum = 0;

            while ((line = sr.ReadLine()) != null) {
                lineNum += 1;
                var d = line.Split("\t");
                if (d.Length < 2) {
                    logger.LogCritical($"Could not read user on line {lineNum}");
                } else {
                    var user = d[0];
                    var hashedPwd = d[1];
                    logger.LogDebug($"Got user with {user} and hashed password {hashedPwd}");

                    users.Add(user, hashedPwd);
                }
            }
        }
    }

    /// <inheritdoc />
    public bool ValidateUser(string userName,
                             string userPassword) {
        string? hashedPassword;
        if (users.TryGetValue(userName, out hashedPassword)) {
            return BC.Verify(userPassword, hashedPassword);
        }

        return false;
    }
}
