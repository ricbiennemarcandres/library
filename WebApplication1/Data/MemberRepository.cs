using library.Models;
using Npgsql;

namespace library.Data;

public class MemberRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public MemberRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    // READ: Convert one database row into a Member object.
    private static Member ReadMember(NpgsqlDataReader reader)
    {
        return new Member
        {
            MemberId = reader.GetInt64(0),
            FullName = reader.GetString(1),
            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
            MemberType = reader.GetString(3)
        };
    }

    // READ: Get all members.
    public async Task<List<Member>> GetAllAsync()
    {
        const string sql = "SELECT member_id, full_name, email, member_type " +
                           "FROM lending.member " +
                           "ORDER BY full_name;";

        var members = new List<Member>();

        await using var command = _dataSource.CreateCommand(sql);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            members.Add(ReadMember(reader));
        }

        return members;
    }

    // CREATE: Add a new member.
    public async Task AddAsync(Member member)
    {
        const string sql = "INSERT INTO lending.member " +
                           "(full_name, email, member_type) " +
                           "VALUES (@full_name, @email, @member_type);";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("full_name", member.FullName);

        // Convert C# null into database NULL.
        command.Parameters.AddWithValue(
            "email",
            (object?)member.Email ?? DBNull.Value
        );

        command.Parameters.AddWithValue("member_type", member.MemberType);

        await command.ExecuteNonQueryAsync();
    }
}