﻿namespace Lemondo.Libary.API.Modules.Auth.Models;

public class User
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}
