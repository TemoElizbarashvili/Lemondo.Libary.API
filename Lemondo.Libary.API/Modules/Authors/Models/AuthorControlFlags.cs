namespace Lemondo.Libary.API.Modules.Authors.Models;

[Flags]
public enum AuthorControlFlags : byte
{
    Basic = 1 << 1,
    Books = 1 << 2,
}