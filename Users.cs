public interface IUsers {
    public bool ValidateUser(string userName, string userPasswordHash);
}

public class Users : IUsers
{
    private Dictionary<string, string> users = new Dictionary<string, string>();

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

    public bool ValidateUser(string userName, string userPassword)
    {
        throw new NotImplementedException();
    }
}
