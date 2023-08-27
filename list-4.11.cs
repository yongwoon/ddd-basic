class User {
  public User(UserName name) {
    if (name == null) {
      throw new ArgumentNullException(nameof(name));
    }

    Id = new UserId(Guid.NewGuid().ToString());
    Name = name;
  }

  public UserId Id { get; }
  public UserName Name { get; }
}

// ----------------------------------------

class UserId {
  public UserId(string value) {
    if (value == null) {
      throw new ArgumentNullException(nameof(value));
    }

    Value = value;
  }

  public string Value { get; }
}

// ----------------------------------------

class UserName {
  public UserName(string value) {
    if (value == null) {
      throw new ArgumentNullException(nameof(value));
    }

    if (value.Length < 3) {
      throw new ArgumentException("ユーザー名は3文字以上です。", nameof(value));
    }

    Value = value;
  }

  public string Value { get; }
}

// ----------------------------------------

class UserService {
  public bool Exists(User user) {
    var connectionString = ConfigurationManager.ConnectionStrings["UserDb"].ConnectionString;

    using(var connection = new SqlConnection(connectionString))
    using(var command = connection.CreateCommand()) {
      connection.Open();

      command.CommandText = "SELECT COUNT(*) FROM Users WHERE Name = @Name";
      command.Parameters.Add(newSqlParameter("@Name", user.Name.Value));

      using(var reader = command.ExecuteReader()) {
        var exist = reader.Read();
        return exist;
      }
    }
  }
}

// ----------------------------------------

class Program {
  public void CreateUser(string UserName) {
    var user = new User(new UserName(UserName));
    var userService = new UserService();

    if (userService.Exists(user)) {
      throw new Eeception($"{userName}  は既に存在しています。")
    }

    var connectionString = ConfigurationManager.ConnectionStrings["UserDb"].ConnectionString;

    using(var connection = new SqlConnection(connectionString))
    using(var command = connection.CreateCommand()) {
      connection.Open();

      command.CommandText = "INSERT INTO Users (Id, Name) VALUES (@Id, @Name)";
      command.Parameters.Add(newSqlParameter("@Id", user.Id.Value));
      command.Parameters.Add(newSqlParameter("@Name", user.Name.Value));
      command.ExecuteNonQuery();
    }
  }
}