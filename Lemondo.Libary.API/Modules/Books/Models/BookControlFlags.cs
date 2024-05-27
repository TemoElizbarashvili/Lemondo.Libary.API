namespace Lemondo.Libary.API.Modules.Books.Models;

[Flags]
public enum BookControlFlags : byte
{
    None = 0,
    Basic = 1 << 1,
    Authors = 1 << 2,
    All = Basic | Authors,
}
