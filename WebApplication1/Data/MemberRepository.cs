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

    // READ: every member, with how many books each has out now.
    public async Task<List<Member>> GetAllAsync()
    {
        // 1. The SQL: every member, joined to their open loans, which are counted.
        const string sql =
            "SELECT m.member_id, m.full_name, m.email, m.member_type, COUNT(l.loan_id) " +
            "FROM lending.member m " +
            "LEFT JOIN lending.loan l ON l.member_id = m.member_id AND l.returned_on IS NULL " +
            "GROUP BY m.member_id " +
            "ORDER BY m.full_name;";                                                   // B
                                                                                       // 2. An empty list, to fill.
        var members = new List<Member>();                                              // H
                                                                                       // 3. A command that will run the SQL.
        await using var command = _dataSource.CreateCommand(sql);                      // G
                                                                                       // 4. Run it, and get a reader for the rows.
        await using var reader = await command.ExecuteReaderAsync();                   // A
        while (await reader.ReadAsync())                                               // 5. E
        {
            // 6. Copy the row into a Member.
            var member = ReadMember(reader);                                           // D
                                                                                       // 7. Add the count from the fifth column, which only this SELECT has.
            member.OpenLoans = reader.GetInt64(4);                                     // F
                                                                                       // 8. Add the member to the list.
            members.Add(member);                                                       // I
        }
        // 9. Give the list back.
        return members;                                                                // C
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

    // READ: Get one member using their ID.
    public async Task<Member?> GetByIdAsync(long id)
    {
        const string sql = "SELECT member_id, full_name, email, member_type " +
                           "FROM lending.member " +
                           "WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("id", id);

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? ReadMember(reader) : null;
    }

    // UPDATE: Change the information of a member.
    public async Task UpdateAsync(Member member)
    {
        const string sql = "UPDATE lending.member " +
                           "SET full_name = @full_name, " +
                           "email = @email, " +
                           "member_type = @member_type " +
                           "WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("full_name", member.FullName);

        command.Parameters.AddWithValue(
            "email",
            (object?)member.Email ?? DBNull.Value
        );

        command.Parameters.AddWithValue("member_type", member.MemberType);
        command.Parameters.AddWithValue("id", member.MemberId);

        await command.ExecuteNonQueryAsync();
    }

    // DELETE: Remove one member.
    public async Task DeleteAsync(long id)
    {
        const string sql = "DELETE FROM lending.member WHERE member_id = @id;";

        await using var command = _dataSource.CreateCommand(sql);

        command.Parameters.AddWithValue("id", id);

        await command.ExecuteNonQueryAsync();
    }
}